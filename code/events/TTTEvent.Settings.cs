namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Settings
        {
            /// <summary>
            /// Occurs when server settings are changed.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public const string Change = "tttreborn.settings.change";
        }
    }
}
