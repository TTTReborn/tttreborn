namespace TTTReborn.Events.UI
{
    [GameEvent("ui_reloaded")]
    public partial class ReloadedEvent : GameEvent
    {
        /// <summary>
        /// Occurs when the UI was reloaded.
        /// </summary>
        public ReloadedEvent() : base() { }
    }
}
