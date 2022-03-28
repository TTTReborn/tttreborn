namespace TTTReborn.Events.Settings
{
    [GameEvent("settings_change")]
    public partial class ChangeEvent : ParameterlessGameEvent
    {
        /// <summary>
        /// Occurs when server or client settings are changed.
        /// </summary>
        public ChangeEvent() : base() { }
    }
}
