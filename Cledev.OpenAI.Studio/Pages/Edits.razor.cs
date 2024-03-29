﻿using Cledev.OpenAI.Studio.Extensions;
using Cledev.OpenAI.V1.Contracts.Edits;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Studio.Pages;

public class EditsPage : PageComponentBase
{
    protected CreateEditRequest Request { get; set; } = null!;
    protected CreateEditResponse? Response { get; set; }

    public IList<string> EditModels { get; set; } = new List<string>();

    protected override void OnInitialized()
    {
        Request = new CreateEditRequest
        {
            Model = EditsModel.CodeDavinciEditV1.ToStringModel(),
            Instruction = string.Empty,
            N = 1
        };

        EditModels = Enum.GetValues(typeof(EditsModel)).Cast<EditsModel>().Select(x => x.ToStringModel()).ToList();
    }

    protected async Task OnSubmitAsync()
    {
        IsProcessing = true;

        Response = null;
        Error = null;

        Response = await OpenAIClient.CreateEdit(Request);
        Error = Response?.Error;

        if (Response is not null)
        {
            foreach (var choice in Response.Choices)
            {
                choice.Text = choice.Text.FormatCode()!;
            }
        }

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Input = "Optional (Defaults to \"\"). The input text to use as a starting point for the edit.";
        public static string Instruction = "Required. The instruction that tells the model how to edit the prompt.";
    }
}
