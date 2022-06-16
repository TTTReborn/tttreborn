using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Teams;

namespace TTTReborn.UI
{
    public class PostRoundStats
    {
        public readonly string WinningTeam;
        public Color WinningColor;

        public PostRoundStats(string winningTeam, Color winningColor)
        {
            WinningTeam = winningTeam;
            WinningColor = winningColor;
        }
    }

    [UseTemplate]
    public class PostRoundMenu : Panel
    {
        public static PostRoundMenu Instance { get; set; }

        private PostRoundStats _stats;

        private Panel ContainerPanel { get; set; }

        private TranslationLabel HeaderLabel { get; set; }
        private TranslationLabel ContentLabel { get; set; }

        public PostRoundMenu()
        {
            Instance = this;
        }

        public void OpenAndSetPostRoundMenu(PostRoundStats stats)
        {
            _stats = stats;

            OpenPostRoundMenu();
        }

        public void ClosePostRoundMenu()
        {
            SetClass("fade-in", false);
            ContainerPanel.SetClass("pop-in", false);
        }

        public void OpenPostRoundMenu()
        {
            SetClass("fade-in", true);
            ContainerPanel.SetClass("pop-in", true);

            ContentLabel.UpdateTranslation(new TranslationData("WIN.DESCRIPTION"));

            HeaderLabel.UpdateTranslation(new TranslationData(Utils.GetTranslationKey(_stats.WinningTeam, "WIN")));
            HeaderLabel.Style.FontColor = _stats.WinningColor;
        }

        [Event("game_finish")]
        public static void OnFinishRound(Team team)
        {
            Instance.OpenAndSetPostRoundMenu(new PostRoundStats(
                winningTeam: team.Name,
                winningColor: team.Color
            ));
        }
    }
}
