using Cledev.OpenAI.V1.Contracts.Chats;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.JSInterop;
using Cledev.OpenAI.Playground.Extensions;

namespace Cledev.OpenAI.Playground.Pages;

public class ChatPage : PageComponentBase
{
    protected CreateChatCompletionRequest Request { get; set; } = null!;

    public IList<string> ChatModels { get; set; } = new List<string>();

    public string? Prompt { get; set; }
    public IList<Message> Messages { get; set; } = new List<Message>();

    protected override void OnInitialized()
    {
        Request = new CreateChatCompletionRequest
        {
            Model = ChatModel.Gpt_35_Turbo.ToStringModel(),
            N = 1,
            MaxTokens = 500,
            Stream = true,
            Messages = new List<ChatCompletionMessage>()
        };

        ChatModels = Enum.GetValues(typeof(ChatModel)).Cast<ChatModel>().Select(x => x.ToStringModel()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Request.Messages.Add(new ChatCompletionMessage("user", Prompt ?? string.Empty));

        Messages.Add(new Message("User", Prompt ?? string.Empty, 0));
        Messages.Add(new Message("Assistant", string.Empty, 0));

        Prompt = null;

        if (Request.Stream is true)
        {
            var completions = OpenAIClient.CreateChatCompletionAsStream(Request);

            await foreach (var completion in completions)
            {
                Error = completion.Error;

                if (Error is not null)
                {
                    continue;
                }

                Messages.Last().Content += completion.Choices[0].Message?.Content;

                if (Messages.Last().Content.ContainsCode())
                {
                    Messages.Last().Content = Messages.Last().Content.FormatCode()!;
                }

                await InvokeAsync(StateHasChanged);

                await JsRuntime.InvokeVoidAsync("scrollToTarget", "bottom");
            }
        }
        else
        {
            var response = await OpenAIClient.CreateChatCompletion(Request);
            Error = response?.Error;

            if (Error is null)
            {
                var content = response!.Choices[0].Message?.Content;

                Messages.Last().Content += content.FormatCode();

                await InvokeAsync(StateHasChanged);

                await JsRuntime.InvokeVoidAsync("scrollToTarget", "bottom");
            }
        }

        IsProcessing = false;
    }

    protected void Reset()
    {
        Prompt = null;
        Messages.Clear();
        JsRuntime.InvokeVoidAsync("scrollToTarget", "top");
    }

    public class Message
    {
        public Message(string role, string content, int tokens)
        {
            Role = role;
            Content = content;
            Tokens = tokens;
        }

        public string Role { get; set; }
        public string Content { get; set; }
        public int Tokens { get; set; }
    }
}