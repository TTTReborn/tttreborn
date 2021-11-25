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

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Gamemode;

namespace TTTReborn.UI
{
    public class GameTimerDisplay : Panel
    {
        private readonly Panel _timerPanel;
        private readonly Label _timerLabel;
        private readonly Panel _roundPanel;
        private readonly TranslationLabel _roundLabel;

        public GameTimerDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/gametimer/GameTimerDisplay.scss");

            AddClass("background-color-primary");
            AddClass("centered-horizontal");
            AddClass("opacity-heavy");
            AddClass("rounded");
            AddClass("text-shadow");

            _timerPanel = new(this);
            _timerPanel.AddClass("timer-panel");

            _timerLabel = _timerPanel.Add.Label();
            _timerLabel.AddClass("timer-label");

            _roundPanel = new(this);
            _roundPanel.AddClass("round-panel");

            _roundLabel = _roundPanel.Add.TranslationLabel();
            _roundLabel.AddClass("round-label");
            _roundLabel.AddClass("text-color-info");
        }

        public override void Tick()
        {
            base.Tick();

            if (Game.Instance.Round == null)
            {
                return;
            }

            _roundLabel.SetTranslation($"ROUND_STATE_{Game.Instance.Round.RoundName.ToUpper().Replace(' ', '_')}");

            _timerPanel.SetClass("disabled", Game.Instance.Round is Rounds.WaitingRound);
            _timerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
        }
    }
}
