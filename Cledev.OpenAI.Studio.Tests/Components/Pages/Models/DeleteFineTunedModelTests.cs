using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using Cledev.OpenAI.V1.Contracts.Models;
using Moq;
using NUnit.Framework;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.Models;

public class DeleteFineTunedModelTests : ComponentTestBase
{
    [Test]
    public void GivenFineTuneIsDeleted_WhenAPIReturnsAValidResponse_ThenDeleteModalIsClosed()
    {
        OpenAIClient
            .Setup(x => x.DeleteFineTunedModel("model-1", CancellationToken.None))
            .ReturnsAsync(new DeleteFineTunedModelResponse { Id = "model-1" });

        OpenAIClient
            .Setup(x => x.ListModels(CancellationToken.None))
            .ReturnsAsync(new ListModelsResponse { Data = new List<ModelResponse> { new() { Id = "model-1", OwnedBy = "my-org" } } });

        JSInterop.SetupVoid("toggleModal", "DeleteFineTunedModelModal");

        var cut = RenderComponent<Studio.Pages.Models>();

        cut.Find("button[id=ListModels]").Click();

        cut.WaitForState(() => cut.Find("td[id=model-model-1]") is { TextContent: not null });

        cut.Find("a[id=OpenDeleteFineTunedModelModal]").Click();

        cut.WaitForState(() => cut.Find("div[id=DeleteFineTunedModelModal]") is { TextContent: not null });

        cut.Find("button[id=DeleteFineTunedModel]").Click();

        cut.WaitForState(() => true, TimeSpan.FromSeconds(5));

        cut.Find("div[id=DeleteFineTunedModelModal]").ClassList.Contains("hide");
        OpenAIClient.Verify(client => client.DeleteFineTunedModel("model-1", CancellationToken.None), Times.Once);
    }
}