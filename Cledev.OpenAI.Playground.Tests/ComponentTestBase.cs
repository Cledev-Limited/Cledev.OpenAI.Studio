using Bunit;

namespace Cledev.OpenAI.Playground.Tests;

public abstract class ComponentTestBase : TestContext
{
    protected ComponentTestBase()
    {
        JSInterop.SetupVoid("addTooltips");
    }
}