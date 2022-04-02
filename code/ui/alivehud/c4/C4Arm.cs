using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Items;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public class C4Arm : Panel
    {
        public static C4Arm Instance;
        public C4Entity Entity { get; set; }

        private int _selectedPresetIndex;
        private string TimerText { get; set; }
        private TranslationLabel DefuseChance { get; set; }
        private Panel TimerButtonPanel { get; set; }
        private Panel ActionButtonPanel { get; set; }

        public C4Arm() : base()
        {
            Instance = this;
            TimerText = "00:00";

            for (int i = 0; i < C4Entity.TimerPresets.Length; i++)
            {
                int selectedPresetIndex = i;

                if (i == 0)
                {
                    SetTimer(selectedPresetIndex);
                }

                TimerButtonPanel.Add.Button(Utils.TimerString(C4Entity.TimerPresets[selectedPresetIndex].Timer), "button", () =>
                {
                    SetTimer(selectedPresetIndex);
                });
            }

            ActionButtonPanel.Add.TranslationButton(new TranslationData("EQUIPMENT.C4.UI.PICKUP"), null, "button action-button", () =>
            {
                C4Entity.PickUp(Entity.NetworkIdent, Local.Pawn.NetworkIdent);
            });

            ActionButtonPanel.Add.TranslationButton(new TranslationData("EQUIPMENT.C4.UI.DESTROY"), null, "button action-button", () =>
            {
                C4Entity.Delete(Entity.NetworkIdent);
            });

            ActionButtonPanel.Add.TranslationButton(new TranslationData("EQUIPMENT.C4.UI.ARM"), null, "button arm-button", () =>
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
            TimerText = Utils.TimerString(preset.Timer);

            DefuseChance.UpdateTranslation(new TranslationData("EQUIPMENT.C4.UI.DEFUSECHANCE", (1f / preset.Wires * 1000f).FloorToInt() / 10f));
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
