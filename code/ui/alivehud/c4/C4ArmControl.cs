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

        private readonly Header _header;
        private readonly Content _content;

        public C4ArmControl()
        {
            IsShowing = false;
            StyleSheet.Load("/ui/alivehud/c4/C4ArmControl.scss");

            _header = new(this);
            _content = new(this);
        }

        public void Open(C4Entity entity, TTTPlayer user)
        {
            Entity = entity;
            User = user;

            Open();
        }

        private class Header : TTTPanel
        {
            private Label _title;
            private Button _closeButton;

            public Header(Panel parent)
            {
                Parent = parent;

                _title = Add.Label("C4 Controls", "title");
                _closeButton = Add.Button("Close", () => { ((C4ArmControl)Parent).Close(); });
            }
        }

        private class Content : TTTPanel
        {
            private Display _display;
            private Controls _controls;

            public Content(Panel parent)
            {
                Parent = parent;

                _display = new(this);
                _controls = new(this);
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

                public class Presets : TTTPanel
                {
                    private PresetButtons _presetButtons;
                    private Label _presetInfo;

                    public Presets(Panel parent)
                    {
                        Parent = parent;

                        _presetButtons = new(this);
                        _presetInfo = Add.Label("X wires, Y% defuse chance");
                    }

                    public class PresetButtons : TTTPanel
                    {
                        private Button _presetShort;
                        private Button _presetMedium;
                        private Button _presetLong;

                        public PresetButtons(Panel parent)
                        {
                            Parent = parent;

                            _presetShort = Add.Button("Short");
                            _presetMedium = Add.Button("Medium");
                            _presetLong = Add.Button("Long");
                        }
                    }
                }
            }

            public class Controls : TTTPanel
            {
                private Button _pickUpButton;
                private Button _destroyButton;
                private Button _armButton;

                public Controls(Panel parent)
                {
                    Parent = parent;

                    _pickUpButton = Add.Button("Pick Up", "pickUpButton");
                    _destroyButton = Add.Button("Destroy", "destroyButton");
                    _armButton = Add.Button("Arm", "armButton");

                    var armControl = (C4ArmControl) Parent.Parent;

                    _pickUpButton.AddEventListener("onclick", () =>
                    {
                        // Pick up the item
                        armControl.Close();
                    });

                    _destroyButton.AddEventListener("onclick", () =>
                    {
                        // Destroy the bomb
                        armControl.Close();
                    });

                    _armButton.AddEventListener("onclick", () =>
                    {
                        // Arm the bomb here
                        armControl.Close();
                    });

                }
            }
        }
    }
}
