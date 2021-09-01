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

        private readonly Panel _canvas;
        private readonly Panel _inputPanel;
        private readonly Panel _inputTeamIndicator;
        private readonly TextEntry _inputField;

        public ChatBox() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/chat/ChatBox.scss");

            _canvas = new Panel(this);
            _canvas.AddClass("chat-canvas");
            _canvas.AddClass("background-color-primary");
            _canvas.AddClass("rounded");
            _canvas.AddClass("opacity-90");
            _canvas.PreferScrollToBottom = true;

            _inputPanel = new Panel(this);
            _inputPanel.AddClass("input-panel");
            _inputPanel.AddClass("background-color-primary");
            _inputPanel.AddClass("opacity-0");
            _inputPanel.AddClass("rounded");

            _inputTeamIndicator = new Panel(_inputPanel);
            _inputTeamIndicator.AddClass("input-team-indicator");
            _inputTeamIndicator.AddClass("circular");

            _inputField = _inputPanel.Add.TextEntry("");
            _inputField.CaretColor = Color.White;
            _inputField.AcceptsFocus = true;
            _inputField.AllowEmojiReplace = true;
            _inputField.AddClass("input-field");
            _inputField.AddClass("text-color-player");
            _inputField.AddEventListener("onsubmit", Submit);
            _inputField.AddEventListener("onblur", Close);

            Sandbox.Hooks.Chat.OnOpenChat += Open;
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn.LifeState == LifeState.Alive)
            {
                if (IsTeamChatting && Local.Pawn is TTTPlayer player)
                {
                    _inputTeamIndicator.Style.BackgroundColor = player.Team.Color;
                    _inputPanel.Style.BorderColor = player.Team.Color;
                }
                else
                {
                    _inputTeamIndicator.Style.BackgroundColor = null;
                    _inputPanel.Style.BorderColor = null;
                }

                _inputTeamIndicator.AddClass("background-color-player");
                _inputPanel.AddClass("border-color-player");
            }
            else
            {
                _inputTeamIndicator.AddClass("background-color-spectator");
                _inputPanel.AddClass("border-color-spectator");
            }

            if (IsOpened)
            {
                _lastChatFocus = 0f;
            }

            _canvas.SetClass("fadeOut", _lastChatFocus > MAX_DISPLAY_TIME);
        }

        private void Open()
        {
            if (Input.Down(InputButton.Run) && Local.Pawn is TTTPlayer player && CanUseTeamChat(player))
            {
                IsTeamChatting = true;
            }

            IsOpened = true;

            _inputPanel.SetClass("opacity-90", true);
            _inputPanel.SetClass("open", true);

            _inputPanel.Style.Dirty();

            _inputField.Focus();
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

            _inputPanel.SetClass("opacity-90", false);
            _inputPanel.SetClass("open", false);

            _inputField.Text = "";
            _inputField.Blur();
        }

        private void Submit()
        {
            bool wasTeamChatting = IsTeamChatting;

            string msg = _inputField.Text.Trim();

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
                chatEntry.Header.Style.FontColor = Color.White;
                chatEntry.Avatar.SetTexture(avatar);
            }

            chatEntry.SetClass("show-header", showHeader);

            if (!string.IsNullOrEmpty(team))
            {
                chatEntry.Header.Style.FontColor = TeamFunctions.GetTeam(team).Color;
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

