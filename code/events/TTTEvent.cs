using Sandbox;

namespace TTTReborn.Events
{
    public class TTTEvent
    {
        public static void Run(TTTEvents e)
        {
            Event.Run(e.EventName);
        }

        public static void Run<T>(TTTEvents e, T arg0)
        {
            Event.Run(e.EventName, arg0);
        }

        public static void Run<T, U>(TTTEvents e, T arg0, U arg1)
        {
            Event.Run(e.EventName, arg0, arg1);
        }
    }
}
