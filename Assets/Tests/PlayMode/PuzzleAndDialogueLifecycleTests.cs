using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

public class PuzzleAndDialogueLifecycleTests
{
    private readonly List<GameObject> createdObjects = new List<GameObject>();

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (GameObject sceneObject in createdObjects)
        {
            if (sceneObject != null)
            {
                Object.Destroy(sceneObject);
            }
        }

        createdObjects.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator BrailleCanSolveOnTheFinalMoveWithoutAnswerTextAndLocksAfterCompletion()
    {
        BrailleNumber first = CreateNumber(1);
        BrailleNumber second = CreateNumber(9);
        BraillePuzzleManager manager = CreateManager(first, second);
        int completionCount = 0;
        var onCompleted = new UnityEvent();
        onCompleted.AddListener(() => completionCount++);
        SetField(manager, "onCompleted", onCompleted);
        manager.gameObject.SetActive(true);
        yield return null;

        GetDots(first)[3].Toggle(); // 1 -> 5
        GetDots(second)[0].Toggle(); // 9 -> 6
        GetDots(second)[3].Toggle(); // 6 -> 7, on move three

        Assert.That(manager.IsPuzzleCompleted(), Is.True);
        Assert.That(manager.GetRemainingMoves(), Is.Zero);
        Assert.That(completionCount, Is.EqualTo(1));
        GetDots(first)[0].Toggle();
        Assert.That(first.GetCurrentNumber(), Is.EqualTo(5));
        Assert.That(completionCount, Is.EqualTo(1));

        manager.ResetPuzzle();
        Assert.That(manager.IsPuzzleCompleted(), Is.False);
        Assert.That(manager.GetRemainingMoves(), Is.EqualTo(3));
        Assert.That(first.GetCurrentNumber(), Is.EqualTo(1));
        Assert.That(second.GetCurrentNumber(), Is.EqualTo(9));
    }

    [UnityTest]
    public IEnumerator BrailleResetsAtZeroMovesAndDoesNotDuplicateSubscriptionsOnReenable()
    {
        BrailleNumber first = CreateNumber(1);
        BrailleNumber second = CreateNumber(9);
        BraillePuzzleManager manager = CreateManager(first, second);
        manager.gameObject.SetActive(true);
        yield return null;

        GetDots(first)[5].Toggle();
        GetDots(first)[4].Toggle();
        GetDots(first)[2].Toggle();
        Assert.That(manager.GetRemainingMoves(), Is.EqualTo(3));
        Assert.That(first.GetCurrentNumber(), Is.EqualTo(1));

        manager.gameObject.SetActive(false);
        manager.gameObject.SetActive(true);
        GetDots(first)[3].Toggle();
        Assert.That(manager.GetRemainingMoves(), Is.EqualTo(2));

        // Programmatic changes refresh the number without consuming player moves.
        first.SetNumber(8);
        Assert.That(first.GetCurrentNumber(), Is.EqualTo(8));
        Assert.That(manager.GetRemainingMoves(), Is.EqualTo(2));
    }

    [UnityTest]
    public IEnumerator DialogueCompletionIsIdempotentAndOptionalIndicatorCanBeMissing()
    {
        GameObject root = CreateObject("Dialogue");
        root.SetActive(false);
        var dialogue = root.AddComponent<VisualNovelDialogSystem>();
        SetField(dialogue, "speakerTMP", CreateText("Speaker"));
        SetField(dialogue, "lineTMP", CreateText("Line"));
        SetField(dialogue, "useTypewriter", false);
        var asset = new TextAsset("<s>You</s><l>Only line</l>");
        SetField(dialogue, "dialogTextAsset", asset);
        GameObject nextSection = CreateObject("Next section");
        nextSection.SetActive(false);
        SetField(dialogue, "objectsThatEnableAfterDialogue", new[] { nextSection });
        root.SetActive(true);
        yield return null;

        dialogue.Advance();
        Assert.That(nextSection.activeSelf, Is.True);
        nextSection.SetActive(false);
        dialogue.Advance();
        dialogue.Advance();
        Assert.That(nextSection.activeSelf, Is.False);
        Object.Destroy(asset);
    }

