using Sandbox;

[Library("bot_dummy")]
public class DummyBot : Bot
{
    [AdminCmd("bot_add_dummy", Help = "Spawn a dummy bot")]
    static void Spawn()
    {
        new DummyBot();
    }
}
