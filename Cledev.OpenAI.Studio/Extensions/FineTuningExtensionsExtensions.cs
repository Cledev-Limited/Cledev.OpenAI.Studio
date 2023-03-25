using Cledev.OpenAI.Studio.Pages;
using Cledev.OpenAI.V1.Contracts.Files;
using Cledev.OpenAI.V1.Contracts.FineTunes;

namespace Cledev.OpenAI.Studio.Extensions;

public static class FineTuningExtensionsExtensions
{
    public static List<string> ToActiveFineTuneModels(this IEnumerable<FineTuneResponse> data)
    {
        return data
            .Where(fineTuneResponse =>
                string.IsNullOrEmpty(fineTuneResponse.FineTunedModel) is false &&
                fineTuneResponse.Status == "succeeded")
            .OrderBy(fineTuneResponse => fineTuneResponse.CreatedAt)
            .Select(fineTuneResponse => fineTuneResponse.FineTunedModel!)
            .ToList();
    }

    public static List<FineTuneFile> ToFineTuneFiles(this IEnumerable<FileResponse> data)
    {
        return data
            .Where(fileResponse => fileResponse.Purpose == "fine-tune")
            .OrderBy(fileResponse => fileResponse.CreatedAt)
            .Select(fileResponse => new FineTuneFile(fileResponse.Id, $"{fileResponse.FileName} ({fileResponse.Id})"))
            .ToList();
    }
}
