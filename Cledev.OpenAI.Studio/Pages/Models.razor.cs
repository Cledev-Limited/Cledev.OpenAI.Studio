using Cledev.OpenAI.V1.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Cledev.OpenAI.Studio.Pages;

public class ModelsPage : PageComponentBase
{
    protected string? ModelId { get; set; }
    protected bool SearchCompleted { get; set; }

    public List<ModelResponse> Models { get; set; } = new();

    protected void OnValueChanged(ChangeEventArgs e)
    {
        ModelId = e.Value?.ToString();
    }

    protected async Task OnSubmitAsync()
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

    protected static class Tooltips
    {
        public static string ModelId = "Optional. The ID of the model to use for this request.";
    }
}
