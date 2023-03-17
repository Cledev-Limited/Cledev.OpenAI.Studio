using Bunit;
using Cledev.OpenAI.V1;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Cledev.OpenAI.Studio.Tests;

public abstract class ComponentTestBase : TestContext
{
    protected Mock<IOpenAIClient> OpenAIClient;

    protected ComponentTestBase()
    {
        JSInterop.SetupVoid("addTooltips");

        OpenAIClient = new Mock<IOpenAIClient>();

        Services.AddSingleton(OpenAIClient.Object);
    }
}
