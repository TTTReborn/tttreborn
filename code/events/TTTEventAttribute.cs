using Sandbox;

namespace TTTReborn.Events
{
    public class TTTEventAttribute : EventAttribute
    {
        public TTTEventAttribute(string eventName) : base(eventName) { }
        public TTTEventAttribute(TTTEvents gameEvent) : base(gameEvent.EventName) { }
    }
}
