using System;
using NUnit.Framework;
using EventPlannerSim.Data;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for VendorRelationship progression.
    /// Feature: event-planner-simulator, Property 19: Vendor Relationship Progression
    /// Validates: Requirements R20
    /// </summary>
    [TestFixture]
    public class VendorRelationshipPropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Level calculation
        /// For any vendor hired N times:
        /// N >= 5: Level 5
        /// N >= 3: Level 3
        /// N >= 1: Level 1
        /// N == 0: Level 0
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_RelationshipLevel_CalculatesCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                int timesHired = _random.Next(0, 20); // 0 to 19 hires
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = timesHired
                };

                int expectedLevel = timesHired switch
                {
                    >= 5 => 5,
                    >= 3 => 3,
                    >= 1 => 1,
                    _ => 0
                };

                Assert.AreEqual(expectedLevel, relationship.RelationshipLevel,
                    $"RelationshipLevel calculation failed. TimesHired: {timesHired}");
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Discount calculation
        /// Level 5+: 10% discount
        /// Level 3-4: 5% discount
        /// Level 0-2: 0% discount
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_DiscountPercent_CalculatesCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                int timesHired = _random.Next(0, 20);
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = timesHired
                };

                float expectedDiscount = relationship.RelationshipLevel switch
                {
                    >= 5 => 0.10f,
                    >= 3 => 0.05f,
                    _ => 0f
                };

                Assert.AreEqual(expectedDiscount, relationship.DiscountPercent, 0.001f,
                    $"DiscountPercent calculation failed. TimesHired: {timesHired}, Level: {relationship.RelationshipLevel}");
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - RecordHire increments counter
        /// For any relationship, calling RecordHire should increment timesHired by 1
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_RecordHire_IncrementsTimesHired()
        {
            for (int i = 0; i < 100; i++)
            {
                int initialHires = _random.Next(0, 50);
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = initialHires
                };

                relationship.RecordHire();

                Assert.AreEqual(initialHires + 1, relationship.timesHired,
                    $"RecordHire should increment timesHired. Initial: {initialHires}");
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Level monotonically increases
        /// For any relationship, recording more hires should never decrease the level
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_Level_MonotonicallyIncreases()
        {
            for (int i = 0; i < 100; i++)
            {
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = 0
                };

                int previousLevel = relationship.RelationshipLevel;
                int hiresToSimulate = _random.Next(1, 15);

                for (int j = 0; j < hiresToSimulate; j++)
                {
                    relationship.RecordHire();
                    int currentLevel = relationship.RelationshipLevel;

                    Assert.GreaterOrEqual(currentLevel, previousLevel,
                        $"Level should never decrease. Previous: {previousLevel}, Current: {currentLevel}, TimesHired: {relationship.timesHired}");

                    previousLevel = currentLevel;
                }
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Reliability revealed at level 3
        /// When timesHired >= 3, reliabilityRevealed should be true after RecordHire
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_ReliabilityRevealed_AtLevel3()
        {
            for (int i = 0; i < 100; i++)
            {
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = 0
                };

                // Hire until we reach level 3
                for (int j = 0; j < 3; j++)
                {
                    relationship.RecordHire();
                }

                Assert.IsTrue(relationship.reliabilityRevealed,
                    $"Reliability should be revealed at level 3. TimesHired: {relationship.timesHired}");
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Flexibility revealed at level 5
        /// When timesHired >= 5, flexibilityRevealed should be true after RecordHire
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_FlexibilityRevealed_AtLevel5()
        {
            for (int i = 0; i < 100; i++)
            {
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = 0
                };

                // Hire until we reach level 5
                for (int j = 0; j < 5; j++)
                {
                    relationship.RecordHire();
                }

                Assert.IsTrue(relationship.flexibilityRevealed,
                    $"Flexibility should be revealed at level 5. TimesHired: {relationship.timesHired}");
            }
        }

        /// <summary>
        /// Property 19: Vendor Relationship Progression - Discount is bounded
        /// Discount should always be between 0 and 0.10 (0% to 10%)
        /// **Validates: Requirements R20**
        /// </summary>
        [Test]
        public void VendorRelationship_DiscountPercent_IsBounded()
        {
            for (int i = 0; i < 100; i++)
            {
                int timesHired = _random.Next(0, 100);
                var relationship = new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = timesHired
                };

                Assert.GreaterOrEqual(relationship.DiscountPercent, 0f,
                    $"Discount should never be negative. TimesHired: {timesHired}");
                Assert.LessOrEqual(relationship.DiscountPercent, 0.10f,
                    $"Discount should never exceed 10%. TimesHired: {timesHired}");
            }
        }
    }
}
