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
                }
                else if (Input.Released(InputButton.Run) && IsTeamVoiceChatEnabled)
                {
                    ConsoleSystem.Run("requestteamchat", false);
                }

                // Edge-case fix (if `requestteamchat false` was sent, but IsTeamVoiceChatEnabled was not already set)
                if (Input.Down(InputButton.Run))
                {
                    _teamChatButtonPressPending = 0f;
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

            List<Client> clients = new();

            if (toggle)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer pawnPlayer && player.Team == pawnPlayer.Team)
                    {
                        clients.Add(client);
                    }
                }

                _oldReceiveClients[player] = clients;
            }
            else
            {
                // Player has not talked before
                if (!_oldReceiveClients.ContainsKey(player))
                {
                    return;
                }

                // cleanup disconnected clients
                foreach (Client client in _oldReceiveClients[player])
                {
                    if (client.IsValid())
                    {
                        clients.Add(client);
                    }
                }

                _oldReceiveClients[player] = clients;
            }

            ClientToggleTeamVoiceChat(To.Multiple(clients), player, toggle);
        }

        public static bool CanUseTeamVoiceChat(TTTPlayer player)
        {
            return player.LifeState == LifeState.Alive && player.Team.GetType() == typeof(TraitorTeam);
        }

        [ClientRpc]
        public static void ClientToggleTeamVoiceChat(TTTPlayer player, bool toggle)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.IsTeamVoiceChatEnabled = toggle;

            if (Local.Pawn is TTTPlayer localPlayer && localPlayer != player)
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

            // sync already talking other players with the current player
            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer pawnPlayer && player != pawnPlayer && pawnPlayer.IsTeamVoiceChatEnabled)
                {
                    bool activateTalking = player.Team == pawnPlayer.Team;

                    if (activateTalking)
                    {
                        _oldReceiveClients[pawnPlayer].Add(playerClient);
                    }
                    else
                    {
                        _oldReceiveClients[pawnPlayer].Remove(playerClient);
                    }

                    ClientToggleTeamVoiceChat(To.Single(player), pawnPlayer, activateTalking);
                }
            }
        }
    }
}
