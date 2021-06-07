using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

// TODO export into Main and own Entry

namespace TTTReborn.UI
{
    public class OldScoreboard : Panel
    {
        public OldScoreboard()
        {
            StyleSheet.Load("/ui/Scoreboard.scss");

            // new ScoreboardHeader(this);
            // new ScoreboardMain(this);
        }

        public override void Tick()
        {
            base.Tick();

            SetClass("open", Local.Client?.Input.Down(InputButton.Score) ?? false);
        }

        public class ScoreboardHeader : Panel
        {
            public ScoreboardHeader(Panel parent)
            {
                Parent = parent;

                new ScoreboardHeaderTop(this);
                new ScoreboardHeaderTablehead(this);
            }

            public class ScoreboardHeaderTop : Panel
            {
                public ScoreboardHeaderTop(Panel parent)
                {
                    Parent = parent;

                    new ScoreboardLogo(this);
                    new ScoreboardInformation(this);
                }

                public class ScoreboardLogo : Panel
                {
                    public ScoreboardLogo(Panel parent)
                    {
                        Parent = parent;
                    }
                }

                public class ScoreboardInformation : Panel
                {
                    // Div for the textfields
                    public Label ServerName { set; get; }
                    public Label ServerInfo { set; get; }
                    public Label ServerDescription { set; get; }
                    public ScoreboardInformation(Panel parent)
                    {
                        Parent = parent;

                        ServerName = Add.Label("Here will be the servername", "serverName");
                        ServerInfo = Add.Label("", "serverInfo");
                        ServerDescription = Add.Label("This is the server description: Lorem ipsum dolor sit  elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.", "serverDescription");
                    }

                    public override void Tick()
                    {
                        // EXPECTED: player / maxPlayer - mapName - rounds left / time left
                        // 10/32 - ttt_minecraft_b5 - 3 rounds / 09:41 left
                        ServerInfo.Text = "PLAYER/MAXPLAYER - MAP - 3 rounds / 09:41 left";
                    }
                }
            }

            public class ScoreboardHeaderTablehead : Panel
            {


                public ScoreboardHeaderTablehead(Panel parent)
                {
                    Parent = parent;

                    new PlayerLeft(this);
                    new PlayerRight(this);
                }

                public class PlayerRight : Panel
                {
                    public PlayerRight(Panel parent)
                    {
                        Parent = parent;

                        Parent = parent;
                        Karma = Add.Label("Karma", "karma");
                        Score = Add.Label("Score", "score");
                        Ping = Add.Label("Ping", "ping");

                    }
                    public Label Karma { set; get; }
                    public Label Score { set; get; }
                    public Label Ping { set; get; }

                }
                public class PlayerLeft : Panel
                {
                    public Label PlayerLeftCount { set; get; }
                    public Label RoleText { set; get; }

                    public PlayerLeft(Panel parent)
                    {
                        Parent = parent;

                        PlayerLeftCount = Add.Label("X", "playerleftcount");
                        PlayerLeftCount.SetClass("enemys", true);
                        RoleText = Add.Label(" left", "roletext");
                    }

                    public override void Tick()
                    {
                        TTTPlayer player = Local.Pawn as TTTPlayer;

                        if (player != null)
                        {
                            if (player.Role.ToString() == "Traitor")
                            {
                                // TODO: Get player left number
                                PlayerLeftCount.Text = "0";
                                RoleText.Text = " Innocents left";

                                return;
                            }
                        }

                        PlayerLeftCount.Text = "0";
                        RoleText.Text = " Traitor left";
                    }
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
        }

        public class ScoreboardMain : Panel
        {
            public ScoreboardMain(Panel parent)
            {
                // for each player group (alive,  missing, dead, spectator)
                // for (group in playerGroups)
                // new ScoreboardGroup(group);
                Parent = parent;

                new ScoreboardGroup(this, "alive");
            }

            public class ScoreboardGroup : Panel
            {
                public Label Title { set; get; }

                public ScoreboardGroup(Panel parent, string group)
                {
                    Parent = parent;

                    SetClass(group, true);

                    Title = Add.Label(group.ToUpper() + " - NUM.PLAYER");
                    Title.SetClass("groupTitle", true);

                    new ScoreboadGroupContent(this);
                }

                public class ScoreboadGroupContent : Sandbox.UI.Scoreboard<ScoreboardEntry>
                {
                    public ScoreboadGroupContent(Panel parent)
                    {
                        Parent = parent;

                        // groupPlayerList.sort()
                        // for (player in groupPlayerList)
                        // new ScoreboardEntry(player)
                    }
                }

                public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
                {
                    public Label Score { set; get; }
                    public Label Karma { set; get; }

                    public ScoreboardEntry()
                    {
                        PlayerName = Add.Label("", "playername");
                        Score = Add.Label("", "score");
                        Karma = Add.Label("", "karma");
                        Ping = Add.Label("", "ping");
                    }

                    public override void UpdateFrom(PlayerScore.Entry entry)
                    {
                        Entry = entry;

                        PlayerName.Text = entry.GetString("name"); // Does the playername need to update? In orig. you get kicked if you change ur name
                        Score.Text = entry.Get<int>("kills", 0).ToString();
                        Karma.Text = entry.Get<int>("karma", 0).ToString();
                        Ping.Text = entry.Get<int>("ping", 0).ToString();

                        SetClass("me", Local.Client != null && entry.Get<ulong>("steamid", 0) == Local.Client.SteamId);
                    }
                }
            }
        }
    }
}
