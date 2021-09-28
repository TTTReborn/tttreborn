namespace TTTReborn.Events
{
    public class SettingsEvents
    {
        /// <summary>
        /// Triggered when a settings are changed. <c>TTTPlayer</c> object is passed to events.
        /// </summary>
        public TTTEvents OnChanged => new("tttreborn.settings.changed");
    }
}
