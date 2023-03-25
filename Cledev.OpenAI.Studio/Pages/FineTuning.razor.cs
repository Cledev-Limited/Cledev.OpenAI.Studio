using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Files;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.JSInterop;

namespace Cledev.OpenAI.Studio.Pages;

public class FineTuningPage : PageComponentBase
{
    protected CreateFineTuneRequest CreateFineTuneRequest { get; set; } = null!;
    protected string? ClassificationBeta { get; set; }

    public IList<FineTuneFile> ExistingFiles { get; set; } = new List<FineTuneFile>();
    public List<string> FineTuningModels { get; set; } = new();

    public List<FineTuneResponse> FineTunes { get; set; } = new();
    public FineTuneResponse? FineTuneResponse { get; set; }
    public ListFineTuneEventsResponse? ListFineTuneEventsResponse { get; set; }

    public bool IsCreating { get; set; }
    protected Error? CreateError { get; set; }

    public string? FineTuneIdToCancel { get; set; }
    public bool IsCancelling { get; set; }
    protected Error? CancelError { get; set; }

    public bool IsInfoLoading { get; set; }
    protected Error? InfoError { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsProcessing = true;

        CreateFineTuneRequest = new CreateFineTuneRequest
        {
            Model = FineTuningModel.Curie.ToStringModel(),
            TrainingFile = string.Empty,
            NEpochs = 4,
            PromptLossWeight = 0.01f
        };

        var files = await OpenAIClient.ListFiles();
        ExistingFiles = files is not null ? files.Data.ToFineTuneFiles() : new List<FineTuneFile>();

        CreateFineTuneRequest.TrainingFile = ExistingFiles.Any() ? ExistingFiles.First().FileId : string.Empty;

        await LoadFineTunes();
    }

    protected async Task OnSubmitAsync()
    {
        IsCreating = true;
        CreateError = null;

        var response = await OpenAIClient.CreateFineTune(CreateFineTuneRequest);
        CreateError = response?.Error;
        if (CreateError is null)
        {
            await JsRuntime.InvokeVoidAsync("toggleModal", "CreateFineTuneModal");
            await LoadFineTunes();
        }

        IsCreating = false;
    }

    protected async Task LoadFineTunes()
    {
        IsProcessing = true;

        FineTunes.Clear();
        FineTuningModels.Clear();

        StateHasChanged();

        var response = await OpenAIClient.ListFineTunes();
        Error = response?.Error;

        FineTuningModels = Enum.GetValues(typeof(FineTuningModel)).Cast<FineTuningModel>().Select(x => x.ToStringModel()).ToList();

        if (response is not null)
        {
            FineTunes.AddRange(response.Data.OrderBy(fineTuneResponse => fineTuneResponse.CreatedAt));

            var listModelsResponse = await OpenAIClient.ListModels();
            if (listModelsResponse is not null && listModelsResponse.Error is null)
            {
                FineTuningModels.AddRange(listModelsResponse.Data.Where(model => model.OwnedBy == StudioSettings.Value.OrganizationName).Select(model => model.Id).ToList());
            }
        }

        IsProcessing = false;
    }

    protected async Task LoadFineTuneDetails(string id)
    {
        IsInfoLoading = true;
        InfoError = null;
        FineTuneResponse = null;

        FineTuneResponse = await OpenAIClient.RetrieveFineTune(id);
        InfoError = FineTuneResponse?.Error;

        IsInfoLoading = false;
    }

    protected async Task LoadFineTuneEvents(string id)
    {
        IsInfoLoading = true;
        InfoError = null;
        ListFineTuneEventsResponse = null;

        ListFineTuneEventsResponse = await OpenAIClient.ListFineTuneEvents(id);
        InfoError = ListFineTuneEventsResponse?.Error;
        
        IsInfoLoading = false;
    }

    protected void SetFineTuneIdToCancel(string fineTuneIdToCancel)
    {
        CancelError = null;
        FineTuneIdToCancel = fineTuneIdToCancel;
    }

    protected async Task CancelFineTuneJob()
    {
        CancelError = null;
        IsCancelling = true;

        var fineTuneResponse = await OpenAIClient.CancelFineTune(FineTuneIdToCancel!);
        CancelError = fineTuneResponse?.Error;
        if (CancelError is null)
        {
            await JsRuntime.InvokeVoidAsync("toggleModal", "CancelFineTuneJobModal");
            await LoadFineTunes();
        }

        IsCancelling = false;
    }

    protected void AddClassificationBeta()
    {
        if (ClassificationBeta is null)
        {
            return;
        }

        CreateFineTuneRequest.ClassificationBetas ??= new List<string>();
        CreateFineTuneRequest.ClassificationBetas!.Add(ClassificationBeta);
        ClassificationBeta = null;
    }

