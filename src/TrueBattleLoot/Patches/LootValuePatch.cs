using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace TrueBattleLoot.Patches
{
    /// <summary>
    /// Patches <see cref="DefaultBattleRewardModel.GetExpectedLootedItemValueFromCasualty"/>
    /// to remove the item value cap that prevents high-quality items from appearing
    /// in battle loot. The vanilla game restricts loot value based on the player's
    /// roguery skill, causing most post-battle loot to consist of low-value junk.
    /// This patch raises the cap so all items have a chance of being looted.
    /// </summary>
    [HarmonyPatch(typeof(DefaultBattleRewardModel), "GetExpectedLootedItemValueFromCasualty")]
    internal static class LootValuePatch
    {
        /// <summary>
        /// Multiplier applied to the vanilla expected loot value.
        /// A value of 100 effectively removes value-based restrictions
        /// while still allowing the roguery system to scale loot quality.
        /// </summary>
        internal const float ValueMultiplier = 100f;

        /// <summary>
        /// After the vanilla method calculates the expected loot value,
        /// multiplies it to allow much higher-value items to drop.
        /// Only modifies positive results to avoid enabling loot where
        /// the vanilla system intentionally returns zero.
        /// </summary>
        internal static void Postfix(ref float __result)
        {
            if (__result > 0f)
            {
                __result *= ValueMultiplier;
            }
        }
    }
}
