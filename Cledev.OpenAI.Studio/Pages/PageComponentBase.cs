using Cledev.OpenAI.V1;
using Cledev.OpenAI.V1.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Cledev.OpenAI.Studio.Pages;

public abstract class PageComponentBase : ComponentBase
{
    [Inject] public IOpenAIClient OpenAIClient { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public IOptions<StudioSettings> StudioSettings { get; set; } = null!;

    protected bool IsProcessing { get; set; }

    protected Error? Error { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("addTooltips");
        }
    }

    protected async Task<byte[]> GetFileBytes(InputFileChangeEventArgs e)
    {
        using var memoryStream = new MemoryStream();

        try
        {
            await e.File.OpenReadStream(maxAllowedSize: 4000000).CopyToAsync(memoryStream);
        }
        catch (Exception exception)
        {
            Error = new Error
            {
                Message = exception.Message
            };
        }

        return memoryStream.ToArray();
    }

    protected void FakeClick()
    {
        // For Bunit tests
    }
}
