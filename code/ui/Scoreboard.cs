using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Scoreboard : Panel
    {
        private Dictionary<int, ScoreboardEntry> Entries = new();

        private Dictionary<string, ScoreboardGroup> ScoreboardGroups = new();

        private Header header;

        private TableHeader tableHeader;

        private Panel mainContent;

        private Panel footer;

        public Scoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");

            header = new Header(this);
            tableHeader = new TableHeader(this);

            mainContent = Add.Panel("mainContent");

            // TODO: create playergroups & for each playergroup:
            AddScoreboardGroup("Alive");

            PlayerScore.OnPlayerAdded += AddPlayer;
            PlayerScore.OnPlayerUpdated += UpdatePlayer;
            PlayerScore.OnPlayerRemoved += RemovePlayer;

            footer = Add.Panel("footer");

            // TODO: Implement UpdatePlayer method
            // PlayerScore.OnPlayerUpdated += UpdatePlayer;

            foreach (PlayerScore.Entry player in PlayerScore.All)
            {
                AddPlayer(player);
            }
        }

        public class Header : Panel
        {
            public Panel ScoreboardLogo;
            public Panel InformationHolder;
            public Label ServerName;
            public Label ServerInfo;
            public Label ServerDescription;

            public Header(Panel parent)
            {
                Parent = parent;

                ScoreboardLogo = Add.Panel("scoreboardLogo");
                InformationHolder = Add.Panel("informationHolder");
                ServerName = InformationHolder.Add.Label("Trouble in Terry's Town", "serverName"); // Here will be the servername
                ServerInfo = InformationHolder.Add.Label(GetServerInfoStr(), "serverInfo");
                //ServerDescription = InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");
            }

            public void UpdateServerInfo()
            {
                this.ServerInfo.Text = this.GetServerInfoStr();
            }

            public string GetServerInfoStr()
            {
                // TODO: Get this out of the header
                // TODO: Fill the other variables
                return $"{PlayerScore.All.Length} Player(s) - Map: '{Sandbox.Global.MapName}'";
            }
        }

        public class TableHeader : Panel
        {
            public Label PlayerAliveCountLabel;
            public Label KarmaLabel;
            public Label ScoreLabel;
            public Label PingLabel;

            public TableHeader(Panel parent)
            {
                Parent = parent;

                PlayerAliveCountLabel = Add.Label("? players left", "name");
                KarmaLabel = Add.Label("Karma", "karma");
                ScoreLabel = Add.Label("Score", "score");
                PingLabel = Add.Label("Ping", "ping");
            }

            public override void Tick()
            {
                PlayerAliveCountLabel.Text = $"{TTTReborn.Gamemode.Game.GetConfirmedPlayers().Count} players left";
            }
        }

        public class ScoreboardGroup : Panel
        {
            public string GroupTitle { get; private set; }
            public Panel GroupContent;
            public int GroupMembers = 0;
            private Panel groupTitleWrapper;
            private Label groupTitleLabel;

            public ScoreboardGroup(Panel parent, string groupName)
            {
                Parent = parent;

                GroupTitle = groupName;

                AddClass(groupName);
                groupTitleWrapper = Add.Panel("scoreboardGroup__title-wrapper");
                groupTitleLabel = groupTitleWrapper.Add.Label("", "scoreboardGroup__title");
                GroupContent = Add.Panel("scoreboardGroup__content");
            }

            // TODO: Implement logic for the player counter in the title
            public void UpdateLabel()
            {
                groupTitleLabel.Text = $"{GroupTitle.ToUpper()}  -  {GroupMembers}";
            }

            public ScoreboardEntry AddEntry(PlayerScore.Entry entry)
            {
                ScoreboardEntry scoreboardEntry = GroupContent.AddChild<ScoreboardEntry>();
                scoreboardEntry.ScoreboardGroupName = GroupTitle;

                scoreboardEntry.UpdateFrom(entry);

                return scoreboardEntry;
            }
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (ScoreboardGroups.ContainsKey(groupName))
            {
                return ScoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new ScoreboardGroup(mainContent, groupName);
            scoreboardGroup.UpdateLabel();

            ScoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
        }

        private void AddPlayer(PlayerScore.Entry entry)
        {
            ScoreboardGroup scoreboardGroup = GetScoreboardGroup(entry);
            ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry(entry);

            scoreboardGroup.GroupMembers += 1;

            Entries.Add(entry.Id, scoreboardEntry);

            scoreboardGroup.UpdateLabel();
            header.UpdateServerInfo();
        }

        // TODO add MIA
        private ScoreboardGroup GetScoreboardGroup(PlayerScore.Entry entry)
        {
            string group = "Alive";

            if (!entry.Get<bool>("alive", true))
            {
                // TODO better spectator check, maybe with a player var
                group = "Dead";
            }

            ScoreboardGroups.TryGetValue(group, out ScoreboardGroup scoreboardGroup);

            if (scoreboardGroup == null)
            {
                scoreboardGroup = AddScoreboardGroup(group);
            }

            return scoreboardGroup;
        }

        private void UpdatePlayer(PlayerScore.Entry entry)
        {
            if (Entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                ScoreboardGroup scoreboardGroup = GetScoreboardGroup(entry);

                if (scoreboardGroup.GroupTitle != panel.ScoreboardGroupName)
                {
                    // instead of remove and add, move the panel into the right parent
                    RemovePlayer(entry);
                    AddPlayer(entry);

                    DeleteEmptyScoreboardGroups();

                    return;
                }

                panel.UpdateFrom(entry);
            }
            else
            {
                // Add to queue? Up to now, just print an error #hacky
                Log.Error($"Tried to update the ScoreboardPanel of the player with sid: '{entry.Get<ulong>("steamid")}'");
            }
        }

        private void DeleteEmptyScoreboardGroups()
        {
            List<string> removeList = new();

            foreach (KeyValuePair<string, ScoreboardGroup> keyValuePair in ScoreboardGroups)
            {
                if (keyValuePair.Value.GroupMembers == 0)
                {
                    removeList.Add(keyValuePair.Key);
                }
            }

            foreach (string key in removeList)
            {
                ScoreboardGroups[key].Delete();

                ScoreboardGroups.Remove(key);
            }
        }

        private void RemovePlayer(PlayerScore.Entry entry)
        {
            if (Entries.TryGetValue(entry.Id, out ScoreboardEntry panel))
            {
                ScoreboardGroups.TryGetValue(panel.ScoreboardGroupName, out ScoreboardGroup scoreboardGroup);

                if (scoreboardGroup != null)
                {
                    scoreboardGroup.GroupMembers -= 1;
                }

                scoreboardGroup.UpdateLabel();

                panel.Delete();
                Entries.Remove(entry.Id);
            }
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("open", Input.Down(InputButton.Score));
        }
    }
}
