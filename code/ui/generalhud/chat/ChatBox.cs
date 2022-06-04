using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Teams;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class ChatBox : Panel
    {
        public enum Channel { Info, Player, Spectator, Team };
        public static ChatBox Instance { get; private set; }

        public readonly List<ChatEntry> Messages = new();

        public const int MAX_MESSAGES_COUNT = 200;
        public const float MAX_DISPLAY_TIME = 8f;

        public bool IsOpened { get; private set; } = false;
        public bool IsTeamChatting { get; private set; } = false;

        private TimeSince _lastChatFocus = 0f;

        private Panel Canvas { get; set; }
        private Panel InputPanel { get; set; }
        private Panel InputTeamIndicator { get; set; }
        private ChatBoxTextEntry InputField { get; set; }

        public ChatBox() : base()
        {
            Instance = this;

            Canvas.PreferScrollToBottom = true;

            InputField.CaretColor = Color.White;
            InputField.AcceptsFocus = true;
            InputField.AllowEmojiReplace = true;
            InputField.Text = "";
            InputField.AddEventListener("onsubmit", Submit);
            InputField.AddEventListener("onblur", Close);

            Sandbox.Hooks.Chat.OnOpenChat += Open;
        }

        public void OnTab()
        {
            if (Local.Pawn is not Player player || player.LifeState != LifeState.Alive)
            {
                return;
            }

            if (CanUseTeamChat(player))
            {
                IsTeamChatting = !IsTeamChatting;
            }
        }

        public override void Tick()
        {
            base.Tick();

            bool isAlive = Local.Pawn != null && Local.Pawn.LifeState == LifeState.Alive;

            if (isAlive && Local.Pawn is Player player && IsTeamChatting)
            {
                InputTeamIndicator.Style.BackgroundColor = player.Team.Color;
                InputPanel.Style.BorderColor = player.Team.Color;
            }
            else
            {
                InputTeamIndicator.Style.BackgroundColor = null;
                InputPanel.Style.BorderColor = null;
            }

            InputTeamIndicator.SetClass("background-color-alive", isAlive);
            InputTeamIndicator.SetClass("background-color-spectator", !isAlive);
            InputPanel.SetClass("border-color-alive", isAlive);
            InputPanel.SetClass("border-color-spectator", !isAlive);

            if (IsOpened)
            {
                _lastChatFocus = 0f;
            }

            Canvas.SetClass("fadeout", _lastChatFocus > MAX_DISPLAY_TIME);
        }

        private void Open()
        {
            IsOpened = true;

            InputPanel.SetClass("opacity-medium", true);
            InputPanel.SetClass("open", true);

            InputField.Focus();
        }

        private void Close()
        {
            IsTeamChatting = false;
            IsOpened = false;

            InputPanel.SetClass("opacity-medium", false);
            InputPanel.SetClass("open", false);

            InputField.Text = "";
            InputField.Blur();
        }

        private void Submit()
        {
            bool wasTeamChatting = IsTeamChatting;

            string msg = InputField.Text.Trim();

            if (!string.IsNullOrWhiteSpace(msg) && Local.Pawn is Player)
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
        }

        public void AddEntry(string header, string content, Channel channel, string avatar = null, string teamName = null)
        {
            _lastChatFocus = 0f;

            if (channel == Channel.Team && (string.IsNullOrEmpty(teamName) || TeamFunctions.TryGetTeam(teamName) == null))
            {
                Log.Error("Cannot add chat entry to Team channel without a team name.");

                return;
            }

            header ??= "";

            #region Cleanup Old Messages
            if (Messages.Count > MAX_MESSAGES_COUNT)
            {
                ChatEntry entry = Messages[0];
                Messages.RemoveAt(0);
                entry.Delete();
            }
            #endregion

            ChatEntry chatEntry = Canvas.AddChild<ChatEntry>();

            chatEntry.Header.Text = header;
            chatEntry.Content.Text = content;
            chatEntry.Channel = channel;

            chatEntry.Header.SetClass("disable", string.IsNullOrEmpty(header));
            chatEntry.Header.SetClass("header", chatEntry.Channel != Channel.Info);
            chatEntry.Content.SetClass("disable", string.IsNullOrEmpty(content));
            chatEntry.Content.SetClass("header", chatEntry.Channel == Channel.Info);
            chatEntry.Content.SetClass("text-color-info", chatEntry.Channel == Channel.Info);

            switch (channel)
            {
                case Channel.Info:
                    chatEntry.Header.AddClass("text-color-info");

                    break;

                case Channel.Player:
                    chatEntry.Header.AddClass("text-color-alive");

                    break;

                case Channel.Spectator:
                    chatEntry.Header.AddClass("text-color-spectator");

                    break;

                case Channel.Team:
                    chatEntry.Header.Style.FontColor = TeamFunctions.GetTeam(teamName).Color;

                    break;
            }

            bool showHeader =
                Messages.Count == 0 ||
                Messages[^1].Channel != chatEntry.Channel ||
                !Messages[^1].Header.Text.Equals(chatEntry.Header.Text) ||
                chatEntry.Channel == Channel.Info;

            if (showHeader)
            {
                chatEntry.Avatar.SetTexture(avatar);
            }

            chatEntry.SetClass("show-header", showHeader);

            Messages.Add(chatEntry);
        }
        public static bool CanUseTeamChat(Player player) => player.LifeState == LifeState.Alive && player.Team.GetType() == typeof(TraitorTeam);

        [ConCmd.Client("chat_add", CanBeCalledFromServer = true)]
        public static void AddChatEntry(string name, string message, Channel channel, string avatar = null, string team = null)
        {
            Instance?.AddEntry(name, message, channel, avatar, team);

            // Only log clientside if we're not the listen server host
            if (!Global.IsListenServer)
            {
                Log.Info($"{name}: {message}");
            }
        }

        [ConCmd.Client("chat_addinfo", CanBeCalledFromServer = true)]
        public static void AddInformation(string message, string avatar = null)
        {
            Instance?.AddEntry(null, message, Channel.Info, avatar);
        }

        [ConCmd.Server("say")]
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
                AddChatEntry(To.Multiple(Utils.GetClients((pl) => pl.LifeState == LifeState.Dead)), ConsoleSystem.Caller.Name, message, Channel.Spectator, $"avatar:{ConsoleSystem.Caller.PlayerId}");
            }
            else
            {
                AddChatEntry(To.Everyone, ConsoleSystem.Caller.Name, message, Channel.Player, $"avatar:{ConsoleSystem.Caller.PlayerId}");
            }
        }

        [ConCmd.Server("sayteam")]
        public static void SayTeam(string message)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // TODO: Consider RegEx to remove any messed up user chat messages.
            if (ConsoleSystem.Caller.Pawn is not Player player || !CanUseTeamChat(player) || message.Contains('\n') || message.Contains('\r'))
            {
                return;
            }

            Log.Info($"{ConsoleSystem.Caller}: {message}");

            List<Client> clients = new();

            player.Team.Members.ForEach(member => clients.Add(member.Client));

            AddChatEntry(To.Multiple(clients), ConsoleSystem.Caller.Name, message, Channel.Team, $"avatar:{ConsoleSystem.Caller.PlayerId}", player.Team.Name);
        }
    }
}

