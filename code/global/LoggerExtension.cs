using Sandbox;

namespace TTTReborn.Globals
{
    public static class LoggerExtension
    {
        public static void Debug(this Logger log, params object[] obj)
        {
            if (!Gamemode.Game.Instance?.Debug ?? true)
            {
                return;
            }

            string host = Host.IsServer ? "SERVER" : "CLIENT";

            log.Info($"[DEBUG][{host}] {string.Join(',', obj)}");
        }
    }
}
