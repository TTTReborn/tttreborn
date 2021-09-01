using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.UI
{
    public partial class ChatBox : Panel
    {
        public static ChatBox Instance { get; private set; }

        public readonly List<ChatEntry> Messages = new();

        public const int MAX_MESSAGES_COUNT = 200;
        public const float MAX_DISPLAY_TIME = 5f;

        public bool IsOpened { get; private set; } = false;
        public bool IsTeamChatting { get; private set; } = false;

        private TimeSince _lastChatFocus = 0f;

        private readonly Sandbox.UI.Panel _canvas;
        private readonly TextEntry _input;

        public ChatBox() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/chat/ChatBox.scss");

            _canvas = Add.Panel();
            _canvas.Style.BackgroundColor = ColorScheme.Primary;
            _canvas.AddClass("chat-canvas");
            _canvas.AddClass("rounded");
            _canvas.AddClass("opacity-75");
            _canvas.PreferScrollToBottom = true;

            _input = Add.TextEntry("");
            _input.Style.BackgroundColor = ColorScheme.Primary;
            _input.CaretColor = Color.White;
            _input.AddClass("input");
            _input.AddClass("rounded");
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

            if (IsTeamChatting && Local.Pawn is TTTPlayer player)
            {
                _input.Style.BorderBottomColor = player.Team.Color;
            }
            else
            {
                _input.Style.BorderBottomColor = null;
            }

            _input.Style.Dirty();

            _input.Focus();
        }

        private void OpenTeamChat()
        {
            IsTeamChatting = true;

            Open();
        }

        private void Close()
        {
            IsTeamChatting = false;
            IsOpened = false;

            _input.Text = "";

            SetClass("open", false);

            _input.Blur();
        }

        private void Submit()
        {
            bool wasTeamChatting = IsTeamChatting;

            string msg = _input.Text.Trim();

            if (!string.IsNullOrWhiteSpace(msg) && Local.Pawn is TTTPlayer)
            {
                if (wasTeamChatting)
                {
                    SayTeam(msg);
                }
                else
                {
                    Say(msg);
                }
            }

            Close();
        }

        public void AddEntry(string header, string content, string avatar, LifeState lifeState, string team = null)
        {
            _lastChatFocus = 0f;

            if (Messages.Count > MAX_MESSAGES_COUNT)
            {
                ChatEntry entry = Messages[0];

                Messages.RemoveAt(0);

                entry.Delete();
            }

            ChatEntry chatEntry = _canvas.AddChild<ChatEntry>();
            chatEntry.Name = header;

            chatEntry.Header.Text = header;
            chatEntry.Content.Text = content;

            chatEntry.Header.SetClass("disable", string.IsNullOrEmpty(header));
            chatEntry.Content.SetClass("disable", string.IsNullOrEmpty(content));

            bool showHeader = Messages.Count == 0 || header == null || Messages[^1].Name != header;

            if (showHeader)
            {
                chatEntry.Header.AddClass(lifeState == LifeState.Alive ? "alive" : "dead");
                chatEntry.Avatar.SetTexture(avatar);
            }

            chatEntry.SetClass("show-header", showHeader);

            if (!string.IsNullOrEmpty(team))
            {
                chatEntry.Style.BorderLeftWidth = Length.Pixels(4f);
                chatEntry.Style.BorderLeftColor = TeamFunctions.GetTeam(team).Color;
                chatEntry.Style.Dirty();
            }

            Messages.Add(chatEntry);
        }
        public static bool CanUseTeamChat(TTTPlayer player)
        {
            return player.LifeState == LifeState.Alive && player.Team.GetType() == typeof(TraitorTeam);
        }

        [ClientCmd("chat_add", CanBeCalledFromServer = true)]
        public static void AddChatEntry(string name, string message, string avatar = null, LifeState lifeState = LifeState.Alive, string team = null)
        {
            Instance?.AddEntry(name, message, avatar, lifeState, team);

            // Only log clientside if we're not the listen server host
            if (!Global.IsListenServer)
            {
                Log.Info($"{name}: {message}");
            }
        }

        [ClientCmd("chat_addinfo", CanBeCalledFromServer = true)]
        public static void AddInformation(string message, string avatar = null, LifeState lifeState = LifeState.Alive)
        {
            Instance?.AddEntry(message, null, avatar, lifeState);
        }

        [ClientCmd("open_teamchat")]
        public static void OpenTeamChatInput()
        {
            if (Local.Pawn is TTTPlayer player && CanUseTeamChat(player))
            {
                Instance?.OpenTeamChat();
            }
        }

        [ServerCmd("say")]
        public static void Say(string message)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // TODO: Consider RegEx to remove any messed up user chat messages.
            if (message.Contains('\n') || message.Contains('\r'))
            {
                return;
            }

            Log.Info($"{ConsoleSystem.Caller}: {message}");

            LifeState lifeState = ConsoleSystem.Caller.Pawn.LifeState;

            if (Gamemode.Game.Instance?.Round is Rounds.InProgressRound && lifeState == LifeState.Dead)
            {
                AddChatEntry(To.Multiple(Utils.GetDeadClients()), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", lifeState);
            }
            else
            {
                AddChatEntry(To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", lifeState);
            }
        }

        [ServerCmd("sayteam")]
        public static void SayTeam(string message)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // TODO: Consider RegEx to remove any messed up user chat messages.
            if (ConsoleSystem.Caller.Pawn is not TTTPlayer player || !CanUseTeamChat(player) || message.Contains('\n') || message.Contains('\r'))
            {
                return;
            }

            Log.Info($"{ConsoleSystem.Caller}: {message}");

            List<Client> clients = new();

            player.Team.Members.ForEach(member => clients.Add(member.GetClientOwner()));

            AddChatEntry(To.Multiple(clients), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}", player.LifeState, player.Team.Name);
        }
    }
}

