using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Playground.Pages;

public class CreateImageEditPage : ImagePageBase
{
    protected CreateImageEditRequest Request { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Request = new CreateImageEditRequest
        {
            Image = new byte[1],
            ImageName = "Something",
            Prompt = string.Empty,
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

    public async Task OnInputFileForMaskChange(InputFileChangeEventArgs e)
    {
        Request.Mask = await GetFileBytes(e);
        Request.MaskName = e.File.Name;
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

        Response = await OpenAIClient.CreateImageEdit(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            Images.AddOriginalFromBytes(Request.Image);
            Images.AddRangeFromResponse(Response, ImageType.Edited);
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Image = "Required. The image to edit. Must be a valid PNG file, less than 4MB, and square. If mask is not provided, image must have transparency, which will be used as the mask.";
        public static string Mask = "Optional. An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.";
        public static string Prompt = "Required. A text description of the desired image(s). The maximum length is 1000 characters.";
    }
}
