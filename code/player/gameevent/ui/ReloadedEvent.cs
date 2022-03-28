namespace TTTReborn.Events
{
    public static partial class UI
    {
        [GameEvent("ui_reloaded")]
        public partial class ReloadedEvent : ParameterlessGameEvent
        {
            /// <summary>
            /// Occurs when the UI was reloaded.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public ReloadedEvent() : base() { }
        }
    }
}
