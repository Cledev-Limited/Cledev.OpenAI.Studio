﻿using Cledev.OpenAI.V1.Contracts.Moderations;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Studio.Pages;

public class ModerationsPage : PageComponentBase
{
    protected CreateModerationRequest Request { get; set; } = null!;
    protected CreateModerationResponse? Response { get; set; }

    public IList<string> ModerationModels { get; set; } = new List<string>();

    protected override void OnInitialized()
    {
        Request = new CreateModerationRequest
        {
            Model = ModerationModel.TextModerationStable.ToStringModel(),
            Input = string.Empty
        };

        ModerationModels = Enum.GetValues(typeof(ModerationModel)).Cast<ModerationModel>().Select(x => x.ToStringModel()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;

        Response = await OpenAIClient.CreateModeration(Request);
        Error = Response?.Error;

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Input = "Required. The input text to classify.";
    }
}
