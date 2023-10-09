using System.Text.RegularExpressions;

namespace Blog.PublicAPI.Extensions;

public static partial class StringExtensions
{
    private static readonly Regex PeplaceSpecialWordsRegex = PeplaceSpecialWordsRegexAttr();
    private static readonly Regex RemoveMultipleSpacesRegex = RemoveMultipleSpacesRegexAttr();

    public static string UrlFriendly(this string title)
    {
        var titlePeplaceSpecialWords = PeplaceSpecialWordsRegex.Replace(title, " ").Trim();
        var removeMutipleSpaces = RemoveMultipleSpacesRegex.Replace(titlePeplaceSpecialWords, " ");
        var replaceDashes = removeMutipleSpaces.Replace(" ", "-");
        var duplicateDashesRemove = replaceDashes.Replace("--", "-");
        return duplicateDashesRemove.ToLowerInvariant();
    }

    [GeneratedRegex("&quot;|['\",&?%\\.!()@$^_+=*:#/\\\\-]", RegexOptions.Compiled)]
    private static partial Regex PeplaceSpecialWordsRegexAttr();

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex RemoveMultipleSpacesRegexAttr();
}