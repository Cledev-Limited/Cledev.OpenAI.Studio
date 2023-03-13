using Bunit;
using Cledev.OpenAI.Playground.Shared;
using FluentAssertions;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace Cledev.OpenAI.Playground.Tests;

public class SubmitButtonTests : TestContext
{
    [Test]
    public void ShouldRenderText_WhenNonProcessing()
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

    [Test]
    public void ShouldRenderProcessingText_WhenProcessing()
    {
        var componentParameters = new[]
        {
            ComponentParameter.CreateParameter(
                name: "Text",
                value: "Create Completion"),
            ComponentParameter.CreateParameter(
                name: "ProcessingText",
                value: "Creating Completion..."),
            ComponentParameter.CreateParameter(
                name: "IsProcessing",
                value: true)
        };

        var component = RenderComponent<SubmitButton>(componentParameters);

        var buttonText = component.Find("button").TextContent;

        buttonText.Should().Contain("Creating Completion...");
    }
}