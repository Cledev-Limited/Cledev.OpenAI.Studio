using Cledev.OpenAI.V1.Contracts.Embeddings;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Playground.Pages;

public class EmbeddingsPage : PageComponentBase
{
    protected CreateEmbeddingsRequest Request { get; set; } = null!;
    protected CreateEmbeddingsResponse? Response { get; set; }

    public IList<string> EmbeddingModels { get; set; } = new List<string>();

    protected override void OnInitialized()
    {
        Request = new CreateEmbeddingsRequest
        {
            Model = EmbeddingsModel.TextEmbeddingAdaV2.ToStringModel()
        };

        EmbeddingModels = Enum.GetValues(typeof(EmbeddingsModel)).Cast<EmbeddingsModel>().Select(x => x.ToStringModel()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsLoading = true;
        Response = null;

        Response = await OpenAIClient.CreateEmbeddings(Request);
        Error = Response?.Error;

        IsLoading = false;
    }

    protected static class Tooltips
    {
        public static string Input = "Required. Input text to get embeddings for, encoded as a string or array of tokens. To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays. Each input must not exceed 8192 tokens in length.";
        public static string Model = "Required. ID of the model to use. You can use the List models API to see all of your available models, or see our Model overview for descriptions of them.";
        public static string User = "Optional. A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.";
    }
}
