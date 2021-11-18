using Sandbox;

public static class LoggerExtenstion
{
    public static void Debug(this Logger log, object text = null)
    {
        if (!TTTReborn.Gamemode.Game.Instance.Debug)
        {
            return;
        }

        log.Info($"[DEBUG] {text}");
    }
}
