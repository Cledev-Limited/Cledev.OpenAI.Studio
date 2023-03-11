using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Playground.Pages;

public class CreateImageVariationPage : ImagePageBase
{
    protected CreateImageVariationRequest Request { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Request = new CreateImageVariationRequest
        {
            Image = new byte[1],
            ImageName = "Something",
            Size = ImageSize.Size512x512.ToStringSize(),
            ResponseFormat = ImageResponseFormat.B64Json.ToStringFormat(),
            N = 1
        };
    }

    public async Task OnInputFileForImageChange(InputFileChangeEventArgs e)
    {
        Request.Image = await GetFileBytes(e);
        Request.ImageName = e.File.Name;
    }

    private async Task<byte[]> GetFileBytes(InputFileChangeEventArgs e)
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

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;
        Images.Clear();

        Response = await OpenAIClient.CreateImageVariation(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            Images.AddOriginalFromBytes(Request.Image);
            Images.AddRangeFromResponse(Response, ImageType.Variation);
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Image = "Required. The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.";
    }
}
