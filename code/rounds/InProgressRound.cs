using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Map;
using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Settings;
using TTTReborn.Teams;

namespace TTTReborn.Rounds
{
    public class InProgressRound : BaseRound
    {
        public override string RoundName => "In Progress";
        private List<TTTLogicButton> _logicButtons;
        private List<TTTPlayer> _startingRoundPlayers = new();
        private bool _areRolesAssigned = false;

        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.RoundTime;
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
            ChangeRoundIfOver();
        }

        public override void OnPlayerLeave(TTTPlayer player)
        {
            base.OnPlayerLeave(player);
            ChangeRoundIfOver();
        }

        [Event(TTTEvent.Player.Role.Select)]
        private static void OnPlayerRoleChange(TTTPlayer player)
        {
            if (Host.IsClient)
            {
                return;
            }

            if (Gamemode.Game.Instance.Round is InProgressRound inProgressRound)
            {
                inProgressRound.ChangeRoundIfOver();
            }
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client is not TTTPlayer player)
                    {
                        return;
                    }

                    player.Client.SetValue("forcedspectator", player.IsForcedSpectator);

                    if (player.LifeState == LifeState.Dead)
                    {
                        player.Respawn();
                    }
                    else
                    {
                        player.SetHealth(player.MaxHealth);
                    }

                    if (!player.IsForcedSpectator)
                    {
                        SetLoadout(player);
                        _startingRoundPlayers.Add(player);
                    }
                }

                // Cache buttons for OnSecond tick.
                _logicButtons = Entity.All.Where(x => x.GetType() == typeof(TTTLogicButton)).Select(x => x as TTTLogicButton).ToList();

                AssignRoles();
            }
        }

        protected override void OnTimeUp()
        {
            LoadPostRound(TeamFunctions.GetTeam(typeof(InnocentTeam)));

            base.OnTimeUp();
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            if (player.IsForcedSpectator)
            {
                player.MakeSpectator();
            }
            else
            {
                SetLoadout(player);
            }

            base.OnPlayerSpawn(player);
        }

        private static void SetLoadout(TTTPlayer player)
        {
            Extensions.Log.Debug($"Added loadout to {player.Client.Name}");

            player.Inventory.TryAdd(new Hands(), true);

            // Randomize between SMG and shotgun
            if (Utils.RNG.Next() % 2 == 0)
            {
                if (player.Inventory.TryAdd(new Shotgun(), false))
                {
                    player.Inventory.Ammo.Give("buckshot", 16);
                }
            }
            else
            {
                if (player.Inventory.TryAdd(new SMG(), false))
                {
                    player.Inventory.Ammo.Give("smg", 60);
                }
            }

            if (player.Inventory.TryAdd(new Pistol(), false))
            {
                player.Inventory.Ammo.Give("pistol", 30);
            }
        }

        private TTTTeam IsRoundOver()
        {
            List<TTTTeam> aliveTeams = new();

            foreach (TTTPlayer player in Utils.GetAlivePlayers())
            {
                if (player.Team != null && !aliveTeams.Contains(player.Team))
                {
                    aliveTeams.Add(player.Team);
                }
            }

            if (aliveTeams.Count == 0)
            {
                return TeamFunctions.GetTeam(typeof(NoneTeam));
            }

            return aliveTeams.Count == 1 ? aliveTeams[0] : null;
        }

        private void AssignRoles()
        {
            // TODO: There should be a neater way to handle this logic.
            int traitorCount = (int) Math.Max(_startingRoundPlayers.Count * 0.25f, 1f);

            for (int i = 0; i < traitorCount; i++)
            {
                List<TTTPlayer> unassignedPlayers = _startingRoundPlayers.Where(p => p.Role is NoneRole).ToList();
                int randomId = Utils.RNG.Next(unassignedPlayers.Count);

                if (unassignedPlayers[randomId].Role is NoneRole)
                {
                    unassignedPlayers[randomId].SetRole(new TraitorRole());
                }
            }

            foreach (TTTPlayer player in _startingRoundPlayers)
            {
                if (player.Role is NoneRole)
                {
                    player.SetRole(new InnocentRole());
                }

                // send everyone their roles
                using (Prediction.Off())
                {
                    player.SendClientRole();
                }
            }

            _areRolesAssigned = true;
        }

        private static void LoadPostRound(TTTTeam winningTeam)
        {
            Gamemode.Game.Instance.ForceRoundChange(new PostRound());
            RPCs.ClientOpenAndSetPostRoundMenu(
                winningTeam.Name,
                winningTeam.Color
            );
        }

        public override void OnSecond()
        {
            if (Host.IsServer)
            {
                if (!Settings.ServerSettings.Instance.Debug.PreventWin)
                {
                    base.OnSecond();
                }
                else
                {
                    RoundEndTime += 1f;
                }

                _logicButtons.ForEach(x => x.OnSecond()); // Tick role button delay timer.

                if (!Utils.HasMinimumPlayers() && IsRoundOver() == null)
                {
                    Gamemode.Game.Instance.ForceRoundChange(new WaitingRound());
                }
            }
        }

        private bool ChangeRoundIfOver()
        {
            if (!_areRolesAssigned)
            {
                return false;
            }

            TTTTeam result = IsRoundOver();

            if (result != null && !Settings.ServerSettings.Instance.Debug.PreventWin)
            {
                LoadPostRound(result);

                return true;
            }

            return false;
        }

        [Event(TTTReborn.Events.TTTEvent.Settings.Change)]
        private static void OnChangeSettings()
        {
            if (Gamemode.Game.Instance.Round is not InProgressRound inProgressRound)
            {
                return;
            }

            inProgressRound.ChangeRoundIfOver();
        }
    }
}
