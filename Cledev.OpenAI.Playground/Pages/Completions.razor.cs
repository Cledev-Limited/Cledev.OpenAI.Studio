using Cledev.OpenAI.V1.Contracts.Completions;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Playground.Pages;

public class CompletionsPage : PageComponentBase
{
    protected CreateCompletionRequest Request { get; set; } = null!;
    protected CreateCompletionResponse? Response { get; set; }

    public IList<string> CompletionModels { get; set; } = new List<string>();

    protected override void OnInitialized()
    {
        Request = new CreateCompletionRequest
        {
            Model = CompletionsModel.TextDavinciV3.ToStringModel(),
            N = 1,
            MaxTokens = 100,
            Stream = true
        };

        CompletionModels = Enum.GetValues(typeof(CompletionsModel)).Cast<CompletionsModel>().Select(x => x.ToStringModel()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsLoading = true;
        Response = null;

        if (Request.Stream is true)
        {
            var completions = OpenAIClient.CreateCompletionAsStream(Request);

            await foreach (var completion in completions)
            {
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

        IsLoading = false;
    }

    protected static class Tooltips
    {
        public static string Prompt = "Optional (Defaults to <|endoftext|>). The prompt(s) to generate completions for, encoded as a string, array of strings, array of tokens, or array of token arrays. Note that <|endoftext|> is the document separator that the model sees during training, so if a prompt is not specified the model will generate as if from the beginning of a new document.";
        public static string Model = "Required. ID of the model to use. You can use the List models API to see all of your available models, or see our Model overview for descriptions of them.";
        public static string Suffix = "Optional (Defaults to null). The suffix that comes after a completion of inserted text.";
        public static string MaxTokens = "Optional (Defaults to 16). The maximum number of tokens to generate in the completion. The token count of your prompt plus max_tokens cannot exceed the model's context length. Most models have a context length of 2048 tokens (except for the newest models, which support 4096).";
        public static string Temperature = "Optional (Defaults to 1). What sampling temperature to use. Higher values means the model will take more risks. Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer. We generally recommend altering this or top_p but not both.";
        public static string TopP = "Optional (Defaults to 1). An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both.";
        public static string N = "Optional (Defaults to 1). How many completions to generate for each prompt. Note: Because this parameter generates many completions, it can quickly consume your token quota. Use carefully and ensure that you have reasonable settings for max_tokens and stop.";
        public static string Stream = "Optional (Defaults to false). Whether to stream back partial progress. If set, tokens will be sent as data-only server-sent events as they become available, with the stream terminated by a data: [DONE] message.";
        public static string LogProbs = "Optional (Defaults to null). Include the log probabilities on the logprobs most likely tokens, as well the chosen tokens. For example, if logprobs is 5, the API will return a list of the 5 most likely tokens. The API will always return the logprob of the sampled token, so there may be up to logprobs+1 elements in the response. The maximum value for logprobs is 5. If you need more than this, please contact us through our Help center and describe your use case.";
        public static string Echo = "Optional (Defaults to false). Echo back the prompt in addition to the completion.";
        public static string Stop = "Optional (Defaults to null). Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.";
        public static string PresencePenalty = "Optional (Defaults to 0). Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.";
        public static string FrequencyPenalty = "Optional (Defaults to 0). Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.";
        public static string BestOf = "Optional (Defaults to 1). Generates best_of completions server-side and returns the best (the one with the highest log probability per token). Results cannot be streamed. When used with n, best_of controls the number of candidate completions and n specifies how many to return – best_of must be greater than n. Note: Because this parameter generates many completions, it can quickly consume your token quota. Use carefully and ensure that you have reasonable settings for max_tokens and stop.";
        public static string LogitBias = "Optional (Defaults to null). Modify the likelihood of specified tokens appearing in the completion. Accepts a json object that maps tokens (specified by their token ID in the GPT tokenizer) to an associated bias value from -100 to 100. You can use this tokenizer tool (which works for both GPT-2 and GPT-3) to convert text to token IDs. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token. As an example, you can pass {'50256': -100} to prevent the <|endoftext|> token from being generated.";
        public static string User = "Optional. A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.";
    }
}
