using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Player;

// Chat needs to hook into s&box in order to function.
namespace Sandbox.Hooks
{
    public static partial class Chat
    {
        public static event Action OnOpenChat;

        [ClientCmd("openchat")]
        internal static void MessageMode()
        {
            OnOpenChat?.Invoke();
        }

    }
}

namespace TTTReborn.UI
{
    public partial class Chat : Panel
    {
        public static Chat Instance;

        private Panel Canvas { get; set; }
        private TextEntry Input { get; set; }

        public Chat()
        {
            Instance = this;

            StyleSheet.Load("/ui/Chat.scss");

            Canvas = Add.Panel("chat_canvas");

            Input = Add.TextEntry("");
            Input.AddEventListener("onsubmit", Submit);
            Input.AddEventListener("onblur", Close);
            Input.AcceptsFocus = true;
            Input.AllowEmojiReplace = true;

            Sandbox.Hooks.Chat.OnOpenChat += Open;
        }

        private void Open()
        {
            AddClass("open");
            Input.Focus();
        }

        private void Close()
        {
            RemoveClass("open");
            Input.Blur();
        }

        private void Submit()
        {
            Close();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            var msg = Input.Text.Trim();
            Input.Text = "";

            if (string.IsNullOrWhiteSpace(msg))
                return;


            Say(msg, player.LifeState);
        }

        public void AddEntry(bool isAlive, string name, string message, string avatar)
        {
            var chatEntry = Canvas.AddChild<ChatEntry>();
            chatEntry.Message.Text = message;

            chatEntry.NameLabel.Text = name;
            chatEntry.NameLabel.AddClass(isAlive ? "alive" : "dead");

            chatEntry.Avatar.SetTexture(avatar);

            chatEntry.SetClass("noname", string.IsNullOrEmpty(name));
            chatEntry.SetClass("noavatar", string.IsNullOrEmpty(avatar));
        }

        [ClientCmd("chat_add", CanBeCalledFromServer = true)]
        public static void AddChatEntry(string name, string message, string avatar = null, bool isAlive = true)
        {
            Instance?.AddEntry(isAlive, name, message, avatar);

            // Only log clientside if we're not the listen server host
            if (!Global.IsListenServer)
            {
                Log.Info($"{name}: {message}");
            }
        }

        [ClientCmd("chat_addinfo", CanBeCalledFromServer = true)]
        public static void AddInformation(string message, string avatar = null, bool isAlive = true)
        {
            Instance?.AddEntry(isAlive, null, message, avatar);
        }

        [ServerCmd("say")]
        public static void Say(string message, LifeState lifeState)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // TODO: Reject more messed up user inputs
            if ( message.Contains('\n') || message.Contains('\r'))
                return;

            Log.Info($"{ConsoleSystem.Caller}: {message}");

            if (lifeState == LifeState.Dead)
            {
                var deadClients = Gamemode.Game.GetDeadClients();
                AddChatEntry(To.Multiple(deadClients), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", false);
            }
            else
            {
                AddChatEntry(To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", true);
            }
        }
    }
}

