using Bunit;
using Cledev.OpenAI.V1.Contracts.Completions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Error = Cledev.OpenAI.V1.Contracts.Error;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.Completions;

public class CompletionsAsStreamTests : ComponentTestBase
{
    [Test]
    public void GivenAPIReturnsError_ThenErrorMessageIsRendered()
    {
        async IAsyncEnumerable<CreateCompletionResponse> GetResponses()
        {
            await Task.CompletedTask;
            yield return new CreateCompletionResponse { Error = new Error { Message = "Some Error Message" } };
        }

        OpenAIClient
            .Setup(x => x.CreateCompletionAsStream(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .Returns(GetResponses);

        var cut = RenderComponent<Studio.Pages.Completions>();

        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("span[id=errorMessage]") is { TextContent: not null });

        cut.Find("span[id=errorMessage]").TextContent.Should().Contain("Some Error Message");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenCompletionIsRendered()
    {
        async IAsyncEnumerable<CreateCompletionResponse> GetResponses()
        {
            await Task.CompletedTask;
            yield return new CreateCompletionResponse { Choices = new List<CompletionChoice> { new() { Text = "Completion Text" } } };
        }

        OpenAIClient
            .Setup(x => x.CreateCompletionAsStream(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .Returns(GetResponses);

        var cut = RenderComponent<Studio.Pages.Completions>();

        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("div[id=choice1Text]") is { TextContent: not null });

        cut.Find("div[id=choice1Text]").TextContent.Should().Contain("Completion Text");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenErrorMessageIsNotRendered()
    {
        async IAsyncEnumerable<CreateCompletionResponse> GetResponses()
        {
            await Task.CompletedTask;
            yield return new CreateCompletionResponse { Choices = new List<CompletionChoice> { new() { Text = "Completion Text" } } };
        }

        OpenAIClient
            .Setup(x => x.CreateCompletionAsStream(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .Returns(GetResponses);

        var cut = RenderComponent<Studio.Pages.Completions>();

        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("div[id=choice1Text]") is { TextContent: not null });

        Action act = () => cut.Find("span[id=errorMessage]");
        act.Should().Throw<ElementNotFoundException>();
    }
}