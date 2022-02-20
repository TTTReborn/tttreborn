using Sandbox.UI;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ComponentTestingPage : Panel
    {
        public bool CheckedValue { get; set; } = true;
        public float FloatValue { get; set; } = 0f;
        public float SliderValue { get; set; } = 0f;
        public string TextValue { get; set; } = "";
    }
}
