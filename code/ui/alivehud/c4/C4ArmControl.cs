using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.UI.Construct;

namespace TTTReborn.UI
{
    public class C4ArmControl : TTTPanel
    {
        public TTTPlayer User { get; set; }
        public C4Entity Entity { get; set; }
        public bool IsPresetSelected { get; set; } = false;

        private readonly Header _header;
        private readonly Content _content;
        private readonly Footer _footer;

        public static string TimerString(int time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            return span.ToString("mm\\:ss");
        }

        public C4ArmControl()
        {
            IsShowing = false;
            StyleSheet.Load("/ui/alivehud/c4/C4ArmControl.scss");

            _header = new(this);
            _content = new(this);
            _footer = new(this);
        }

        public void Open(C4Entity entity, TTTPlayer user)
        {
            Entity = entity;
            User = user;

            Open();
        }

        public void SetTimer(C4Preset preset)
        {
            var timerString = TimerString(preset.timer);

            Entity.UpdateTimerDisplay(timerString);
            _content.UpdateTimer(timerString);

            int wires = preset.wires;
            int defuseChance = (1f / wires * 100f).FloorToInt();

            _content.UpdatePresetInfo($"{defuseChance}% chance to defuse");
        }

        public override void Tick()
        {
            if (IsShowing && User != null && Entity != null)
            {
                // If the player moves away from the bomb or the bomb becomes armed, close the UI
                if (Vector3.DistanceBetween(User.Position, Entity.Position) > 100 || Entity.IsArmed)
                {
                    Close();
                }
            }

            base.Tick();
        }

        private class Header : TTTPanel
        {
            private Label _title;
            private Button _closeButton;

            public Header(Panel parent)
            {
                Parent = parent;

                _title = Add.Label("C4 Control", "title");
                _closeButton = Add.Button("Close", () => { ((C4ArmControl) Parent).Close(); });
            }
        }

        private class Content : TTTPanel
        {
            private Display _display;

            public Content(Panel parent)
            {
                Parent = parent;

                _display = new(this);
            }

            public void UpdateTimer(string timerString)
            {
                _display.UpdateTimer(timerString);
            }

            public void UpdatePresetInfo(string infoString)
            {
                _display.UpdatePresetInfo(infoString);
            }

            public class Display : TTTPanel
            {
                private Label _timer;
                private Presets _presets;

                public Display(Panel parent)
                {
                    Parent = parent;

                    _timer = Add.Label("00:00", "timer");
                    _presets = new(this);
                }

                public void UpdateTimer(string timerString)
                {
                    _timer.Text = timerString;
                }

                public void UpdatePresetInfo(string infoString)
                {
                    _presets.UpdatePresetInfo(infoString);
                }

                public class Presets : TTTPanel
                {
                    private PresetButtons _presetButtons;
                    private Label _presetInfo;

                    public Presets(Panel parent)
                    {
                        Parent = parent;

                        _presetInfo = Add.Label("Select a timer preset:");
                        _presetButtons = new(this);
                    }

                    public void UpdatePresetInfo(string infoString)
                    {
                        _presetInfo.Text = infoString;
                    }

                    public class PresetButtons : TTTPanel
                    {
                        private List<Button> _presetButtons;

                        public PresetButtons(Panel parent)
                        {
                            Parent = parent;

                            var controls = (C4ArmControl) Parent.Parent.Parent.Parent;

                            _presetButtons = new List<Button>();

                            for (int i = 0; i < C4Entity.TimerPresets.Length; i++)
                            {
                                var preset = C4Entity.TimerPresets[i];
                                int presetIndex = i; // This hack is required to set preset in the for loop

                                var button = Add.Button(TimerString(preset.timer));
                                button.AddEventListener("onclick", () =>
                                {
                                    C4Entity.SetPreset(controls.Entity.NetworkIdent, presetIndex);

                                    controls.SetTimer(preset);
                                    controls.IsPresetSelected = true;
                                });

                                _presetButtons.Add(button);
                            }
                        }
                    }
                }
            }
        }

        public class Footer : TTTPanel
        {
            private Button _pickUpButton;
            private Button _destroyButton;
            private Button _armButton;

            public Footer(Panel parent)
            {
                Parent = parent;

                _pickUpButton = Add.Button("Pick Up", "pickUpButton");
                _destroyButton = Add.Button("Destroy", "destroyButton");
                _armButton = Add.Button("Arm", "armButton");

                var armControl = (C4ArmControl) Parent;

                _pickUpButton.AddEventListener("onclick", () =>
                {
                    C4Entity.PickUp(armControl.Entity.NetworkIdent, armControl.User.NetworkIdent);
                    armControl.Close();
                });

                _destroyButton.AddEventListener("onclick", () =>
                {
                    C4Entity.Delete(armControl.Entity.NetworkIdent);
                    armControl.Close();
                });

                _armButton.AddEventListener("onclick", () =>
                {
                    if (armControl.IsPresetSelected)
                    {
                        C4Entity.Arm(armControl.Entity.NetworkIdent);
                        armControl.Close();
                    }
                });

            }
        }
    }
}
