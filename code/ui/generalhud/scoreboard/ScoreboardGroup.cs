using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class ScoreboardGroup : Panel
    {
        public string GroupTitle { get; set; }
        public int GroupMembers = 0;

        private TranslationLabel GroupTitleLabel { get; set; }
        private TranslationLabel GroupScoreLabel { get; set; }
        private TranslationLabel GroupKarmaLabel { get; set; }
        private TranslationLabel GroupPingLabel { get; set; }
        private Panel GroupContent { get; set; }

        public ScoreboardGroup(Panel parent, string groupName) : base(parent)
        {
            GroupTitle = groupName;

            AddClass(groupName);

            GroupScoreLabel.UpdateTranslation(new TranslationData("SCOREBOARD.COLUMNS.SCORE"));
            GroupKarmaLabel.UpdateTranslation(new TranslationData("SCOREBOARD.COLUMNS.KARMA"));
            GroupPingLabel.UpdateTranslation(new TranslationData("SCOREBOARD.COLUMNS.PING"));
        }

        public ScoreboardEntry AddEntry(Client client)
        {
            ScoreboardEntry scoreboardEntry = GroupContent.AddChild<ScoreboardEntry>();
            scoreboardEntry.ScoreboardGroupName = GroupTitle;
            scoreboardEntry.Client = client;

            scoreboardEntry.Update();

            return scoreboardEntry;
        }

        public void UpdateLabel()
        {
            GroupTitleLabel.UpdateTranslation(new TranslationData($"SCOREBOARD.GROUP.{GroupTitle.ToUpper()}", GroupMembers));
        }
    }
}
