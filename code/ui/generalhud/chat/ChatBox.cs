using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        private static Regex MESSAGE_CLEAN_PATTERN => new Regex(@"(\\n|\\t|\\r)");

        public readonly List<ChatEntry> Messages = new();

        public const int MAX_MESSAGES_COUNT = 200;
        public const float MAX_DISPLAY_TIME = 8f;

        public bool IsOpened { get; private set; } = false;
        public bool IsTeamChatting { get; private set; } = false;

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

            SetClass("open", false);

            _input.Blur();
        }

        private void Submit()
        {
            bool wasTeamChatting = IsTeamChatting;

            Close();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            string msg = _input.Text.Trim();

            _input.Text = "";

            msg = MESSAGE_CLEAN_PATTERN.Replace(msg, " ");

            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            if (msg.StartsWith('!'))
            {
                string cmd = msg.TrimStart('!');

                ConsoleResult cr = ConsoleSystem.Run(cmd);

                if (cr.WasSuccess)
                {
                    AddEntry("SYSTEM", $"You ran the cmd: '{cmd}'.");
                }
                else
                {
                    if (!cr.WasFound)
                    {
                        AddEntry("SYSTEM", $"Cmd: '{cmd}' was not found!");

                        return;
                    }

                    AddEntry("SYSTEM", $"Error with cmd: '{cmd}'!");
                }

                return;
            }

            if (wasTeamChatting)
            {
                SayTeam(msg);
            }
            else
            {
                Say(msg);
            }
        }

        public void AddEntry(string name, string message, string avatar = null, LifeState lifeState = LifeState.Alive, string team = null)
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
            Instance?.AddEntry(null, message, avatar, lifeState);
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

            message = MESSAGE_CLEAN_PATTERN.Replace(message, " ");

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

            message = MESSAGE_CLEAN_PATTERN.Replace(message, " ");

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

