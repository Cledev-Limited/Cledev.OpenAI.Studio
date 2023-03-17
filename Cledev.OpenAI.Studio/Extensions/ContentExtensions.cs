using System.Text.RegularExpressions;

namespace Cledev.OpenAI.Studio.Extensions;

public static class ContentExtensions
{
    private static readonly string OpenTag = @"<pre>";
    private static readonly string CloseTag = @"</pre>";
    private static readonly string Value = @"```";

    public static string? FormatCode(this string? content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }

        var numberOfValues = Regex.Matches(content, Value).Count;
        if (numberOfValues <= 0)
        {
            return content;
        }

        for (var i = 0; i < numberOfValues; i++)
        {
            var numberOfOpenTags = Regex.Matches(content, OpenTag).Count;
            var numberOfCloseTags = Regex.Matches(content, CloseTag).Count;

            var replacement = numberOfOpenTags > numberOfCloseTags ? CloseTag : OpenTag;

            content = new Regex(Value).Replace(content, replacement, 1);
        }

        return content;
    }

    public static bool ContainsCode(this string? content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return false;
        }

        var numberOfValues = Regex.Matches(content, Value).Count;
        return numberOfValues > 0;
    }
}
