using Sandbox;

namespace TTTReborn.Globals
{
    public static class LoggerExtension
    {
        public static void Debug(this Logger log, object obj = null)
        {
            if (!Gamemode.Game.Instance.Debug)
            {
                return;
            }

            string host = Host.IsServer ? "SERVER" : "CLIENT";

            log.Info($"[DEBUG][{host}] {obj}");
        }
    }
}
