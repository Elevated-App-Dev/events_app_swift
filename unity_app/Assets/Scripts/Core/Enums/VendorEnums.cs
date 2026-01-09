namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the category of vendor services.
    /// </summary>
    public enum VendorCategory
    {
        Caterer,
        Entertainer,
        Decorator,
        Photographer,
        Florist,
        Baker,
        RentalCompany,
        AVTechnician,
        Transportation,  // Post-MVP
        Security         // Post-MVP
    }

    /// <summary>
    /// Represents the quality/price tier of a vendor.
    /// </summary>
    public enum VendorTier
    {
        Budget,
        Standard,
        Premium,
        Luxury
    }
}
