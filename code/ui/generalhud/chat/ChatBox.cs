using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class ChatBox : Panel
    {
        public static ChatBox Instance { get; private set; }

        public readonly List<ChatEntry> Messages = new();

        public const int MAX_MESSAGES_COUNT = 200;
        public const float MAX_DISPLAY_TIME = 8f;

        public bool IsOpened { get; private set; } = false;

        private TimeSince _lastChatFocus = 0f;

        private readonly Panel _canvas;
        private readonly TextEntry _input;

        public ChatBox()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/chat/ChatBox.scss");

            _canvas = Add.Panel("chat_canvas");
            _canvas.PreferScrollToBottom = true;

            _input = Add.TextEntry("");
            _input.AddEventListener("onsubmit", Submit);
            _input.AddEventListener("onblur", Close);
            _input.AcceptsFocus = true;
            _input.AllowEmojiReplace = true;

            Sandbox.Hooks.Chat.OnOpenChat += Open;
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("dead", Local.Pawn.LifeState == LifeState.Dead);

            if (IsOpened)
            {
                _lastChatFocus = 0f;
            }

            _canvas.SetClass("hide", _lastChatFocus > MAX_DISPLAY_TIME);
        }

        private void Open()
        {
            IsOpened = true;

            SetClass("open", true);

            _input.Focus();
        }

        private void Close()
        {
            IsOpened = false;

            SetClass("open", false);

            _input.Blur();
        }

        private void Submit()
        {
            Close();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            string msg = _input.Text.Trim();

            _input.Text = "";

            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            Say(msg, player.LifeState);
        }

        public void AddEntry(string name, string message, string avatar, LifeState lifeState)
        {
            _lastChatFocus = 0f;

            if (Messages.Count > MAX_MESSAGES_COUNT)
            {
                ChatEntry entry = Messages[0];

                Messages.RemoveAt(0);

                entry.Delete();
            }

            ChatEntry chatEntry = _canvas.AddChild<ChatEntry>();
            chatEntry.Name = name;
            chatEntry.Text = message;

            chatEntry.Message.Text = message;

            chatEntry.SetClass("noname", string.IsNullOrEmpty(name));
            chatEntry.SetClass("noavatar", string.IsNullOrEmpty(avatar));

            bool showHead = Messages.Count == 0 || name == null || Messages[Messages.Count - 1].Name != name;

            if (showHead)
            {
                chatEntry.NameLabel.Text = name;
                chatEntry.NameLabel.AddClass(lifeState == LifeState.Alive ? "alive" : "dead");

                chatEntry.Avatar.SetTexture(avatar);
            }

            chatEntry.SetClass("showHead", showHead);

            chatEntry.Index = Messages.Count;

            Messages.Add(chatEntry);
        }

        [ClientCmd("chat_add", CanBeCalledFromServer = true)]
        public static void AddChatEntry(string name, string message, string avatar = null, LifeState lifeState = LifeState.Alive)
        {
            Instance?.AddEntry(name, message, avatar, lifeState);

            // Only log clientside if we're not the listen server host
            if (!Global.IsListenServer)
            {
                Log.Info($"{name}: {message}");
            }
        }

        [ClientCmd("chat_addinfo", CanBeCalledFromServer = true)]
        public static void AddInformation(string message, string avatar = null, LifeState lifeState = LifeState.Alive)
        {
            Instance?.AddEntry(null, message, avatar, lifeState);
        }

        [ServerCmd("say")]
        public static void Say(string message, LifeState lifeState)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // TODO: Consider RegEx to remove any messed up user chat messages.
            if (message.Contains('\n') || message.Contains('\r'))
            {
                return;
            }

            Log.Info($"{ConsoleSystem.Caller}: {message}");

            if (Gamemode.Game.Instance?.Round is Rounds.InProgressRound && lifeState == LifeState.Dead)
            {
                AddChatEntry(To.Multiple(Utils.GetDeadClients()), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", lifeState);
            }
            else
            {
                AddChatEntry(To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", lifeState);
            }
        }
    }
}

