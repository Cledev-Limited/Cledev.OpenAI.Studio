using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Error = Cledev.OpenAI.V1.Contracts.Error;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.FineTuning;

public class FineTunesTests : ComponentTestBase
{
    [Test]
    public void GivenAPIReturnsError_ThenErrorMessageIsRendered()
    {
        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Error = new Error { Message = "Some Error Message" } });

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.WaitForState(() => cut.Find("span[id=errorMessage]") is { TextContent: not null });

        cut.Find("span[id=errorMessage]").TextContent.Should().Contain("Some Error Message");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenFineTuneIsRendered()
    {
        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Data = new List<FineTuneResponse> { new() { Id = "Id1", Model = "BaseModel", FineTunedModel = "FineTunedModel" } } });

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.WaitForState(() => cut.Find("td[id=fineTune1Id]") is { TextContent: not null });

        cut.Find("td[id=fineTune1Id]").TextContent.Should().Be("Id1");
    }

    [Test]
    public void GivenAPIReturnsResult_ThenErrorMessageIsNotRendered()
    {
        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Data = new List<FineTuneResponse> { new() { Id = "Id1", Model = "BaseModel", FineTunedModel = "FineTunedModel" } } });

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.WaitForState(() => cut.Find("td[id=fineTune1Id]") is { TextContent: not null });

        Action act = () => cut.Find("span[id=errorMessage]");
        act.Should().Throw<ElementNotFoundException>();
    }
}