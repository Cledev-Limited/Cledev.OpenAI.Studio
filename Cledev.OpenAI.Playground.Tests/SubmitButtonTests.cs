using Bunit;
using Cledev.OpenAI.Playground.Shared;
using FluentAssertions;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace Cledev.OpenAI.Playground.Tests;

public class SubmitButtonTests : TestContext
{
    [Test]
    public void ShouldRenderText()
    {
        var componentParameters = new[]
        {
            ComponentParameter.CreateParameter(
                name: "Text",
                value: "Create Completion")
        };

        var component = RenderComponent<SubmitButton>(componentParameters);

        var buttonText = component.Find("button").TextContent;

        buttonText.Should().Be("Create Completion");
    }
}