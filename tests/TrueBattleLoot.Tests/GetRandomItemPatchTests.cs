using System.Collections.Generic;
using TrueBattleLoot.Patches;
using TaleWorlds.Core;
using Xunit;

namespace TrueBattleLoot.Tests
{
    /// <summary>
    /// Unit tests for <see cref="GetRandomItemPatch"/>.
    /// </summary>
    public class GetRandomItemPatchTests
    {
        [Fact]
        public void CollectCandidates_EmptyEquipment_ReturnsEmptyList()
        {
            var equipment = new Equipment();
            List<EquipmentElement> candidates = GetRandomItemPatch.CollectCandidates(equipment);
            Assert.Empty(candidates);
        }

        [Fact]
        public void SelectWeightedRandom_SingleCandidate_ReturnsThatCandidate()
        {
            var element = new EquipmentElement();
            var candidates = new List<EquipmentElement> { element };
            EquipmentElement result = GetRandomItemPatch.SelectWeightedRandom(candidates);
            Assert.Equal(element, result);
        }
    }
}
