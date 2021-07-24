using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Scoreboard
    {
        private class ScoreboardGroup : Panel
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
            string group = "Alive";

            if (!entry.Get<bool>("alive", true))
            {
                // TODO better spectator check, maybe with a player var
                group = "Dead";
            }
            else
            {
                ulong steamId = entry.Get<ulong>("steamid", 0);

                if (steamId != 0)
                {
                    foreach (Client client in Client.All)
                    {
                        if (client.Pawn.IsValid() && client.SteamId == steamId)
                        {
                            if ((client.Pawn as TTTPlayer).IsMissingInAction)
                            {
                                group = "MIA";
                            }

                            break;
                        }
                    }
                }
            }

            _scoreboardGroups.TryGetValue(group, out ScoreboardGroup scoreboardGroup);

            if (scoreboardGroup == null)
            {
                scoreboardGroup = AddScoreboardGroup(group);
            }

            return scoreboardGroup;
        }

        private void DeleteEmptyScoreboardGroups()
        {
            List<string> removeList = new();

            foreach ((string key, ScoreboardGroup value) in _scoreboardGroups)
            {
                if (value.GroupMembers == 0)
                {
                    removeList.Add(key);
                }
            }

            foreach (string key in removeList)
            {
                _scoreboardGroups[key].Delete();

                _scoreboardGroups.Remove(key);
            }
        }
    }
}
