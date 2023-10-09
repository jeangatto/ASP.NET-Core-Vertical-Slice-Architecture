using System;
using System.Text.RegularExpressions;

namespace Blog.PublicAPI.Extensions;

public static partial class StringExtensions
{
    private static readonly Regex ReplaceSpecialWordsRegex = ReplaceSpecialWordsRegexAttr();
    private static readonly Regex RemoveMultipleSpacesRegex = RemoveMultipleSpacesRegexAttr();

    public static string ToUrlFriendly(this string title)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));

        var titleReplaceSpecialWords = ReplaceSpecialWordsRegex.Replace(title, " ").Trim();
        var removeMutipleSpaces = RemoveMultipleSpacesRegex.Replace(titleReplaceSpecialWords, " ");
        var replaceDashes = removeMutipleSpaces.Replace(" ", "-");
        var duplicateDashesRemove = replaceDashes.Replace("--", "-");
        return duplicateDashesRemove.ToLowerInvariant();
    }

    [GeneratedRegex("&quot;|['\",`&?%\\.!()@$^_+=*:#/\\\\-]", RegexOptions.Compiled)]
    private static partial Regex ReplaceSpecialWordsRegexAttr();

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex RemoveMultipleSpacesRegexAttr();
}