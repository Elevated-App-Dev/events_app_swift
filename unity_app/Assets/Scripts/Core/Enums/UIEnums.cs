namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the apps available in the phone interface.
    /// </summary>
    public enum PhoneApp
    {
        Calendar,
        Messages,
        Bank,
        Contacts,
        Reviews,
        Tasks,
        Clients
    }

    /// <summary>
    /// Represents the current step in the tutorial sequence.
    /// </summary>
    public enum TutorialStep
    {
        Welcome,
        AcceptClient,
        SelectVenue,
        SelectCaterer,
        EventExecution,
        ViewResults,
        Complete
    }

    /// <summary>
    /// Represents background music tracks.
    /// </summary>
    public enum MusicTrack
    {
        MainMenu,
        Planning,
        Execution,
        Results,
        Celebration
    }

    /// <summary>
    /// Represents sound effects.
    /// </summary>
    public enum SoundEffect
    {
        ButtonClick,
        Success,
        Failure,
        Notification,
        CashRegister,
        Warning
    }
}
