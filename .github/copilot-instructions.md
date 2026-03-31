# TrueBattleLoot — Copilot Instructions

## Project Overview

Bannerlord mod that removes the vanilla value-based loot restrictions from post-battle spoils. The vanilla game caps lootable item value based on the player's roguery skill, causing most battle loot to be low-value junk. This mod raises the value cap and replaces item selection with a value-weighted random pick so high-quality equipment has a real chance of appearing. Uses Harmony patches against `DefaultBattleRewardModel` — no game files are modified.

## Tech Stack

- **Language:** C# 9.0 targeting .NET Framework 4.7.2
- **Game SDK:** TaleWorlds Mount & Blade II: Bannerlord (TaleWorlds.Core, TaleWorlds.CampaignSystem, TaleWorlds.Library, TaleWorlds.MountAndBlade)
- **Patching:** Harmony 2.2.2 for runtime method patching
- **Testing:** xUnit 2.6.6
- **Nullable:** Disabled project-wide

## Build, Test & Deploy Commands

```powershell
# Build
dotnet build src\TrueBattleLoot\TrueBattleLoot.csproj -c Release

# Run tests
dotnet test tests\TrueBattleLoot.Tests\TrueBattleLoot.Tests.csproj

# Deploy to game
./deploy.ps1
```

## Architecture

| File | Role |
|------|------|
| `SubModule.cs` | Mod entry point — applies Harmony patches on load, reverts them on unload |
| `Patches/LootValuePatch.cs` | Postfix on `DefaultBattleRewardModel.GetExpectedLootedItemValueFromCasualty` — multiplies the vanilla value cap by 100x |
| `Patches/GetRandomItemPatch.cs` | Prefix on `DefaultBattleRewardModel.GetRandomItem` — replaces item selection with value-weighted random from all equipment slots |

### Key Design Decisions

- **Postfix for value cap (`LootValuePatch`):** Uses a Postfix rather than Prefix so the vanilla roguery calculation still runs. The result is then multiplied by `ValueMultiplier` (100). Only positive values are modified — zero results (where vanilla intentionally disables loot) are preserved.
- **Prefix for item selection (`GetRandomItemPatch`):** Fully replaces the original method because the vanilla logic filters candidates by target value first. The replacement collects all non-empty `EquipmentElement` slots and uses value-weighted random selection (`Item.Value` as weight, minimum weight of 1).
- **No CampaignBehaviorBase needed:** All changes are pure Harmony patches on the reward model — no campaign events or save data involved.
- **Single Harmony ID:** `"com.truebattleloot.bannerlord"` for clean bulk unpatching in `OnSubModuleUnloaded`.

## Code Conventions

- **Namespace:** `TrueBattleLoot` at root, `TrueBattleLoot.Patches` for Harmony patches
- **Patch classes:** `internal static` with `[HarmonyPatch]` attribute targeting the specific class and method
- **XML documentation:** All public and internal types and methods must have `<summary>`, `<param>`, and `<returns>` XML doc comments
- **Error handling:** `SubModule.OnSubModuleLoad` wraps `PatchAll()` in try-catch and displays colored messages via `InformationManager.DisplayMessage` (green for success, red for errors)
- **Null safety:** Use `?.` and `?? fallback` for properties that may be null outside full game context
- **Equipment iteration:** Use `(int)EquipmentIndex.NumEquipmentSetSlots` as the loop bound for equipment slots

## Module Metadata

`Module/SubModule.xml` defines the mod for Bannerlord's launcher:
- **Id:** `TrueBattleLoot`
- **Dependencies:** Native, SandBoxCore, Sandbox, StoryMode
- **Entry point:** `TrueBattleLoot.SubModule`

Keep `SubModule.xml` in sync with any namespace or class name changes.

## Post-Change Workflow

After making any code changes, always follow these steps in order:

1. **Build:** `dotnet build src\TrueBattleLoot\TrueBattleLoot.csproj -c Release` — confirm the project compiles
2. **Write tests:** Add or update tests covering the new or changed code
3. **Test:** `dotnet test tests\TrueBattleLoot.Tests\TrueBattleLoot.Tests.csproj` — confirm all tests pass before proceeding
4. **Deploy:** `./deploy.ps1` — copies the built DLL and SubModule.xml to the game's module folder

Do not deploy if the build fails or any tests are failing.

## Testing Guidelines

- Tests use `InternalsVisibleTo` to access `internal` methods directly
- `LootValuePatch.Postfix` can be tested by calling with various `ref float` values — verify positive values are multiplied, zero and negative values remain unchanged
- `GetRandomItemPatch.CollectCandidates` can be tested with empty `Equipment` instances — verify it returns an empty list
- `GetRandomItemPatch.SelectWeightedRandom` can be tested with single-element lists — verify it returns that element
- Full integration tests (actual loot generation from troop equipment) require a live campaign context with valid `DefaultBattleRewardModel` and `CharacterObject` instances
