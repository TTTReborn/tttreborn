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
    public class C4Menu : TTTPanel
    {
        public static C4Menu Instance;

        private C4ControlPanel ControlPanel;
        private static C4Entity Entity;

        public C4Menu()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/alivehud/c4/C4Menu.scss");

            ControlPanel = new C4ControlPanel(this);
        }

        public void Open(C4State state, C4Entity entity)
        {
            IsShowing = true;
            Entity = entity;
            // ControlPanel.ShowState(state); //Implement this
        }

        public void Close()
        {
            IsShowing = false;
        }

        private class C4ControlPanel : TTTPanel
        {
            private readonly ArmScreen _ArmScreen;
            private readonly DisarmScreen _DisarmScreen;

            public C4ControlPanel(Panel parent)
            {
                Parent = parent;

                _ArmScreen = new ArmScreen(this);
                _DisarmScreen = new DisarmScreen(this);

                IsShowing = true;
            }

            public void ShowState(C4State state)
            {
                if (state == C4State.Unarmed)
                {
                    _ArmScreen.IsShowing = true;
                }
                else if (state == C4State.Armed)
                {
                    _DisarmScreen.IsShowing = true;
                }
                else if (state == C4State.Disarmed)
                {
                    _DisarmScreen.IsShowing = true;
                }
            }

            //Start of arming panels
            private class ArmScreen : TTTPanel
            {
                private readonly TimerSection _TimerSection;

                public ArmScreen(Panel parent)
                {
                    Parent = parent;
                    IsShowing = true;
                    _TimerSection = new TimerSection(this);
                }

                private class TimerSection : TTTPanel
                {
                    private readonly Panel TimeStuff;
                    private readonly Label _TimePreview;
                    private readonly Label _TimeTillD;
                    private readonly Slider _TimeSlider;
                    private readonly Label _WireHint;

                    private readonly Panel _RemovalSection;
                    private readonly Button _Pickup;
                    private readonly Button _Destroy;
                    private readonly Button _Arm;
                    private readonly Button _Cancel;

                    public TimerSection(Panel parent)
                    {
                        Parent = parent;
                        IsShowing = true;
                        TimeStuff = Add.Panel("timestuff");
                        _TimeTillD = TimeStuff.Add.Label("Time until detonation:");
                        _TimePreview = TimeStuff.Add.Label("00:00");
                        _TimeSlider = Add.Slider(45, 600, 15);
                        _TimeSlider.Value = _TimeSlider.MinValue;
                        _WireHint = Add.Label("In disarm attempts, X out of 6 wires will cause instant detonation when cut.");

                        _RemovalSection = Add.Panel("RemovalSection");

                        _Arm = _RemovalSection.Add.Button("Arm C4", () =>
                        {

                        });

                        _Pickup = _RemovalSection.Add.Button("Pick up C4", () =>
                        {

                        });

                        _Destroy = _RemovalSection.Add.Button("Destroy C4", () =>
                        {

                        });

                        _Cancel = _RemovalSection.Add.Button("Cancel", () =>
                        {
                            Instance.Close();
                        }

                        );
                    }

                    public override void Tick()
                    {
                        base.Tick();
                        _TimePreview.Text = TimeSpan.FromSeconds(_TimeSlider.Value).ToString(@"mm\:ss");
                        var badWires = Math.Floor(_TimeSlider.Value / 60);
                        badWires += 1;
                        if (badWires >= 6)
                        {
                            badWires = 5;
                        }
                        _WireHint.Text = $"In disarm attempts, {badWires} out of 6 wires will cause instant detonation when cut.";
                    }
                }


            } //End of ArmScreen panel

            private class DisarmScreen : TTTPanel
            {

                public DisarmScreen(Panel parent)
                {
                    Parent = parent;
                    IsShowing = false;
                }

            }

        } //End of C4Control panel


    }
}
