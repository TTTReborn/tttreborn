using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class C4Arm : Panel
    {
        public static C4Arm Instance;
        public TTTPlayer User { get; set; }
        public C4Entity Entity { get; set; }

        private int _selectedPresetIndex;
        private readonly Label _timer;
        private readonly Label _defuseChance;

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
            _defuseChance = contentPanel.Add.Label("", "defuse-label");

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

            actionButtons.Add.Button("Pick Up", "button action-button", () =>
            {
                C4Entity.PickUp(Entity.NetworkIdent, User.NetworkIdent);
                Enabled = false;
            });

            actionButtons.Add.Button("Destroy", "button action-button", () =>
            {
                C4Entity.Delete(Entity.NetworkIdent);
                Enabled = false;
            });

            actionButtons.Add.Button("Arm", "button arm-button", () =>
            {
                C4Entity.Arm(Entity.NetworkIdent, _selectedPresetIndex);
                Enabled = false;
            });

            Enabled = false;
        }

        public void Open(C4Entity entity, TTTPlayer user)
        {
            Entity = entity;
            User = user;
            Enabled = true;
        }

        private void SetTimer(int presetIndex)
        {
            _selectedPresetIndex = presetIndex;

            C4Entity.C4Preset preset = C4Entity.TimerPresets[_selectedPresetIndex];

            _timer.Text = Utils.TimerString(preset.Timer);

            int defuseChance = (1f / preset.Wires * 100f).FloorToInt();
            _defuseChance.Text = $"{defuseChance}% chance to defuse";
        }
    }
}
