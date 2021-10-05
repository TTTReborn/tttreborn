using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class C4Arm : Panel
    {
        public static C4Arm Instance;
        public TTTPlayer User { get; set; }
        public C4Entity Entity { get; set; }

        private const string EMPTY_TIMER = "--:--";

        private readonly Label _timer;
        private readonly Label _defuseChance;

        public static string TimerString(int time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            return span.ToString("mm\\:ss");
        }

        public C4Arm()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/c4/C4Arm.scss");

            AddClass("text-shadow");

            Panel backgroundPanel = new Panel(this);
            backgroundPanel.AddClass("background-color-secondary");
            backgroundPanel.AddClass("opacity-medium");
            backgroundPanel.AddClass("fullscreen");

            Panel contentPanel = new Panel(this);
            contentPanel.AddClass("background-color-primary");
            contentPanel.AddClass("content-container");
            contentPanel.AddClass("opacity-heavy");
            contentPanel.AddClass("rounded");

            Panel timerPanel = new Panel(contentPanel);
            timerPanel.AddClass("timer-panel");
            _timer = timerPanel.Add.Label("00:00", "timer-label");

            _defuseChance = contentPanel.Add.Label("WIP Text Please remove this.", "defuse-label");

            Panel timerButtons = new Panel(contentPanel);
            timerButtons.AddClass("timer-button-panel");
            foreach (C4Entity.C4Preset timerPreset in C4Entity.TimerPresets)
            {
                timerButtons.Add.Button(TimerString(timerPreset.Timer), "button", () =>
                {
                    Log.Info("Timer button clicked.");
                });
            }

            Panel actionButtons = new Panel(contentPanel);
            actionButtons.AddClass("action-button-panel");
            actionButtons.Add.Button("Pick Up", "button", () => { });
            actionButtons.Add.Button("Destroy", "button", () => { });
            actionButtons.Add.Button("Arm", "button", () => { });

            Enabled = true;
        }

        public void SetTimer(C4Entity.C4Preset preset)
        {
            _timer.Text = TimerString(preset.Timer);

            int wires = preset.Wires;
            int defuseChance = (1f / wires * 100f).FloorToInt();
        }
    }
}
