using System.Collections.Generic;

using Sandbox;

using TTTReborn.Teams;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public bool IsTeamVoiceChatEnabled { get; private set; } = false;

        // clientside-only
        public bool IsSpeaking { get; internal set; } = false;

        private static Dictionary<TTTPlayer, List<Client>> _oldReceiveClients = new();

        private TimeSince _teamChatButtonPressPending = 0f;

        private const float _pressDelayFix = 0.5f;

        private TimeSince _testCounter = 0f;

        public void TickPlayerVoiceChat()
        {
            using (Prediction.Off())
            {
                IsSpeaking = false;

                if (Input.Down(InputButton.Voice) || IsTeamVoiceChatEnabled)
                {
                    IsSpeaking = true;

                    UI.VoiceList.Current?.OnVoicePlayed(GetClientOwner(), 1f);
                }

                if (Input.Pressed(InputButton.Run) && CanUseTeamVoiceChat(this))
                {
                    ConsoleSystem.Run("requestteamchat", true);

                    _testCounter = 0f;
                }
                else if (Input.Released(InputButton.Run) && IsTeamVoiceChatEnabled)
                {
                    ConsoleSystem.Run("requestteamchat", false);
                }

                if (Input.Down(InputButton.Run))
                {
                    _teamChatButtonPressPending = 0f;

                    Log.Error($"You're pressing InputButton.Run since {_testCounter}");
                }
                else if (IsTeamVoiceChatEnabled && _teamChatButtonPressPending > _pressDelayFix)
                {
                    ConsoleSystem.Run("requestteamchat", false);
                }
            }
        }

        [ServerCmd(Name = "requestteamchat")]
        public static void RequestTeamChat(bool toggle)
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid() || toggle && !CanUseTeamVoiceChat(player))
            {
                return;
            }

            ToggleTeamChat(player, toggle);
        }

        private static void ToggleTeamChat(TTTPlayer player, bool toggle)
        {
            player.IsTeamVoiceChatEnabled = toggle;

            List<Client> clients;

            if (toggle)
            {
                clients = new();

                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer pawnPlayer)
                    {
                        if (player.Team == pawnPlayer.Team)
                        {
                            clients.Add(client);
                        }
                    }
                }

                _oldReceiveClients[player] = clients;
            }
            else
            {
                // player did not talked before
                if (!_oldReceiveClients.ContainsKey(player))
                {
                    return;
                }

                clients = _oldReceiveClients[player];
            }

            player.ClientToggleTeamVoiceChat(To.Multiple(clients), toggle);
        }

        public static bool CanUseTeamVoiceChat(TTTPlayer player)
        {
            return player.Team == TTTTeam.GetTeam("Traitors");
        }

        [ClientRpc]
        private void ClientToggleTeamVoiceChat(bool toggle)
        {
            IsTeamVoiceChatEnabled = toggle;

            if (Local.Pawn is TTTPlayer localPlayer && localPlayer != this)
            {
                return;
            }

            // De-/Activate voice chat
            ConsoleSystem.Run((toggle ? "+" : "-") + "iv_voice");
        }

        [Event("tttreborn.player.role.onselect")]
        private static void OnSelectRole(TTTPlayer player)
        {
            if (!Host.IsServer)
            {
                return;
            }

            // if player is talking, stop talking to avoid information leaking
            if (player.IsTeamVoiceChatEnabled)
            {
                ToggleTeamChat(player, false);
            }

            Client playerClient = player.GetClientOwner();

            // sync already talking other players to the current player as well
            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer pawnPlayer)
                {
                    if (player != pawnPlayer && player.Team == pawnPlayer.Team && pawnPlayer.IsTeamVoiceChatEnabled)
                    {
                        _oldReceiveClients[player].Add(playerClient);

                        pawnPlayer.ClientToggleTeamVoiceChat(To.Single(player), true);
                    }
                }
            }
        }
    }
}
