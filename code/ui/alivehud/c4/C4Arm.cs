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
            contentPanel.AddClass("content-container");

            Panel timerPanel = new Panel(contentPanel);
            timerPanel.AddClass("timer-panel");
            timerPanel.AddClass("opacity-heavy");
            _timer = timerPanel.Add.Label("00:00", "timer-label");

            _defuseChance = contentPanel.Add.Label("", "defuse-label");

            Panel timerButtons = new Panel(contentPanel);
            timerButtons.AddClass("timer-button-panel");
            for (int i = 0; i < C4Entity.TimerPresets.Length; ++i)
            {
                C4Entity.C4Preset timerPreset = C4Entity.TimerPresets[i];
                if (i == 0)
                {
                    SetTimer(timerPreset);
                }

                timerButtons.Add.Button(TimerString(timerPreset.Timer), "button", () =>
                {
                    SetTimer(timerPreset);
                });
            }

            Panel actionButtons = new Panel(contentPanel);
            actionButtons.AddClass("action-button-panel");
            actionButtons.Add.Button("Pick Up", "button", () =>
            {
                C4Entity.PickUp(Entity.NetworkIdent, User.NetworkIdent);
            });

            actionButtons.Add.Button("Destroy", "button", () =>
            {
                C4Entity.Delete(Entity.NetworkIdent);
                Enabled = false;
            });

            actionButtons.Add.Button("Arm", "button arm-button", () =>
            {
                C4Entity.Arm(Entity.NetworkIdent);
                Enabled = false;
            });
        }

        public void Open(C4Entity entity, TTTPlayer user)
        {
            Entity = entity;
            User = user;
            Enabled = true;
        }

        private void SetTimer(C4Entity.C4Preset preset)
        {
            _timer.Text = TimerString(preset.Timer);

            int wires = preset.Wires;
            int defuseChance = (1f / wires * 100f).FloorToInt();
            _defuseChance.Text = $"{defuseChance}% chance to defuse";
        }
    }
}