    [UnityTest]
    public IEnumerator ReloadingDialogueCancelsThePreviousTypewriter()
    {
        GameObject root = CreateObject("Typewriter dialogue");
        root.SetActive(false);
        var dialogue = root.AddComponent<VisualNovelDialogSystem>();
        TextMeshProUGUI line = CreateText("Line");
        SetField(dialogue, "speakerTMP", CreateText("Speaker"));
        SetField(dialogue, "lineTMP", line);
        SetField(dialogue, "typeSpeed", 0.01f);
        var first = new TextAsset("<l>A very long previous line that must not continue typing</l>");
        var replacement = new TextAsset("<l><b>New</b>…</l>");
        SetField(dialogue, "dialogTextAsset", first);
        root.SetActive(true);
        yield return null;

        dialogue.LoadFromTextAsset(replacement);
        Assert.That(line.textInfo.characterCount, Is.EqualTo(4));
        Assert.That(line.maxVisibleCharacters, Is.LessThan(line.textInfo.characterCount));
        dialogue.Advance(); // Reveal the new line immediately.
        yield return new WaitForSeconds(0.1f);
        Assert.That(line.text, Is.EqualTo("<b>New</b>…"));
        Assert.That(line.maxVisibleCharacters, Is.EqualTo(int.MaxValue));
        Object.Destroy(first);
        Object.Destroy(replacement);
    }

    [UnityTest]
    public IEnumerator DepartingTileCannotClearItsReplacement()
    {
        GameObject managerObject = CreateObject("Roman numeral manager");
        var manager = managerObject.AddComponent<RomanNumeralPuzzleManager>();
        var oldTile = CreateObject("Old tile").AddComponent<RomanNumeralSlotReporter>();
        var newTile = CreateObject("New tile").AddComponent<RomanNumeralSlotReporter>();
        manager.SetSlotOccupant(RomanNumeralSlotReporter.SlotType.A, oldTile, 3);
        manager.SetSlotOccupant(RomanNumeralSlotReporter.SlotType.A, newTile, 5);
        manager.ClearSlotOccupant(RomanNumeralSlotReporter.SlotType.A, oldTile);
        var values = (int?[])GetField(manager, "slotValues");
        Assert.That(values[0], Is.EqualTo(5));
        manager.ClearSlotOccupant(RomanNumeralSlotReporter.SlotType.A, newTile);
        Assert.That(values[0], Is.Null);
        yield return null;
    }

    private BrailleNumber CreateNumber(int initialNumber)
    {
        GameObject root = CreateObject("Braille number");
        root.SetActive(false);
        var number = root.AddComponent<BrailleNumber>();
        var dots = new BrailleDot[6];
        string pattern = BrailleDigitPatterns.GetPattern(initialNumber);
        for (int index = 0; index < dots.Length; index++)
        {
            var dotObject = new GameObject($"Dot {index}");
            dotObject.transform.SetParent(root.transform);
            dotObject.AddComponent<BoxCollider2D>();
            dots[index] = dotObject.AddComponent<BrailleDot>();
            SetField(dots[index], "dotIndex", index);
            SetField(dots[index], "isOn", pattern[index] == '1');
        }

        SetField(number, "brailleDots", dots);
        root.SetActive(true);
        return number;
    }

    private BraillePuzzleManager CreateManager(BrailleNumber first, BrailleNumber second)
    {
        GameObject root = CreateObject("Braille manager");
        root.SetActive(false);
        var manager = root.AddComponent<BraillePuzzleManager>();
        SetField(manager, "firstNumber", first);
        SetField(manager, "secondNumber", second);
        SetField(manager, "maxMoves", 3);
        return manager;
    }

    private TextMeshProUGUI CreateText(string name)
    {
        GameObject canvasObject = CreateObject(name + " canvas");
        canvasObject.AddComponent<Canvas>();
        var textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(canvasObject.transform, false);
        var text = textObject.AddComponent<TextMeshProUGUI>();
        text.rectTransform.sizeDelta = new Vector2(800f, 200f);
        return text;
    }

    private GameObject CreateObject(string name)
    {
        var sceneObject = new GameObject(name);
        createdObjects.Add(sceneObject);
        return sceneObject;
    }

    private static BrailleDot[] GetDots(BrailleNumber number) => (BrailleDot[])GetField(number, "brailleDots");

    private static object GetField(object target, string name) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);

    private static void SetField(object target, string name, object value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
}
