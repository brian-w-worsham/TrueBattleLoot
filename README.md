# True Battle Loot — Bannerlord Mod

Tired of winning epic battles against elite troops only to loot tattered boots? This mod removes the vanilla value-based loot restrictions so that high-quality items have a real chance of appearing in your post-battle spoils.

## Features

- **No value cap on loot:** The vanilla game filters out expensive items based on your roguery skill level, causing most battle loot to be low-value junk. This mod removes that restriction so all items from fallen enemies are eligible
- **Value-weighted selection:** Higher-value items are prioritized in loot selection — you're more likely to see that lordly plate armor than a pair of sandals
- **Vanilla roguery system preserved:** The amount of loot you receive still scales with your roguery skill. The mod only changes *which* items can appear, not *how many*
- **Zero configuration:** Works automatically for all field battles, sieges, and ambushes

## How It Works

The vanilla loot system calculates an expected item value for each casualty based on the winner's roguery skill. Items above that value threshold are excluded from the loot pool. This mod:

1. Multiplies the expected loot value cap by 100x, making virtually all items eligible
2. Replaces the item selection logic with a value-weighted random pick from all equipment slots
3. The result: better loot quality from every battle without breaking game balance

## Prerequisites

- **Mount & Blade II: Bannerlord** (Steam version)
- **.NET Framework 4.7.2 targeting pack** (comes with Visual Studio)
- **Visual Studio 2022** or the **.NET SDK** with `dotnet build` support
- Bannerlord installed at the expected path, or update the path in the `.csproj`

## Project Structure

```
TrueBattleLoot/
├── TrueBattleLoot.sln
├── deploy.ps1                          # Build & deploy script
├── Module/
│   └── SubModule.xml                   # Bannerlord module definition
├── src/
│   └── TrueBattleLoot/
│       ├── TrueBattleLoot.csproj
│       ├── SubModule.cs                # Mod entry point (Harmony init)
│       └── Patches/
│           ├── LootValuePatch.cs       # Removes item value cap
│           └── GetRandomItemPatch.cs   # Value-weighted item selection
└── tests/
    └── TrueBattleLoot.Tests/
        ├── TrueBattleLoot.Tests.csproj
        ├── LootValuePatchTests.cs
        └── GetRandomItemPatchTests.cs
```

## Setup & Build

### 1. Verify your game path

Open `src/TrueBattleLoot/TrueBattleLoot.csproj` and verify the `<GameFolder>` property points to your Bannerlord installation:

```xml
<GameFolder>C:\Games\steamapps\common\Mount &amp; Blade II Bannerlord</GameFolder>
```

### 2. Build

```powershell
dotnet build src\TrueBattleLoot\TrueBattleLoot.csproj -c Release
```

### 3. Deploy

```powershell
.\deploy.ps1
```

Or specify a custom game path:

```powershell
.\deploy.ps1 -GameFolder "D:\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

### 4. Enable in-game

1. Launch Bannerlord
2. Go to **Mods** in the launcher
3. Enable **True Battle Loot**
4. Start or load a campaign

## Running Tests

```powershell
dotnet test tests\TrueBattleLoot.Tests\TrueBattleLoot.Tests.csproj
```
