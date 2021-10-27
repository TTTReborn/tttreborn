namespace TTTReborn.Events
{
    public static partial class TTTEvent
    {
        public static class Settings
        {
            /// <summary>
            /// Occurs when server or client settings are changed.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public const string Change = "tttreborn.settings.change";

            /// <summary>
            /// Occurs when server or client language settings are changed.
            /// <para>Event is passed the old <strong><see cref="TTTReborn.Globalization.Language"/></strong> instance.</para>
            /// <para>Event is passed the new <strong><see cref="TTTReborn.Globalization.Language"/></strong> instance.</para>
            /// </summary>
            public const string LanguageChange = "tttreborn.settings.languagechange";
        }
    }
}
