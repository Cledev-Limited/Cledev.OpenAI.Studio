using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cledev.OpenAI.Studio.Pages;

public class ModelsPage : PageComponentBase
{
    protected string? ModelId { get; set; }
    protected bool SearchCompleted { get; set; }

    public List<ModelResponse> Models { get; set; } = new();

    public string? FineTunedModelToDelete { get; set; }
    public bool IsDeleting { get; set; }
    protected Error? DeleteError { get; set; }

    protected void OnValueChanged(ChangeEventArgs e)
    {
        ModelId = e.Value?.ToString();
    }

    protected async Task OnSubmitAsync()
    {
        await LoadModels();
    }

    private async Task LoadModels()
    {
        IsProcessing = true;
        SearchCompleted = false;
        Models.Clear();

        if (string.IsNullOrEmpty(ModelId))
        {
            var response = await OpenAIClient.ListModels();
            Error = response?.Error;
            if (response is not null)
            {
                Models.AddRange(response.Data);
            }
        }
        else
        {
            var response = await OpenAIClient.RetrieveModel(ModelId);
            Error = response?.Error;
            if (response is not null)
            {
                Models.Add(response);
            }
        }

        IsProcessing = false;
        SearchCompleted = true;
    }

    protected void SetFineTunedModelToDelete(string fineTuneModelToDelete)
    {
        DeleteError = null;
        FineTunedModelToDelete = fineTuneModelToDelete;
    }

    protected async Task DeleteFineTunedModel()
    {
        DeleteError = null;
        IsDeleting = true;

        var deleteFineTunedModelResponse = await OpenAIClient.DeleteFineTunedModel(FineTunedModelToDelete!);
        DeleteError = deleteFineTunedModelResponse?.Error;
        if (DeleteError is null)
        {
            await JsRuntime.InvokeVoidAsync("toggleModal", "DeleteFineTunedModelModal");
            await LoadModels();
        }

        IsDeleting = false;
    }

    protected static class Tooltips
    {
        public static string ModelId = "Optional. The ID of the model to use for this request.";
    }
}
