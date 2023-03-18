using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using Moq;
using NUnit.Framework;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.FineTuning;

public class RefreshFineTunesTests : ComponentTestBase
{
    [Test]
    public void GivenRefreshButtonIsCLicked_ThenAPIIsCalledToGetNewList()
    {
        OpenAIClient
            .Setup(x => x.ListFineTunes(CancellationToken.None))
            .ReturnsAsync(new ListFineTunesResponse { Data = new List<FineTuneResponse> { new() { Id = "Id1", Model = "BaseModel", FineTunedModel = "FineTunedModel" } } });

        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.WaitForState(() => cut.Find("td[id=fineTune1Id]") is { TextContent: not null });

        cut.Find("button[id=RefreshList]").Click();

        OpenAIClient.Verify(client => client.ListFineTunes(CancellationToken.None), Times.Exactly(2));
    }
}