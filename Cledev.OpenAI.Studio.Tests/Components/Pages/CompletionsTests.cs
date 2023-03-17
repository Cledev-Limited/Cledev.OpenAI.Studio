using Bunit;
using Cledev.OpenAI.Studio.Pages;
using Cledev.OpenAI.V1.Contracts.Completions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Error = Cledev.OpenAI.V1.Contracts.Error;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages;

public class CompletionsTests : ComponentTestBase
{
    [Test]
    public void GivenAPIReturnsError_ThenErrorMessageIsRendered()
    {
        OpenAIClient
            .Setup(x => x.CreateCompletion(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .ReturnsAsync(new CreateCompletionResponse { Error = new Error { Message = "Some Error Message" } });

        var cut = RenderComponent<Completions>();

        cut.Find("input[id=Stream]").Change(false);
        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("span[id=errorMessage]") is { TextContent: not null });

        cut.Find("span[id=errorMessage]").TextContent.Should().Contain("Some Error Message");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenCompletionIsRendered()
    {
        OpenAIClient
            .Setup(x => x.CreateCompletion(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .ReturnsAsync(new CreateCompletionResponse { Choices = new List<CompletionChoice> { new() { Text = "Completion Text" } }});

        var cut = RenderComponent<Completions>();

        cut.Find("input[id=Stream]").Change(false);
        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("div[id=choice1Text]") is { TextContent: not null });

        cut.Find("div[id=choice1Text]").TextContent.Should().Contain("Completion Text");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenErrorMessageIsNotRendered()
    {
        OpenAIClient
            .Setup(x => x.CreateCompletion(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .ReturnsAsync(new CreateCompletionResponse { Choices = new List<CompletionChoice> { new() { Text = "Completion Text" } } });

        var cut = RenderComponent<Completions>();

        cut.Find("input[id=Stream]").Change(false);
        cut.Find("button").Click();

        cut.WaitForState(() => cut.Find("div[id=choice1Text]") is { TextContent: not null });

        Action act = () => cut.Find("span[id=errorMessage]");
        act.Should().Throw<ElementNotFoundException>();
    }
}