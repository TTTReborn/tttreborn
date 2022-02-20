using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
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
            _defuseChance = contentPanel.Add.TranslationLabel(new TranslationData(), "defuse-label");

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

            actionButtons.Add.TranslationButton(new TranslationData("C4_UI_PICKUP"), null, "button action-button", () =>
            {
                C4Entity.PickUp(Entity.NetworkIdent, Local.Pawn.NetworkIdent);
            });

            actionButtons.Add.TranslationButton(new TranslationData("C4_UI_DESTROY"), null, "button action-button", () =>
            {
                C4Entity.Delete(Entity.NetworkIdent);
            });

            actionButtons.Add.TranslationButton(new TranslationData("C4_UI_ARM"), null, "button arm-button", () =>
            {
                C4Entity.Arm(Entity.NetworkIdent, _selectedPresetIndex);
            });

            this.Enabled(false);
        }

        public void Open(C4Entity entity)
        {
            Entity = entity;
            this.Enabled(true);
        }

        private void SetTimer(int presetIndex)
        {
            _selectedPresetIndex = presetIndex;

            C4Entity.C4Preset preset = C4Entity.TimerPresets[_selectedPresetIndex];

            _timer.Text = Utils.TimerString(preset.Timer);

            _defuseChance.UpdateTranslation(new TranslationData("C4_UI_DEFUSECHANCE", (1f / preset.Wires * 1000f).FloorToInt() / 10f));
        }

        public override void Tick()
        {
            if (this.IsEnabled() && Entity?.Transform.Position.Distance(Local.Pawn.Owner.Position) > 100f)
            {
                this.Enabled(false);
            }

            base.Tick();
        }
    }
}
