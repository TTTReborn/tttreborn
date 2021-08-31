using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InspectIconData
    {
        // Determines if the icon exist on every single corpse. If the icon is unique, the icon
        // needs to be deleted/created on each corpse inspection
        public readonly bool IsUnique;
        // Image path to the icon
        public readonly string ImagePath;
        // Description displayed when the icon is clicked
        public string Description;

        public InspectIconData(bool isUnique, string imagePath, string description)
        {
            IsUnique = isUnique;
            ImagePath = imagePath;
            Description = description;
        }
    }

    public class InspectIcon : TTTPanel
    {
        public readonly InspectIconData IconData;

        public InspectIcon(Panel parent, InspectIconData data)
        {
            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectIcon.scss");
            Parent = parent;
            Add.Image(data.ImagePath, "image");
            IconData = data;
        }
    }
}
