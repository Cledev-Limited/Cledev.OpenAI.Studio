using Bunit;
using Cledev.OpenAI.Playground.Pages;
using Cledev.OpenAI.V1;
using Cledev.OpenAI.V1.Contracts.Completions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Error = Cledev.OpenAI.V1.Contracts.Error;

namespace Cledev.OpenAI.Playground.Tests.Components.Pages;

public class CompletionsComponentTests : ComponentTestBase
{
    [Test]
    public void ShouldRenderError_WhenAPIReturnsAnError()
    {
        var openAIClient = new Mock<IOpenAIClient>();
        openAIClient
            .Setup(x => x.CreateCompletion(It.IsAny<CreateCompletionRequest>(), CancellationToken.None))
            .ReturnsAsync(new CreateCompletionResponse { Error = new Error { Message = "Some Error Message" } });

        Services.AddSingleton(openAIClient.Object);

        var component = RenderComponent<Completions>();

        component.Find("input[id=Stream]").Change(false);

        var submitButton = component.Find("button");
        submitButton.Click();

        component.WaitForState(() => component.Find("span[id=errorMessage]") is { TextContent: not null });

        var errorMessage = component.Find("span[id=errorMessage]");
        errorMessage.TextContent.Should().Contain("Some Error Message");
    }
}