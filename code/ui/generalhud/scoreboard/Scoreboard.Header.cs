using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class Scoreboard
    {
        private class Header : Panel
        {
            //public Label ServerDescription;

            private readonly Sandbox.UI.Panel _scoreboardLogo;
            private readonly Label _serverName;
            private readonly Sandbox.UI.Panel _informationHolder;
            private readonly Label _serverInfo;

            public Header(Sandbox.UI.Panel parent)
            {
                Parent = parent;

                _scoreboardLogo = Add.Panel("scoreboardLogo");
                _informationHolder = Add.Panel("informationHolder");
                _serverName = _informationHolder.Add.Label("Trouble in Terry's Town", "serverName"); // Here will be the servername
                _serverInfo = _informationHolder.Add.Label("", "serverInfo");
                //ServerDescription = InformationHolder.Add.Label("This is the server description: Lorem ipsum dolor sit elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat", "serverDescription");
            }

            public void UpdateServerInfo()
            {
                _serverInfo.Text = $"{PlayerScore.All.Length} Player(s) - Map: '{Sandbox.Global.MapName}'";
            }
        }
    }
}
