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

        public static void Debug(this Logger log, params object[] obj)
        {
            if (!Gamemode.Game.Instance.Debug)
            {
                return;
            }

            string host = Host.IsServer ? "SERVER" : "CLIENT";

            log.Info($"[DEBUG][{host}] {string.Join(',', obj)}");
        }
    }
}
