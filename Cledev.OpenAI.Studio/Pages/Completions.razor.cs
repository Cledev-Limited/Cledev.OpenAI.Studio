using Cledev.OpenAI.Studio.Extensions;
using Cledev.OpenAI.V1.Contracts.Completions;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Studio.Pages;

public class CompletionsPage : PageComponentBase
{
    public CreateCompletionRequest Request { get; set; } = null!;
    protected CreateCompletionResponse? Response { get; set; }

    public List<string> CompletionModels { get; set; } = new List<string>();

    protected override async Task OnInitializedAsync()
    {
        Request = new CreateCompletionRequest
        {
            Model = CompletionsModel.TextDavinciV3.ToStringModel(),
            N = 1,
            MaxTokens = 100,
            Stream = true
        };

        CompletionModels = Enum.GetValues(typeof(CompletionsModel)).Cast<CompletionsModel>().Select(x => x.ToStringModel()).ToList();

        var listFineTunesResponse = await OpenAIClient.ListFineTunes();
        if (listFineTunesResponse is not null && listFineTunesResponse.Error is null)
        {
            CompletionModels.AddRange(listFineTunesResponse.Data.ToActiveFineTuneModels());
        }
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;
        Response = null;

        if (Request.Stream is true)
        {
            var completions = OpenAIClient.CreateCompletionAsStream(Request);

            await foreach (var completion in completions)
            {
                Error = completion.Error;

                if (Error is not null)
                {
                    break;
                }

                if (Response is null)
                {
                    Response = completion;
                }
                else
                {
                    Response.Choices[0].Text += completion.Choices[0].Text;
                }

                StateHasChanged();
            }
        }
        else
        {
            Response = await OpenAIClient.CreateCompletion(Request);
            Error = Response?.Error;
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Prompt = "Optional (Defaults to <|endoftext|>). The prompt(s) to generate completions for, encoded as a string, array of strings, array of tokens, or array of token arrays. Note that <|endoftext|> is the document separator that the model sees during training, so if a prompt is not specified the model will generate as if from the beginning of a new document.";
    }
}
