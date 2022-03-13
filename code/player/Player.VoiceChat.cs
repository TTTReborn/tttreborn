using System.Collections.Generic;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Teams;

namespace TTTReborn
{
    public partial class Player
    {
        public bool IsTeamVoiceChatEnabled { get; private set; } = false;

        // clientside-only
        public bool IsSpeaking { get; internal set; } = false;

        private static readonly Dictionary<Player, List<Client>> OldReceiveClients = new();

        private void TickPlayerVoiceChat()
        {
            using (Prediction.Off())
            {
                IsSpeaking = false;

                if (Input.Pressed(InputButton.Walk))
                {
                    if (Local.Pawn is Player player && CanUseTeamVoiceChat(player))
                    {
                        RequestTeamChat(true);
                    }
                }
                else if (Input.Released(InputButton.Walk))
                {
                    RequestTeamChat(false);
                }

                if (Input.Down(InputButton.Voice) || IsTeamVoiceChatEnabled)
                {
                    IsSpeaking = true;

                    UI.VoiceChatDisplay.Instance?.OnVoicePlayed(Client, 1f);
                }
            }
        }

        [ServerCmd(Name = "ttt_requestteamchat")]
        public static void RequestTeamChat(bool toggle)
        {
            Player player = ConsoleSystem.Caller.Pawn as Player;

            if (!player.IsValid() || toggle && !CanUseTeamVoiceChat(player))
            {
                return;
            }

            ToggleTeamChat(player, toggle);
        }

        private static void ToggleTeamChat(Player player, bool toggle)
        {
            player.IsTeamVoiceChatEnabled = toggle;

            List<Client> clients = new();

            if (toggle)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is Player pawnPlayer && player.Team == pawnPlayer.Team)
                    {
                        clients.Add(client);
                    }
                }

                OldReceiveClients[player] = clients;
            }
            else
            {
                // Player has not talked before
                if (!OldReceiveClients.ContainsKey(player))
                {
                    return;
                }

                // cleanup disconnected clients
                foreach (Client client in OldReceiveClients[player])
                {
                    if (client.IsValid())
                    {
                        clients.Add(client);
                    }
                }

                OldReceiveClients[player] = clients;
            }

            ClientToggleTeamVoiceChat(To.Multiple(clients), player, toggle);
        }

        private static bool CanUseTeamVoiceChat(Player player)
        {
            return player.LifeState == LifeState.Alive && player.Team.GetType() == typeof(TraitorTeam);
        }

        [ClientRpc]
        public static void ClientToggleTeamVoiceChat(Player player, bool toggle)
        {
            if (!player.IsValid())
            {
                return;
            }

            player.IsTeamVoiceChatEnabled = toggle;

            if (Local.Pawn is Player localPlayer && localPlayer != player)
            {
                return;
            }

            // De-/Activate voice chat
            ConsoleSystem.Run((toggle ? "+" : "-") + "iv_voice");
        }

        [Event(TTTEvent.Player.Role.SELECT)]
        private static void OnSelectRole(Player player)
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

            Client playerClient = player.Client;

            // sync already talking other players with the current player
            foreach (Client client in Client.All)
            {
                if (client.Pawn is Player pawnPlayer && player != pawnPlayer && pawnPlayer.IsTeamVoiceChatEnabled)
                {
                    bool activateTalking = player.Team == pawnPlayer.Team;

                    if (activateTalking)
                    {
                        OldReceiveClients[pawnPlayer].Add(playerClient);
                    }
                    else
                    {
                        OldReceiveClients[pawnPlayer].Remove(playerClient);
                    }

                    ClientToggleTeamVoiceChat(To.Single(player), pawnPlayer, activateTalking);
                }
            }
        }
    }
}
