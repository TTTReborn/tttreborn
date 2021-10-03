using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ScoreboardGroup : Panel
    {
        public readonly string GroupTitle;
        public int GroupMembers = 0;

        private readonly Panel _groupTitlePanel;
        private readonly Label _groupTitleLabel;
        private readonly Label _groupKarmaLabel;
        private readonly Label _groupPingLabel;
        private readonly Panel _groupContent;

        public ScoreboardGroup(Sandbox.UI.Panel parent, string groupName) : base(parent)
        {
            GroupTitle = groupName;

            AddClass(groupName);
            AddClass("text-shadow");

            _groupTitlePanel = new(this);
            _groupTitlePanel.AddClass("group-title-panel");
            _groupTitlePanel.AddClass("opacity-medium");
            _groupTitlePanel.AddClass("rounded-top");

            _groupTitleLabel = _groupTitlePanel.Add.Label(groupName);
            _groupTitleLabel.AddClass("group-title-label");

            _groupKarmaLabel = _groupTitlePanel.Add.Label("Karma");
            _groupKarmaLabel.AddClass("group-karma-label");

            _groupPingLabel = _groupTitlePanel.Add.Label("Ping");
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
            _groupTitleLabel.Text = $"{GroupTitle.ToUpper()} - {GroupMembers}";
        }
    }
}
