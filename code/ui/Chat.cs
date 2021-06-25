using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
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

        private readonly Panel _canvas;
        private readonly TextEntry _input;

        public Chat()
        {
            Instance = this;

            StyleSheet.Load("/ui/Chat.scss");

            _canvas = Add.Panel("chat_canvas");

            _input = Add.TextEntry("");
            _input.AddEventListener("onsubmit", Submit);
            _input.AddEventListener("onblur", Close);
            _input.AcceptsFocus = true;
            _input.AllowEmojiReplace = true;

            Sandbox.Hooks.Chat.OnOpenChat += Open;
        }

        private void Open()
        {
            AddClass("open");
            _input.Focus();
        }

        private void Close()
        {
            RemoveClass("open");
            _input.Blur();
        }

        private void Submit()
        {
            Close();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            var msg = _input.Text.Trim();
            _input.Text = "";

            if (string.IsNullOrWhiteSpace(msg))
                return;


            Say(msg, player.LifeState);
        }

        public void AddEntry(bool isAlive, string name, string message, string avatar)
        {
            var chatEntry = _canvas.AddChild<ChatEntry>();
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
            if (message.Contains('\n') || message.Contains('\r'))
            {

            }

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

