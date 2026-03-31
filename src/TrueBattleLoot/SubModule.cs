using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TrueBattleLoot
{
    /// <summary>
    /// Entry point for the TrueBattleLoot mod. Applies Harmony patches that
    /// remove value-based restrictions on battle loot, allowing high-quality
    /// items to appear in post-battle loot regardless of player level.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony("com.truebattleloot.bannerlord");
                _harmony.PatchAll();
                InformationManager.DisplayMessage(
                    new InformationMessage("True Battle Loot: Loaded successfully.", Colors.Green));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"True Battle Loot load error: {ex.Message}", Colors.Red));
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("com.truebattleloot.bannerlord");
        }
    }
}
