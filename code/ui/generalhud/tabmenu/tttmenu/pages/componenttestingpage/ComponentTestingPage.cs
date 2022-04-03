using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ComponentTestingPage : Panel
    {
        public bool CheckedValue { get; set; } = true;
        public float FloatValue { get; set; } = 0f;
        public float SliderValue { get; set; } = 0f;
        public string TextValue { get; set; } = "";

        private FileSelection _fileSelection;

        public void CreateFileSelection()
        {
            _fileSelection?.Close();

            FileSelection fileSelection = Hud.Current.GeneralHudPanel.FindPopupPanel().Add.FileSelection();
            fileSelection.DefaultSelectionFileType = "*";
            fileSelection.OnAgree = () => fileSelection.Close();
            fileSelection.IsDataFolder = false;
            fileSelection.Display();

            _fileSelection = fileSelection;
        }
    }
}
