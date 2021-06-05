using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
    {

        public Scoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");

            new ScoreboardHeader(this);
            new ScoreboardMain(this);
        }
        public class ScoreboardHeader : Panel
        {

            public class ScoreboardHeaderTop : Panel
            {
                public class ScoreboardLogo : Panel
                {
                    // Server Logo here
                }
                public class ScoreboardInformation : Panel
                {
                    // Div for the textfields 
                    public Label ServerName { set; get; }
                    public Label ServerDescription { set; get; }
                    public Label PlayerConnected { set; get; }
                    public Label MaxPlayer { set; get; }
                    public Label Map { set; get; }

                    public ScoreboardInformation(Panel parent)
                    {
                        Parent = parent;
                        ServerName = Add.Label("Here will be the servername", "servername");
                        ServerDescription = Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.", "serverdescription");
                        PlayerConnected = Add.Label("1", "playerconnected");
                        MaxPlayer = Add.Label("4", "maxplayer");
                        Map = Add.Label("grassland", "map");
                    }
                    public override void Tick()
                    {
                        // Overwrite PlayerConnected
                    }
                }
                public ScoreboardHeaderTop(Panel parent)
                {
                    Parent = parent;
                    new ScoreboardLogo(this);
                    new ScoreboardInformation(this);
                }
            }
            public class ScoreboardHeaderTablehead : Panel
            {
                public class PlayerLeft : Panel
                {
                    public Label PlayerLeftCount { set; get; }
                    public Label RoleText { set; get; }
                    public PlayerLeft(Panel parent)
                    {
                        Parent = parent;
                        PlayerLeftCount = Add.Label("X", "playerleftcount");
                        PlayerLeftCount.SetClass("enemys");
                        RoleText = Add.Label(" left", "roletext");
                    }
                    public override void Tick()
                    {
                        string PlayerLeftText;
                        TTTPlayer player = Local.Pawn as TTTPlayer;
                        if (player != null)
                        {
                            if (player.Role.ToString() == "Traitor")
                            {
                                // TODO: Get player left number
                                RoleLabel.Text = "0";
                                RoleText.Text = " Innocents left";
                                return;
                            }
                        }
                        RoleLabel.Text = "0";
                        RoleText.Text = " Traitor left";
                    }
                }
                public Label Karma;
                public Label Score;
                public Label Ping;
                public ScoreboardHeaderTablehead(Panel parent)
                {
                    Parent = parent;
                    new PlayerLeft(this);
                    Karma = Add.Label("Karma", "karma");
                    Score = Add.Label("Score", "score");
                    Ping = Add.Label("Ping", "ping");
                }
                // protected override void AddHeader()
                // {
                //     // Is the the table header?
                //     Header = Add.Panel("header");
                //     Header.Add.Label("Player", "name");
                //     Header.Add.Label("Karma", "karma");
                //     Header.Add.Label("Score", "score");
                //     Header.Add.Label("Deaths", "deaths");
                //     Header.Add.Label("Ping", "ping");
                // }
            }
            public ScoreboardHeader(Panel parent)
            {
                Parent = parent;
                new ScoreboardHeaderTop(this);
                new ScoreboardHeaderTablehead(this);
            }
        }

        public class ScoreboardMain : Panel
        {
            public class ScoreboadGroup : Panel
            {
                public Label Title;
                public class ScoreboadGroupContent : Panel
                {
                    public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
                    {
                        public class EntryPicture : Panel
                        {
                            // Steam Avatar
                        }
                        public Label PlayerName;
                        public Label Score;
                        public Label Karma;
                        public Label Ping;

                        public ScoreboardEntry() : base()
                        {
                            PlayerName = Add.Label(entry.GetString("name"), "playername");
                            Score = Add.Label("12", "score");
                            Karma = Add.Label("1000", "karma");
                            Ping = Add.Label("102", "ping");
                        }

                        public override void UpdateFrom(PlayerScore.Entry entry)
                        {
                            Entry = entry;

                            PlayerName.Text = entry.GetString("name"); // Does the playername need to update? In orig. you get kicked if you change ur name
                            Score.Text = entry.Get<int>("kills", 0).ToString();
                            Deaths.Text = entry.Get<int>("deaths", 0).ToString();
                            Karma.Text = entry.Get<int>("karma", 0).ToString();
                            Ping.Text = entry.Get<int>("ping", 0).ToString();

                            SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
                        }
                    }
                    public ScoreboadGroupContent(Panel parent)
                    {
                        Parent = parent;
                        // groupPlayerList.sort()
                        // for (player in groupPlayerList)
                        // new ScoreboardEntry(player)
                    }
                }
                public ScoreboardGroup(Panel parent, string group)
                {
                    Parent = parent;
                    SetClass(group, true);
                    Title = Add.Label(group + " - NUM.PLAYER");
                    Title.SetClass("groupTitle", true);

                    new ScoreboadGroupContent(this);
                }
            }

            public ScoreboardMain(Panel parent)
            {
                // for each player group (alive,  missing, dead, spectator)
                // for (group in playerGroups)
                // new ScoreboardGroup(group);
                parent = parent;
                new ScoreboadGroup(this, "alive");
            }
        }
    }
}
