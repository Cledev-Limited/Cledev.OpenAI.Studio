using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.FineTuning;

public class DeleteFineTuneTests : ComponentTestBase
{
    [Test]
    public void GivenDeleteFineTuneButtonClicked_WhenAPIReturnsAValidResponse_ThenModalIsClosedAndAPIIsCalledToGetNewList()
    {
        OpenAIClient
            .Setup(x => x.DeleteFineTune("fine-tune-model-1", CancellationToken.None))
            .ReturnsAsync(new DeleteFineTuneResponse { Id = "Id1" });

        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Data = new List<FineTuneResponse> { new() { Id = "Id1", Model = "BaseModel", FineTunedModel = "fine-tune-model-1" } } });

        JSInterop.SetupVoid("toggleModal", "DeleteFineTuneModal");

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.Find("a[id=OpenDeleteFineTuneModal]").Click();

        cut.WaitForState(() => cut.Find("div[id=DeleteFineTuneModal]") is { TextContent: not null });

        cut.Find("button[id=DeleteFineTune]").Click();

        cut.WaitForState(() => true, TimeSpan.FromSeconds(5));

        cut.Find("div[id=DeleteFineTuneModal]").ClassList.Contains("hide");
        OpenAIClient.Verify(client => client.DeleteFineTune("fine-tune-model-1", CancellationToken.None), Times.Once);
    }
}