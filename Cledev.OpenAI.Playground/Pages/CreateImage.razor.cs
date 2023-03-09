﻿using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Playground.Pages;

public class CreateImagePage : ImagePageBase
{
    protected CreateImageRequest Request { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Request = new CreateImageRequest
        {
            Prompt = string.Empty,
            Size = ImageSize.Size512x512.ToStringSize(),
            ResponseFormat = ImageResponseFormat.B64Json.ToStringFormat(),
            N = 1
        };
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;
        Images.Clear();

        Response = await OpenAIClient.CreateImage(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            foreach (var image in Response.Data)
            {
                if (string.IsNullOrEmpty(image.Url) is false)
                {
                    Images.Add(image.Url);
                }
                else if (string.IsNullOrEmpty(image.B64Json) is false)
                {
                    var imagePath = Base64ToImage(image.B64Json);
                    Images.Add(imagePath);
                }
            }
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Prompt = "Required. A text description of the desired image(s). The maximum length is 1000 characters.";
    }
}
