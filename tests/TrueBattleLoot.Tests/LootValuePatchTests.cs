using TrueBattleLoot.Patches;
using Xunit;

namespace TrueBattleLoot.Tests
{
    /// <summary>
    /// Unit tests for <see cref="LootValuePatch"/>.
    /// </summary>
    public class LootValuePatchTests
    {
        [Fact]
        public void Postfix_PositiveResult_MultipliesByFactor()
        {
            float result = 100f;
            LootValuePatch.Postfix(ref result);
            Assert.Equal(100f * LootValuePatch.ValueMultiplier, result);
        }

        [Fact]
        public void Postfix_ZeroResult_RemainsZero()
        {
            float result = 0f;
            LootValuePatch.Postfix(ref result);
            Assert.Equal(0f, result);
        }

        [Fact]
        public void Postfix_NegativeResult_RemainsUnchanged()
        {
            float result = -5f;
            LootValuePatch.Postfix(ref result);
            Assert.Equal(-5f, result);
        }

        [Fact]
        public void Postfix_SmallPositiveResult_StillMultiplied()
        {
            float result = 0.5f;
            LootValuePatch.Postfix(ref result);
            Assert.Equal(0.5f * LootValuePatch.ValueMultiplier, result);
        }
    }
}
