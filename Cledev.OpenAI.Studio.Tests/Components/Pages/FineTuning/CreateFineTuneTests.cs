using Bunit;
using Cledev.OpenAI.V1.Contracts.FineTunes;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Error = Cledev.OpenAI.V1.Contracts.Error;

namespace Cledev.OpenAI.Studio.Tests.Components.Pages.FineTuning;

public class CreateFineTuneTests : ComponentTestBase
{
    [Test]
    public void GivenCreateFineTuneButtonClicked_ThenCreateFineTuneModalIsRendered()
    {
        var cut = RenderComponent<Studio.Pages.FineTuning>();

        cut.Find("button[id=CreateFineTune]").Click();

        cut.WaitForState(() => cut.Find("div[id=CreateFineTuneModal]") is { TextContent: not null });

        cut.Find("div[id=CreateFineTuneModal]").TextContent.Should().NotBeNull();
    }
}