using Bunit;
using Cledev.OpenAI.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Cledev.OpenAI.Studio.Tests;

public abstract class ComponentTestBase : TestContext
{
    protected Mock<IOpenAIClient> OpenAIClient;
    protected Mock<IOptions<StudioSettings>> StudioSettings;

    protected ComponentTestBase()
    {
        JSInterop.SetupVoid("addTooltips");

        OpenAIClient = new Mock<IOpenAIClient>();

        StudioSettings = new Mock<IOptions<StudioSettings>>();
        StudioSettings
            .Setup(options => options.Value)
            .Returns(new StudioSettings
            {
                OrganizationName = "my-org"
            });

        Services.AddSingleton(OpenAIClient.Object);
        Services.AddSingleton(StudioSettings.Object);
    }
}
