/// <summary>A spoken or narrated line, paired with the most recent speaker tag.</summary>
public sealed class DialogueEntry
{
    public string Speaker { get; }
    public string Line { get; }

    public DialogueEntry(string speaker, string line)
    {
        Speaker = speaker;
        Line = line;
    }
}
