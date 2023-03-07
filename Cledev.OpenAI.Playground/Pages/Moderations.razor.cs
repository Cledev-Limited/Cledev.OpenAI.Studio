using Cledev.OpenAI.V1.Contracts.Moderations;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Playground.Pages;

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
        IsLoading = true;

        Response = null;
        Error = null;

        Response = await OpenAIClient.CreateModeration(Request);
        Error = Response?.Error;

        IsLoading = false;
    }

    protected static class Tooltips
    {
        public static string Input = "Required. The input text to classify.";
        public static string Model = "Optional (Defaults to text-moderation-latest). Two content moderations models are available: text-moderation-stable and text-moderation-latest. The default is text-moderation-latest which will be automatically upgraded over time. This ensures you are always using our most accurate model. If you use text-moderation-stable, we will provide advanced notice before updating the model. Accuracy of text-moderation-stable may be slightly lower than for text-moderation-latest.";
    }
}
