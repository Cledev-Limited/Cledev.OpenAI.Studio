using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.JSInterop;

namespace Cledev.OpenAI.Studio.Pages;

public class FineTuningPage : PageComponentBase
{
    protected CreateFineTuneRequest CreateFineTuneRequest { get; set; } = null!;
    public IList<string> ExistingFiles { get; set; } = new List<string>();
    public IList<string> FineTuningModels { get; set; } = new List<string>();

    public List<FineTuneResponse> FineTunes { get; set; } = new();
    
    public bool IsCreating { get; set; }
    protected Error? CreateError { get; set; }

    public string? FineTuneModelToDelete { get; set; }
    public bool IsDeleting { get; set; }
    protected Error? DeleteError { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsProcessing = true;

        CreateFineTuneRequest = new CreateFineTuneRequest
        {
            Model = FineTuningModel.Curie.ToStringModel(),
            TrainingFile = string.Empty
        };

        FineTuningModels = Enum.GetValues(typeof(FineTuningModel)).Cast<FineTuningModel>().Select(x => x.ToStringModel()).ToList();

        var files = await OpenAIClient.ListFiles();
        ExistingFiles = files is not null ? files.Data.Select(file => file.Id).ToList() : new List<string>();

        CreateFineTuneRequest.TrainingFile = ExistingFiles.Any() ? ExistingFiles.First() : string.Empty;

        await LoadFineTunes();
    }

    protected async Task OnSubmitAsync()
    {
        IsCreating = true;

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

        StateHasChanged();

        var response = await OpenAIClient.ListFineTunes();
        Error = response?.Error;
        if (response is not null)
        {
            FineTunes.AddRange(response.Data.OrderBy(fineTuneResponse => fineTuneResponse.CreatedAt));
        }

        IsProcessing = false;
    }

    protected void SetFineTuneModelToDelete(string fineTuneModelToDelete)
    {
        FineTuneModelToDelete = fineTuneModelToDelete;
    }

    protected async Task DeleteFineTuneModel()
    {
        IsDeleting = true;

        var deleteFileResponse = await OpenAIClient.DeleteFineTune(FineTuneModelToDelete!);
        DeleteError = deleteFileResponse?.Error;
        if (DeleteError is null)
        {
            await JsRuntime.InvokeVoidAsync("toggleModal", "DeleteFineTuneModal");
            await LoadFineTunes();
        }

        IsDeleting = false;
    }
}
