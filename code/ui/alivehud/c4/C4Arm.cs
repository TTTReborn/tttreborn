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

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class C4Arm : Panel
    {
        public static C4Arm Instance;
        public C4Entity Entity { get; set; }

        private int _selectedPresetIndex;
        private readonly Label _timer;
        private readonly TranslationLabel _defuseChance;

        public C4Arm() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/c4/C4Arm.scss");

            AddClass("text-shadow");

            Panel backgroundPanel = new(this);
            backgroundPanel.AddClass("background-color-secondary");
            backgroundPanel.AddClass("opacity-medium");
            backgroundPanel.AddClass("fullscreen");

            Panel contentPanel = new(this);
            contentPanel.AddClass("content-container");

            Panel timerPanel = new(contentPanel);
            timerPanel.AddClass("timer-panel");
            timerPanel.AddClass("opacity-heavy");

            _timer = timerPanel.Add.Label("00:00", "timer-label");
            _defuseChance = contentPanel.Add.TranslationLabel("", "defuse-label");

            Panel timerButtons = new Panel(contentPanel);
            timerButtons.AddClass("timer-button-panel");

            for (int i = 0; i < C4Entity.TimerPresets.Length; i++)
            {
                int selectedPresetIndex = i;

                if (i == 0)
                {
                    SetTimer(selectedPresetIndex);
                }

                timerButtons.Add.Button(Utils.TimerString(C4Entity.TimerPresets[selectedPresetIndex].Timer), "button", () =>
                {
                    SetTimer(selectedPresetIndex);
                });
            }

            Panel actionButtons = new Panel(contentPanel);
            actionButtons.AddClass("action-button-panel");

            actionButtons.Add.TranslationButton("C4_UI_PICKUP", "button action-button", () =>
            {
                C4Entity.PickUp(Entity.NetworkIdent, Local.Pawn.NetworkIdent);
            });

            actionButtons.Add.TranslationButton("C4_UI_DESTROY", "button action-button", () =>
            {
                C4Entity.Delete(Entity.NetworkIdent);
            });

            actionButtons.Add.TranslationButton("C4_UI_ARM", "button arm-button", () =>
            {
                C4Entity.Arm(Entity.NetworkIdent, _selectedPresetIndex);
            });

            Enabled = false;
        }

        public void Open(C4Entity entity)
        {
            Entity = entity;
            Enabled = true;
        }

        private void SetTimer(int presetIndex)
        {
            _selectedPresetIndex = presetIndex;

            C4Entity.C4Preset preset = C4Entity.TimerPresets[_selectedPresetIndex];

            _timer.Text = Utils.TimerString(preset.Timer);

            _defuseChance.SetTranslation("C4_UI_DEFUSECHANCE", (1f / preset.Wires * 1000f).FloorToInt() / 10f);
        }

        public override void Tick()
        {
            if (Enabled && Entity?.Transform.Position.Distance(Local.Pawn.Owner.Position) > 100f)
            {
                Enabled = false;
            }

            base.Tick();
        }
    }
}
