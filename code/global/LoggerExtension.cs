using Sandbox;
using Sandbox.Diagnostics;

namespace TTTReborn.Globals
{
    public static class LoggerExtension
    {
        public static void Debug(this Logger log, params object[] obj)
        {
            if (!Gamemode.TTTGame.Instance?.Debug ?? true)
            {
                return;
            }

            string host = Game.IsServer ? "SERVER" : "CLIENT";

            log.Info($"[DEBUG][{host}] {string.Join(',', obj)}");
        }
    }
}
