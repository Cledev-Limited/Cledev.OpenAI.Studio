﻿using Cledev.OpenAI.V1.Contracts.Embeddings;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Studio.Pages;

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
        IsProcessing = true;
        Response = null;

        Response = await OpenAIClient.CreateEmbeddings(Request);
        Error = Response?.Error;

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Input = "Required. Input text to get embeddings for, encoded as a string or array of tokens. To get embeddings for multiple inputs in a single request, pass an array of strings or array of token arrays. Each input must not exceed 8192 tokens in length.";
    }
}
