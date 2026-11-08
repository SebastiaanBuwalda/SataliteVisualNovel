using System.IO;
using NUnit.Framework;
using UnityEngine;

public class StoryAndPuzzleRulesTests
{
    [Test]
    public void DialogueRetainsSpeakerUntilTheNextSpeakerTag()
    {
        var entries = DialogueParser.Parse("<s> You </s><l>First</l><l>Second</l><s>Human A</s><l>Third</l>");
        Assert.That(entries.Count, Is.EqualTo(3));
        Assert.That(entries[0].Speaker, Is.EqualTo("You"));
        Assert.That(entries[1].Speaker, Is.EqualTo("You"));
        Assert.That(entries[2].Speaker, Is.EqualTo("Human A"));
        Assert.That(entries[2].Line, Is.EqualTo("Third"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  \n ")]
    public void EmptyDialogueHasNoEntries(string text)
    {
        Assert.That(DialogueParser.Parse(text), Is.Empty);
    }

    [Test]
    public void DialoguePreservesUnicodeMultilineTextAndRichText()
    {
        var entries = DialogueParser.Parse("<l> …\n<b>Who am I?</b> </l><s>Unused</s>");
        Assert.That(entries.Count, Is.EqualTo(1));
        Assert.That(entries[0].Speaker, Is.Empty);
        Assert.That(entries[0].Line, Is.EqualTo("…\n<b>Who am I?</b>"));
    }

    [Test]
    public void AllStoryAssetsContainNonemptyDialogue()
    {
        string[] stories = Directory.GetFiles(Path.Combine(Application.dataPath, "Story Scripts"), "*.txt");
        Assert.That(stories.Length, Is.EqualTo(5));
        foreach (string story in stories)
        {
            var entries = DialogueParser.Parse(File.ReadAllText(story));
            Assert.That(entries, Is.Not.Empty, story);
            foreach (DialogueEntry entry in entries)
            {
                Assert.That(entry.Line, Is.Not.Empty, story);
            }
        }
    }

    [TestCase(5, 10, 6, 30)]
    [TestCase(5, 6, 10, 21)]
    [TestCase(10, 5, 6, 21)]
    [TestCase(0, 10, 6, 0)]
    public void RomanNumeralXMultipliesOnlyInTheMiddle(int first, int middle, int last, int expected)
    {
        Assert.That(RomanNumeralRules.TryEvaluate(first, middle, last, out int actual), Is.True);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void AnEmptyRomanNumeralSlotCannotCountAsZero()
    {
        Assert.That(RomanNumeralRules.TryEvaluate(null, 10, 3, out _), Is.False);
        Assert.That(RomanNumeralRules.TryEvaluate(5, null, 3, out _), Is.False);
        Assert.That(RomanNumeralRules.TryEvaluate(5, 10, null, out _), Is.False);
    }

    [Test]
    public void AllBrailleDigitsRoundTripWithoutAmbiguity()
    {
        for (int digit = 0; digit <= 9; digit++)
        {
            string pattern = BrailleDigitPatterns.GetPattern(digit);
            Assert.That(pattern.Length, Is.EqualTo(6));
            Assert.That(BrailleDigitPatterns.GetNumber(pattern), Is.EqualTo(digit));
        }

        Assert.That(BrailleDigitPatterns.GetNumber("000000"), Is.EqualTo(-1));
        Assert.That(BrailleDigitPatterns.GetNumber("111111"), Is.EqualTo(-1));
        Assert.That(BrailleDigitPatterns.GetNumber(null), Is.EqualTo(-1));
        Assert.That(BrailleDigitPatterns.GetPattern(-1), Is.Null);
        Assert.That(BrailleDigitPatterns.GetPattern(10), Is.Null);
    }

    [TestCase(2, "101000")]
    [TestCase(3, "110000")]
    [TestCase(5, "100100")]
    [TestCase(7, "111100")]
    public void BrailleUsesTheScenesRowOrder(int digit, string expected)
    {
        Assert.That(BrailleDigitPatterns.GetPattern(digit), Is.EqualTo(expected));
    }
}
