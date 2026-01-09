using System;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Tracks the player's relationship with a specific vendor.
    /// Requirements: R20.1-R20.3
    /// </summary>
    [Serializable]
    public class VendorRelationship
    {
        public string vendorId;
        public int timesHired = 0;
        public bool reliabilityRevealed = false; // At level 3
        public bool flexibilityRevealed = false; // At level 5
        public string playerNote = ""; // Player's personal notes about this vendor

        /// <summary>
        /// Calculates the relationship level based on times hired.
        /// Level 0: Never hired
        /// Level 1: Hired 1-2 times
        /// Level 3: Hired 3-4 times (reliability revealed at Stage 3+)
        /// Level 5: Hired 5+ times (flexibility revealed at Stage 3+)
        /// </summary>
        public int RelationshipLevel => timesHired switch
        {
            >= 5 => 5,
            >= 3 => 3,
            >= 1 => 1,
            _ => 0
        };

        /// <summary>
        /// Discount percentage unlocked through relationship (Post-MVP).
        /// Level 3 = 5%, Level 5 = 10%
        /// </summary>
        public float DiscountPercent => RelationshipLevel switch
        {
            >= 5 => 0.10f,
            >= 3 => 0.05f,
            _ => 0f
        };

        /// <summary>
        /// Increments the times hired counter and updates revealed attributes.
        /// </summary>
        public void RecordHire()
        {
            timesHired++;
            UpdateRevealedAttributes();
        }

        /// <summary>
        /// Updates which hidden attributes should be revealed based on relationship level.
        /// Note: Actual reveal only happens at Stage 3+ (checked by caller).
        /// </summary>
        private void UpdateRevealedAttributes()
        {
            if (RelationshipLevel >= 3)
                reliabilityRevealed = true;
            if (RelationshipLevel >= 5)
                flexibilityRevealed = true;
        }
    }
}
