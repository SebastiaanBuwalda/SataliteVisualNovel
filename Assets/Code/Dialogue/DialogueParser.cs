using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>Reads speaker (&lt;s&gt;) and line (&lt;l&gt;) tags from story text.</summary>
public static class DialogueParser
{
    private static readonly Regex TagPattern = new Regex(
        @"<(?<tag>[sl])>(?<content>.*?)</\k<tag>>",
        RegexOptions.Singleline | RegexOptions.CultureInvariant);

    public static List<DialogueEntry> Parse(string rawText)
    {
        var entries = new List<DialogueEntry>();
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return entries;
        }

        string currentSpeaker = string.Empty;
        foreach (Match match in TagPattern.Matches(rawText))
        {
            string content = match.Groups["content"].Value.Trim();
            if (match.Groups["tag"].Value == "s")
            {
                currentSpeaker = content;
            }
            else
            {
                entries.Add(new DialogueEntry(currentSpeaker, content));
            }
        }

        return entries;
    }
}
