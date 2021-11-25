// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class PostRoundStats
    {
        public readonly string WinningRole;
        public Color WinningColor;

        public PostRoundStats(string winningRole, Color winningColor)
        {
            WinningRole = winningRole;
            WinningColor = winningColor;
        }
    }

    public class PostRoundMenu : Panel
    {
        public static PostRoundMenu Instance;

        private PostRoundStats _stats;

        private readonly Panel _backgroundBannerPanel;
        private readonly Panel _containerPanel;

        private readonly TranslationLabel _headerLabel;
        private readonly TranslationLabel _contentLabel;

        public PostRoundMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/postroundmenu/PostRoundMenu.scss");

            AddClass("text-shadow");

            _backgroundBannerPanel = new(this);
            _backgroundBannerPanel.AddClass("background-color-secondary");
            _backgroundBannerPanel.AddClass("background-banner-panel");
            _backgroundBannerPanel.AddClass("opacity-medium");

            _containerPanel = new(_backgroundBannerPanel);
            _containerPanel.AddClass("container-panel");

            _headerLabel = _containerPanel.Add.TranslationLabel("");
            _headerLabel.AddClass("header-label");

            _contentLabel = _containerPanel.Add.TranslationLabel("");
            _contentLabel.AddClass("content-label");
        }

        public void OpenAndSetPostRoundMenu(PostRoundStats stats)
        {
            _stats = stats;

            OpenPostRoundMenu();
        }

        public void ClosePostRoundMenu()
        {
            SetClass("fade-in", false);
            _containerPanel.SetClass("pop-in", false);
        }

        public void OpenPostRoundMenu()
        {
            SetClass("fade-in", true);
            _containerPanel.SetClass("pop-in", true);

            _contentLabel.SetTranslation("POST_ROUND_TEXT");

            _headerLabel.SetTranslation($"POST_ROUND_WIN_{_stats.WinningRole.ToUpper()}");
            _headerLabel.Style.FontColor = _stats.WinningColor;
        }
    }
}
