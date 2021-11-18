using Sandbox;

public static class LoggerExtenstion
{
    public static void Debug(this Logger log, object obj = null)
    {
        if (!TTTReborn.Gamemode.Game.Instance.Debug)
        {
            return;
        }

        log.Info($"[DEBUG] {obj}");
    }
}
