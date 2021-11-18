using Sandbox;

namespace TTTReborn.Globals
{
    public static class LoggerExtenstion
    {
        public static void Debug(this Logger log, object obj = null)
        {
            if (!Gamemode.Game.Instance.Debug)
            {
                return;
            }

            log.Info($"[DEBUG] {obj}");
        }
    }
}
