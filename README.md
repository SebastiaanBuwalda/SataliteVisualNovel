# Orbital Genesis — SataliteVisualNovel

A Unity visual novel about identity, trust, and human instinct, interspersed with lateral-thinking puzzles. The project currently contains an opening chapter, one branching encounter, and three puzzles.

## Story intent and current flow

The protagonist retains language and factual knowledge but has lost their personal identity. Identical suits conceal the characters' bodies and filter their voices. This sets up a question: what distinguishes people when their identifying features and personal histories are removed?

The first encounter makes trust consequential. Fleeing the approaching figure leads to isolation and a bad ending; staying leads to cooperation. The bad ending then rejoins the cooperative route. Later, an apparently endangered child provokes urgency that conflicts with another character's reasoning. My reading is that this develops the tension between instinct and judgment, and may foreshadow manipulation. The captors' purpose and “Mankind's Enemy” remain unresolved in the existing text.

The implemented sequence is:

1. Main menu → awakening (`scene0script001.txt`).
2. Roman numeral puzzle: fill three slots to make 30; X in the middle can mean multiplication.
3. Corridor encounter (`scene0script002.txt`) → run or stay.
4. Running plays `scene0script002badend.txt`, then rejoins the stay route.
5. Meeting the first companion (`scene0script002-2.txt`) → seven-segment counting puzzle.
6. Meeting two more people and hearing the child (`scene0script003.txt`) → Braille multiplication puzzle.
7. The current content stops at the Braille puzzle. Its `On Completed` Inspector event is available for connecting the next section.

Story wording, speaker labels, puzzle targets, starting dot states, and the three-move Braille budget were preserved during the refactor.

## Running the project

The project targets **Unity 6000.2.1f1**. Open `Assets/Scenes/OnlyScene.unity` and enter Play Mode. Dialogue advances with the left mouse button or Space; clicking during typing reveals the current line. Tiles drag and snap to the grid. Braille dots toggle with a click.

Scene sections are GameObjects inside the single scene. Inspector references determine which sections activate after dialogue, a choice, or puzzle completion. This keeps the existing authoring workflow. There is no separate scene loader, save system, or narrative graph yet.

## Code layout

| Folder | Responsibility |
| --- | --- |
| `Assets/Code/Dialogue` | Story entries, tag parsing, dialogue playback, typewriter display, and completion |
| `Assets/Code/Puzzles/RomanNumerals` | Roman numeral rules, slot occupancy reporting, and puzzle control |
| `Assets/Code/Puzzles/SevenSegment` | Required tile arrangement and completion |
| `Assets/Code/Puzzles/Braille` | Dot state, digit interpretation, move budget, completion, and reset button |
| `Assets/Code/Puzzles/Shared` | Grid tile dragging, tile values, and delayed puzzle transitions |
| `Assets/Code/UI` | Choices, hints, click feedback, delayed text, pulsing, and UI movement |
| `Assets/Code/Shared` | Null-safe scene-object activation helpers |
| `Assets/Tests` | Edit Mode rule tests and Play Mode lifecycle regressions |
| `Tools` | Scene-wiring checks that run without Unity |

Runtime code belongs to the `OrbitalGenesis.Runtime` assembly. Test assemblies reference it and are excluded from normal player builds. Moved and renamed scripts retain their original `.meta` GUIDs; the scene's component identifiers were updated to match.

## Authoring dialogue

Story assets use two tags:

```text
<s>You</s>
<l>Where am I?</l>
<l>And why can I remember facts, but not my name?</l>
<s>Human A</s>
<l>We should compare what we know.</l>
```

A speaker remains in effect until the next `<s>` tag. Each `<l>` becomes one advanceable entry. Unicode, multiline dialogue, and TextMeshPro rich text are supported. A trailing speaker tag produces no extra line.

Assign the TextAsset and text components to `VisualNovelDialogSystem`. Configure the objects to enable and disable on completion. `LoadFromTextAsset` replaces and displays dialogue; `LoadFromText` only replaces the parsed entries. `LoadFromStreamingAssets` loads asynchronously and displays the result.

## Puzzle conventions

`RomanNumeralRules` and `BrailleDigitPatterns` contain presentation-independent rules. Managers handle progress and presentation. UI references are optional for puzzle evaluation; missing text should never prevent a solution.

Roman numeral slots distinguish an empty slot from a tile whose value is zero. Slot reporters track ownership so a departing tile cannot clear its replacement. The manager evaluates after tile updates, and dragging tiles do not count as placed.

Braille arrays use **row order**: top-left, top-right, middle-left, middle-right, bottom-left, bottom-right. These correspond to standard Braille dots **1, 4, 2, 5, 3, 6**, not numerical dot order. Keep this order when assigning new cells.

Player toggles consume moves; programmatic changes and resets do not. Exhausting the budget without a solution restores the Inspector-configured starting dots. A correct final move succeeds. Solved dots stop accepting input until reset. The scene starts at 1 × 9, and 5 × 7 is reachable in three toggles.

`BrailleResetButton` handles the existing collider-based reset button; the manager also accepts an optional Canvas UI Button. `On Completed` fires once per successful attempt.

The first two puzzles retain their original delayed destruction of completed puzzle objects. Replaying those sections would require restoring or instantiating the objects; a save/replay system is outside the current implementation.

## Conventions for changes

Use PascalCase for types, methods, and properties; camelCase for private fields and parameters; and private `[SerializeField]` fields for Inspector configuration. Match component filenames to their classes. Keep story content in TextAssets, puzzle rules independent of UI, and event subscriptions paired with unsubscriptions. Comments should explain intent, ordering, or constraints. `.editorconfig` supplies the formatting defaults.

Preserve `.meta` files when moving Unity assets. Keep serialized field names stable, or use `FormerlySerializedAs` when renaming one.

## Validation

Run the scene-wiring checks from the project root:

```sh
python3 Tools/validate_scene.py
```

These check local references, script types, all five connected story assets, the third story's Braille transition, the reset button, and whether the configured Braille target is reachable within its move budget.

In Unity, open **Window → General → Test Runner** and run both Edit Mode and Play Mode tests. They cover story parsing, the Roman numeral operator, Braille patterns, final-move success, resetting and subscription lifecycles, slot ownership, and dialogue completion/reloading.

During this refactor, all runtime and test sources compiled against the installed Unity libraries. Six scene-wiring checks and 16 logic/story checks passed. Unity Play Mode tests could not run because the local licensing client failed to initialize. The installed editor was 6000.3.10f1; the working project's editor version and packages were left unchanged. A visual playthrough in the project's target editor remains necessary.
