using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using Moq;
using NUnit.Framework;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.FineTuning;

public class CancelFineTuneTests : ComponentTestBase
{
    [Test]
    public void GivenFineTuneJobIsCancelled_WhenAPIReturnsAValidResponse_ThenDeleteModalIsClosed()
    {
        OpenAIClient
            .Setup(x => x.CancelFineTune("fine-tune-model-1", CancellationToken.None))
            .ReturnsAsync(new FineTuneResponse{ Id = "Id1" });

        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Data = new List<FineTuneResponse> { new() { Id = "Id1", Model = "BaseModel", Status = "pending" } } });

        JSInterop.SetupVoid("toggleModal", "CancelFineTuneJobModal");

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.Find("a[id=OpenCancelFineTuneJobModal]").Click();

        cut.WaitForState(() => cut.Find("div[id=CancelFineTuneJobModal]") is { TextContent: not null });

        cut.Find("button[id=CancelFineTuneJob]").Click();

        cut.WaitForState(() => true, TimeSpan.FromSeconds(5));

        cut.Find("div[id=CancelFineTuneJobModal]").ClassList.Contains("hide");
        OpenAIClient.Verify(client => client.CancelFineTune("Id1", CancellationToken.None), Times.Once);
    }
}