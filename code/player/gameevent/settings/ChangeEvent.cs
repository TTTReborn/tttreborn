namespace TTTReborn.Events
{
    public static partial class Settings
    {
        [GameEvent("settings_change")]
        public partial class ChangeEvent : ParameterlessGameEvent
        {
            /// <summary>
            /// Occurs when server or client settings are changed.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public ChangeEvent() : base() { }
        }
    }
}
