using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public struct InspectIconData
    {
        // If the icon is unique to the corpse, the UI element will get deleted
        // on each corpse inspect.
        public bool IsUnique;
        // Image path to the icon
        public string ImagePath;
        // Description displayed when the icon is clicked
        public string Description;
    }

    public class InspectIcon : TTTPanel
    {
        public InspectIconData IconData;

        public InspectIcon(Panel parent, InspectIconData data)
        {
            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectIcon.scss");
            Parent = parent;
            Add.Image(data.ImagePath, "image");
            IconData = data;
        }
    }
}
