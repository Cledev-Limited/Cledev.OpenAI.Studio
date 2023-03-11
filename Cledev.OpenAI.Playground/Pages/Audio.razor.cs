using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Audio;
using Cledev.OpenAI.V1.Helpers;
using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Playground.Pages;

public class AudioPage : PageComponentBase
{
    protected CreateAudioTranscriptionRequest Request { get; set; } = null!;
    protected CreateAudioResponse? Response { get; set; }

    public IList<string> AudioModels { get; set; } = new List<string>();
    public IList<string> ResponseFormats { get; set; } = new List<string>();

    protected override void OnInitialized()
    {
        Request = new CreateAudioTranscriptionRequest
        {
            Model = AudioModel.Whisper1.ToStringModel(),
            ResponseFormat = AudioResponseFormat.VerboseJson.ToStringFormat(),
            Language = "en"
        };

        AudioModels = Enum.GetValues(typeof(AudioModel)).Cast<AudioModel>().Select(x => x.ToStringModel()).ToList();
        ResponseFormats = Enum.GetValues(typeof(AudioResponseFormat)).Cast<AudioResponseFormat>().Select(x => x.ToStringFormat()).ToList();
    }

    public async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        Request.File = await GetFileBytes(e);
        Request.FileName = e.File.Name;
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

        Response = await OpenAIClient.CreateAudioTranscription(Request);
        Error = Response?.Error;

        IsProcessing = false;
    }

    protected static class Tooltips
    {
        public static string Prompt = "Optional. An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.";
        public static string File = "Required. The audio file to transcribe, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.";
    }
}
