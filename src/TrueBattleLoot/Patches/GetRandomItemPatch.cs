using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace TrueBattleLoot.Patches
{
    /// <summary>
    /// Patches <see cref="DefaultBattleRewardModel.GetRandomItem"/> to select
    /// items from the full equipment set rather than filtering by target value.
    /// The vanilla method only considers items below a certain value threshold,
    /// causing expensive equipment to be ignored. This patch makes all non-empty
    /// equipment slots eligible and uses weighted random selection that favors
    /// higher-value items.
    /// </summary>
    [HarmonyPatch(typeof(DefaultBattleRewardModel), "GetRandomItem")]
    internal static class GetRandomItemPatch
    {
        /// <summary>
        /// Replaces the vanilla item selection logic. Collects all non-empty
        /// equipment elements and selects one using a value-weighted random,
        /// so better items are more likely to appear without completely
        /// excluding cheaper ones.
        /// </summary>
        /// <param name="equipment">The troop's equipment loadout.</param>
        /// <param name="targetValue">The vanilla value cap (ignored by this patch).</param>
        /// <param name="__result">The selected equipment element.</param>
        /// <returns><c>false</c> to skip the original method.</returns>
        internal static bool Prefix(Equipment equipment, float targetValue, ref EquipmentElement __result)
        {
            var candidates = CollectCandidates(equipment);

            if (candidates.Count == 0)
            {
                __result = EquipmentElement.Invalid;
                return false;
            }

            __result = SelectWeightedRandom(candidates);
            return false;
        }

        /// <summary>
        /// Collects all non-empty equipment elements from the given equipment.
        /// </summary>
        internal static List<EquipmentElement> CollectCandidates(Equipment equipment)
        {
            var candidates = new List<EquipmentElement>();
            for (int i = 0; i < (int)EquipmentIndex.NumEquipmentSetSlots; i++)
            {
                EquipmentElement element = equipment[(EquipmentIndex)i];
                if (!element.IsEmpty)
                {
                    candidates.Add(element);
                }
            }
            return candidates;
        }

        /// <summary>
        /// Selects a random equipment element from candidates, weighted by item value.
        /// Items with higher value have proportionally greater chance of selection.
        /// A minimum weight of 1 ensures cheap items still have a small chance.
        /// </summary>
        internal static EquipmentElement SelectWeightedRandom(List<EquipmentElement> candidates)
        {
            if (candidates.Count == 1)
                return candidates[0];

            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += GetWeight(candidates[i]);
            }

            float roll = (float)(new Random().NextDouble()) * totalWeight;
            float cumulative = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                cumulative += GetWeight(candidates[i]);
                if (roll <= cumulative)
                    return candidates[i];
            }

            return candidates[candidates.Count - 1];
        }

        private static float GetWeight(EquipmentElement element)
        {
            float value = element.Item != null ? element.Item.Value : 0f;
            return Math.Max(value, 1f);
        }
    }
}
