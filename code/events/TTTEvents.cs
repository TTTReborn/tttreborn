namespace TTTReborn.Events
{
    public class TTTEvents
    {
        public string EventName { get; private set; }

        public TTTEvents(string name) { EventName = name; }

        public override string ToString()
        {
            return $"{EventName}";
        }

        public static PlayerEvents Player { get; } = new();
        public static SettingsEvents Settings { get; } = new();
    }
}
