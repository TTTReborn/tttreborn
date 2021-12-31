using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public partial class ScoreboardGroup : Panel
    {
        public readonly string GroupTitle;
        public int GroupMembers = 0;

        private readonly Panel _groupTitlePanel;
        private readonly TranslationLabel _groupTitleLabel;
        private readonly TranslationLabel _groupKarmaLabel;
        private readonly TranslationLabel _groupPingLabel;
        private readonly Panel _groupContent;

        public ScoreboardGroup(Panel parent, string groupName) : base(parent)
        {
            GroupTitle = groupName;

            AddClass(groupName);
            AddClass("text-shadow");

            _groupTitlePanel = new(this);
            _groupTitlePanel.AddClass("group-title-panel");
            _groupTitlePanel.AddClass("opacity-medium");
            _groupTitlePanel.AddClass("rounded-top");

            _groupTitleLabel = _groupTitlePanel.Add.TranslationLabel(new TranslationData());
            _groupTitleLabel.AddClass("group-title-label");

            _groupKarmaLabel = _groupTitlePanel.Add.TranslationLabel(new TranslationData("SCOREBOARD_PLAYER_STATUS_KARMA"));
            _groupKarmaLabel.AddClass("group-karma-label");

            _groupPingLabel = _groupTitlePanel.Add.TranslationLabel(new TranslationData("SCOREBOARD_PLAYER_STATUS_PING"));
            _groupPingLabel.AddClass("group-ping-label");

            _groupContent = new(this);
            _groupContent.AddClass("group-content-panel");
        }

        public ScoreboardEntry AddEntry(Client client)
        {
            ScoreboardEntry scoreboardEntry = _groupContent.AddChild<ScoreboardEntry>();
            scoreboardEntry.ScoreboardGroupName = GroupTitle;
            scoreboardEntry.Client = client;

            scoreboardEntry.Update();

            return scoreboardEntry;
        }

        // TODO: Implement logic for the player counter in the title
        public void UpdateLabel()
        {
            _groupTitleLabel.UpdateTranslation(new TranslationData($"SCOREBOARD_GROUP_{GroupTitle.ToUpper()}", GroupMembers));
        }
    }
}