    protected void RemoveClassificationBeta(string classificationBeta)
    {
        if (CreateFineTuneRequest.ClassificationBetas!.FirstOrDefault(cb => cb == classificationBeta) is null)
        {
            return;
        }

        CreateFineTuneRequest.ClassificationBetas!.Remove(classificationBeta);

        if (CreateFineTuneRequest.ClassificationBetas.Any() is false)
        {
            CreateFineTuneRequest.ClassificationBetas = null;
        }
    }

    protected static class Tooltips
    {
        public static string Model = "Optional (Defaults to curie). The name of the base model to fine-tune. You can select one of \"ada\", \"babbage\", \"curie\", \"davinci\", or a fine-tuned model created after 2022-04-21. To learn more about these models, see the Models documentation.";
        public static string TrainingFile = "Required. The ID of an uploaded file that contains training data. Your dataset must be formatted as a JSONL file, where each training example is a JSON object with the keys \"prompt\" and \"completion\". Additionally, you must upload your file with the purpose fine-tune.";
        public static string ValidationFile = "Optional. The ID of an uploaded file that contains validation data. If you provide this file, the data is used to generate validation metrics periodically during fine-tuning. These metrics can be viewed in the fine-tuning results file. Your train and validation data should be mutually exclusive. Your dataset must be formatted as a JSONL file, where each validation example is a JSON object with the keys \"prompt\" and \"completion\". Additionally, you must upload your file with the purpose fine-tune.";
        public static string NEpochs = "Optional (Defaults to 4). The number of epochs to train the model for. An epoch refers to one full cycle through the training dataset.";
        public static string BatchSize = "Optional (Defaults to null). The batch size to use for training. The batch size is the number of training examples used to train a single forward and backward pass. By default, the batch size will be dynamically configured to be ~0.2% of the number of examples in the training set, capped at 256 - in general, we've found that larger batch sizes tend to work better for larger datasets.";
        public static string LearningRateMultiplier = "Optional (Defaults to null). The learning rate multiplier to use for training. The fine-tuning learning rate is the original learning rate used for pretraining multiplied by this value. By default, the learning rate multiplier is the 0.05, 0.1, or 0.2 depending on final batch_size (larger learning rates tend to perform better with larger batch sizes). We recommend experimenting with values in the range 0.02 to 0.2 to see what produces the best results.";
        public static string PromptLossWeight = "Optional (Defaults to 0.01). The weight to use for loss on the prompt tokens. This controls how much the model tries to learn to generate the prompt (as compared to the completion which always has a weight of 1.0), and can add a stabilizing effect to training when completions are short. If prompts are extremely long (relative to completions), it may make sense to reduce this weight so as to avoid over-prioritizing learning the prompt.";
        public static string ComputeClassificationMetrics = "Optional (Defaults to false). If set, we calculate classification-specific metrics such as accuracy and F-1 score using the validation set at the end of every epoch. These metrics can be viewed in the results file. In order to compute classification metrics, you must provide a validation_file. Additionally, you must specify classification_n_classes for multiclass classification or classification_positive_class for binary classification.";
        public static string ClassificationNClasses = "Optional (Defaults to null). The number of classes in a classification task. This parameter is required for multiclass classification.";
        public static string ClassificationPositiveClass = "Optional (Defaults to null). This parameter is required for multiclass classification. This parameter is needed to generate precision, recall, and F1 metrics when doing binary classification.";
        public static string ClassificationBetas = "Optional (Defaults to null). If this is provided, we calculate F-beta scores at the specified beta values. The F-beta score is a generalization of F-1 score. This is only used for binary classification. With a beta of 1 (i.e. the F-1 score), precision and recall are given the same weight. A larger beta score puts more weight on recall and less on precision. A smaller beta score puts more weight on precision and less on recall.";
        public static string Suffix = "Optional (Defaults to null). A string of up to 40 characters that will be added to your fine-tuned model name. For example, a suffix of \"custom-model-name\" would produce a model name like ada:ft-your-org:custom-model-name-2022-02-15-04-21-04.";
    }
}

public record FineTuneFile(string FileId, string FileDisplayName);

public static class FineTuningExtensionsExtensions
{
    public static List<FineTuneFile> ToFineTuneFiles(this IEnumerable<FileResponse> data)
    {
        return data
            .Where(fileResponse => fileResponse.Purpose == "fine-tune")
            .OrderBy(fileResponse => fileResponse.CreatedAt)
            .Select(fileResponse => new FineTuneFile(fileResponse.Id, $"{fileResponse.FileName} ({fileResponse.Id})"))
            .ToList();
    }
}
