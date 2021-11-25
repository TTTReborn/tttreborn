using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class TTTMenu : Panel
    {
        public string TextValue { get; set; }
        public bool CheckedValue { get; set; } = true;

        public TTTMenu()
        {
            Style.PointerEvents = "all";
        }

        public override void Tick()
        {
            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Menu))
                {
                    Enabled = !Enabled;
                }
            }
        }
    }
}