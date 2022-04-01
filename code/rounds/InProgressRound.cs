using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Map;
using TTTReborn.Settings;
using TTTReborn.Teams;

namespace TTTReborn.Rounds
{
    public partial class InProgressRound : BaseRound
    {
        public override string RoundName { get; set; } = "In Progress";

        [Net]
        public List<Player> Players { get; set; }

        [Net]
        public List<Player> Spectators { get; set; }

        private List<LogicButton> _logicButtons;

        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.RoundTime;
        }

        public override void OnPlayerKilled(Player player)
        {
            Players.Remove(player);
            Spectators.AddIfDoesNotContain(player);

            player.MakeSpectator();
            ChangeRoundIfOver();

            base.OnPlayerKilled(player);
        }

        public override void OnPlayerJoin(Player player)
        {
            Spectators.AddIfDoesNotContain(player);

            base.OnPlayerJoin(player);
        }

        public override void OnPlayerLeave(Player player)
        {
            Players.Remove(player);
            Spectators.Remove(player);

            ChangeRoundIfOver();

            base.OnPlayerLeave(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                // For now, if the Weapons.Count of the map is zero, let's just give the players
                // a fixed weapon loadout.
                if (Gamemode.Game.Instance.MapHandler.Weapons.Count < Players.Count)
                {
                    foreach (Player player in Players)
                    {
                        GiveFixedLoadout(player);
                    }
                }

                // Cache buttons for OnSecond tick.
                _logicButtons = Entity.All.Where(x => x.GetType() == typeof(LogicButton)).Select(x => x as LogicButton).ToList();
            }

            base.OnStart();
        }

        private static void GiveFixedLoadout(Player player)
        {
            Log.Debug($"Added Fixed Loadout to {player.Client.Name}");

            // Randomize between SMG and shotgun
            if (Utils.RNG.Next() % 2 == 0)
            {
                if (player.Inventory.TryAdd(new Shotgun(), deleteIfFails: true, makeActive: false))
                {
                    player.Inventory.Ammo.Give("ammo_buckshot", 16);
                }
            }
            else
            {
                if (player.Inventory.TryAdd(new SMG(), deleteIfFails: true, makeActive: false))
                {
                    player.Inventory.Ammo.Give("ammo_smg", 60);
                }
            }

            if (player.Inventory.TryAdd(new Pistol(), deleteIfFails: true, makeActive: false))
            {
                player.Inventory.Ammo.Give("ammo_pistol", 30);
            }
        }

        protected override void OnTimeUp()
        {
            LoadPostRound(TeamFunctions.GetTeam(typeof(InnocentTeam)));

            base.OnTimeUp();
        }

        private Team IsRoundOver()
        {
            List<Team> aliveTeams = new();

            foreach (Player player in Players)
            {
                if (player.CheckPreventWin())
                {
                    return null;
                }

                if (player.CheckWin() && !aliveTeams.Contains(player.Team))
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

        private static void LoadPostRound(Team winningTeam)
        {
            Gamemode.Game.Instance.MapSelection.TotalRoundsPlayed++;
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
                if (!ServerSettings.Instance.Debug.PreventWin)
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
            Team result = IsRoundOver();

            if (result != null && !ServerSettings.Instance.Debug.PreventWin)
            {
                LoadPostRound(result);

                return true;
            }

            return false;
        }

        protected override void OnFinish()
        {
            base.OnFinish();

            if (!Host.IsServer)
            {
                return;
            }

            List<ILoggedGameEvent> eventList = new();

            foreach (GameEvent gameEvent in GameEvents)
            {
                if (gameEvent is ILoggedGameEvent loggedGameEvent)
                {
                    // TODO merge TakeDamage on one victim

                    eventList.Add(loggedGameEvent);
                }
            }

            NetworkableGameEvent.RegisterNetworked(new Events.Game.GameResultsEvent(eventList));
        }

        [Event(typeof(Events.Player.Role.SelectEvent))]
        protected static void OnPlayerRoleChange(Player player)
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

        [Event(typeof(Events.Settings.ChangeEvent))]
        protected static void OnChangeSettings()
        {
            if (Host.IsClient || Gamemode.Game.Instance.Round is not InProgressRound inProgressRound)
            {
                return;
            }

            inProgressRound.ChangeRoundIfOver();
        }
    }
}
