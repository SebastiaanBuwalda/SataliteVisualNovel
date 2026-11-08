#!/usr/bin/env python3
"""Check Unity scene wiring without launching the editor or installing packages."""

from pathlib import Path
import re
import unittest

ROOT = Path(__file__).resolve().parents[1]
SCENE = ROOT / "Assets/Scenes/OnlyScene.unity"


def field(text, name):
    match = re.search(rf"^  {re.escape(name)}: (.*)$", text, re.MULTILINE)
    if match is None:
        raise AssertionError(f"Missing field: {name}")
    return match.group(1)


def local_id(value):
    return re.search(r"fileID: (-?\d+)", value).group(1)


class SceneWiringTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.scene = SCENE.read_text()
        cls.blocks = dict(re.findall(
            r"^--- !u!\d+ &(-?\d+)\n(.*?)(?=^--- |\Z)",
            cls.scene, re.MULTILINE | re.DOTALL))
        cls.assets = {}
        for metadata in (ROOT / "Assets").rglob("*.meta"):
            match = re.search(r"^guid: (\w+)", metadata.read_text(), re.MULTILINE)
            if match:
                cls.assets[match.group(1)] = metadata.with_suffix("")

    def components(self, name):
        return [block for block in self.blocks.values()
                if f"  m_EditorClassIdentifier: OrbitalGenesis.Runtime::{name}\n" in block]

    def test_local_scene_references_resolve(self):
        for reference in re.findall(r"\{fileID: -?\d+[^}]*\}", self.scene):
            reference_id = local_id(reference)
            if reference_id != "0" and "guid:" not in reference:
                self.assertIn(reference_id, self.blocks, reference)

    def test_custom_component_types_match_their_script_assets(self):
        count = 0
        for block in self.blocks.values():
            if "  m_EditorClassIdentifier: OrbitalGenesis.Runtime::" not in block:
                continue
            guid = re.search(r"m_Script: .*guid: (\w+)", block).group(1)
            script = self.assets[guid]
            self.assertTrue(script.exists(), str(script))
            self.assertEqual(field(block, "m_EditorClassIdentifier"),
                             f"OrbitalGenesis.Runtime::{script.stem}")
            self.assertRegex(script.read_text(), rf"\bclass {script.stem}\b")
            count += 1
        self.assertGreater(count, 0)

    def test_all_five_story_assets_are_connected(self):
        connected = set()
        for block in self.components("VisualNovelDialogSystem"):
            guid = re.search(r"guid: (\w+)", field(block, "dialogTextAsset")).group(1)
            story = self.assets[guid]
            self.assertTrue(story.exists(), str(story))
            connected.add(story)
        self.assertEqual(connected, set((ROOT / "Assets/Story Scripts").glob("*.txt")))

    def test_third_story_opens_the_braille_puzzle(self):
        braille = self.components("BraillePuzzleManager")[0]
        target = field(braille, "m_GameObject")
        for block in self.components("VisualNovelDialogSystem"):
            guid = re.search(r"guid: (\w+)", field(block, "dialogTextAsset")).group(1)
            if self.assets[guid].name == "scene0script003.txt":
                self.assertIn(f"objectsThatEnableAfterDialogue:\n  - {target}", block)
                break
        else:
            self.fail("Third story controller was not found")

    def test_reset_button_targets_the_braille_manager(self):
        reset, = self.components("BrailleResetButton")
        manager_id = local_id(field(reset, "puzzleManager"))
        self.assertIn(self.blocks[manager_id], self.components("BraillePuzzleManager"))
        self.assertNotIn("objectsToEnable:", reset)

    def test_braille_solution_is_reachable_within_the_scene_move_budget(self):
        manager, = self.components("BraillePuzzleManager")
        patterns = re.findall(r'"([01]{6})"',
                             (ROOT / "Assets/Code/Puzzles/Braille/BrailleDigitPatterns.cs").read_text())
        self.assertEqual(len(patterns), 10)
        initial = []
        for number_field in ("firstNumber", "secondNumber"):
            number = self.blocks[local_id(field(manager, number_field))]
            dots = re.findall(r"^  - \{fileID: (\d+)\}", number, re.MULTILINE)
            self.assertEqual(len(dots), 6)
            initial.append("".join(field(self.blocks[dot], "isOn") for dot in dots))
        target = int(field(manager, "correctAnswer"))
        move_budget = int(field(manager, "maxMoves"))
        distances = [sum(a != b for a, b in zip(initial[0] + initial[1], left + right))
                     for first, left in enumerate(patterns)
                     for second, right in enumerate(patterns)
                     if first * second == target]
        self.assertTrue(distances, "Target has no pair of Braille digits")
        self.assertLessEqual(min(distances), move_budget)


if __name__ == "__main__":
    unittest.main(verbosity=2)
