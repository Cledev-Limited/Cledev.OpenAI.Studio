using Cledev.OpenAI.V1.Contracts.Images;
using Cledev.OpenAI.V1.Helpers;

namespace Cledev.OpenAI.Playground.Pages;

public abstract class ImagePageBase : PageComponentBase
{
    protected CreateImageResponse? Response { get; set; }

    public IList<string> Sizes { get; set; } = new List<string>();
    public IList<string> Formats { get; set; } = new List<string>();

    public IList<Image> Images { get; set; } = new List<Image>();

    protected override void OnInitialized()
    {
        Sizes = Enum.GetValues(typeof(ImageSize)).Cast<ImageSize>().Select(x => x.ToStringSize()).ToList();
        Formats = Enum.GetValues(typeof(ImageResponseFormat)).Cast<ImageResponseFormat>().Select(x => x.ToStringFormat()).ToList();
    }
}

public class Image
{
    public Image(string type, string path)
    {
        Type = type;
        Path = path;
    }

    public string Type { get; set; }
    public string Path { get; set; }
}

public static class ImageType
{
    public static string Created = "Created";
    public static string Original = "Original";
    public static string Edited = "Edited";
    public static string Variation = "Variation";
}

public static class ImagesExtensions
{
    public static void AddOriginalFromBytes(this IList<Image> images, byte[] bytes)
    {
        images.Add(new Image(ImageType.Original, bytes.ToImagePath()));
    }

    public static void AddRangeFromResponse(this IList<Image> images, CreateImageResponse response, string imageType)
    {
        foreach (var image in response.Data)
        {
            if (string.IsNullOrEmpty(image.Url) is false)
            {
                images.Add(new Image(imageType, image.Url));
            }
            else if (string.IsNullOrEmpty(image.B64Json) is false)
            {
                var imagePath = image.B64Json.ToImagePath();
                images.Add(new Image(imageType, imagePath));
            }
        }
    }

    public static string ToImagePath(this byte[] bytes)
    {
        var imageName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var imagePath = $"images/{imageName}.png";
        using var imageFile = new FileStream($"wwwroot/{imagePath}", FileMode.Create);

        imageFile.Write(bytes, 0, bytes.Length);
        imageFile.Flush();

        return imagePath;
    }

    private static string ToImagePath(this string base64String)
    {
        var imageName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var imagePath = $"images/{imageName}.png";
        using var imageFile = new FileStream($"wwwroot/{imagePath}", FileMode.Create);

        var bytes = Convert.FromBase64String(base64String);
        imageFile.Write(bytes, 0, bytes.Length);
        imageFile.Flush();

        return imagePath;
    }
}
