namespace Sandbox
{
    [Library("ttt_bot_dummy")]
    public class DummyBot : Bot
    {
        [AdminCmd("ttt_bot_add_dummy", Help = "Spawn a dummy bot")]
        public static void Spawn()
        {
            _ = new DummyBot();
        }
    }
}
