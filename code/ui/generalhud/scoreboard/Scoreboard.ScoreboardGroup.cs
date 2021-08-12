using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Scoreboard
    {
        public enum DefaultScoreboardGroup
        {
            Alive,
            MIA,
            Dead,
            Spectator
        }

        private class ScoreboardGroup : TTTPanel
        {
            public readonly string GroupTitle;
            public int GroupMembers = 0;
            private readonly Panel _groupContent;
            private readonly Panel _groupTitleWrapper;
            private readonly Label _groupTitleLabel;

            public ScoreboardGroup(Panel parent, string groupName)
            {
                Parent = parent;

                GroupTitle = groupName;

                AddClass(groupName);

                _groupTitleWrapper = Add.Panel("scoreboardGroup__title-wrapper");
                _groupTitleLabel = _groupTitleWrapper.Add.Label("", "scoreboardGroup__title");
                _groupContent = Add.Panel("scoreboardGroup__content");
            }

            // TODO: Implement logic for the player counter in the title
            public void UpdateLabel()
            {
                _groupTitleLabel.Text = $"{GroupTitle.ToUpper()}  -  {GroupMembers}";
            }

            public ScoreboardEntry AddEntry(PlayerScore.Entry entry)
            {
                ScoreboardEntry scoreboardEntry = _groupContent.AddChild<ScoreboardEntry>();
                scoreboardEntry.ScoreboardGroupName = GroupTitle;
                scoreboardEntry.SteamId = entry.Get<ulong>("steamid");

                scoreboardEntry.UpdateFrom(entry);

                return scoreboardEntry;
            }
        }

        private ScoreboardGroup AddScoreboardGroup(string groupName)
        {
            if (_scoreboardGroups.ContainsKey(groupName))
            {
                return _scoreboardGroups[groupName];
            }

            ScoreboardGroup scoreboardGroup = new ScoreboardGroup(_mainContent, groupName);
            scoreboardGroup.UpdateLabel();

            _scoreboardGroups.Add(groupName, scoreboardGroup);

            return scoreboardGroup;
        }

        private ScoreboardGroup GetScoreboardGroup(PlayerScore.Entry entry)
        {
            string group = DefaultScoreboardGroup.Alive.ToString();
            ulong steamId = entry.Get<ulong>("steamid", 0);

            if (entry.Get<bool>("forcedspectator", false))
            {
                group = DefaultScoreboardGroup.Spectator.ToString();
            }
            else if (steamId != 0)
            {
                foreach (Client client in Client.All)
                {
                    if (client.SteamId == steamId && client.Pawn != null)
                    {
                        if (client.Pawn is not TTTPlayer player)
                        {
                            break;
                        }

                        if (player.IsConfirmed)
                        {
                            group = DefaultScoreboardGroup.Dead.ToString();
                        }
                        else if (player.IsMissingInAction)
                        {
                            group = DefaultScoreboardGroup.MIA.ToString();
                        }

                        break;
                    }
                }
            }

            _scoreboardGroups.TryGetValue(group, out ScoreboardGroup scoreboardGroup);

            return scoreboardGroup ?? AddScoreboardGroup(group);
        }

        private void UpdateScoreboardGroups()
        {
            foreach (ScoreboardGroup value in _scoreboardGroups.Values)
            {
                value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
                value.Style.Dirty();
            }
        }
    }
}
