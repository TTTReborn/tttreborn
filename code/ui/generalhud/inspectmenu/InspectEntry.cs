using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InspectEntry : Panel
    {
        public string ImagePath;
        public string Description;

        private readonly Image _inspectIcon;
        private readonly Label _inspectQuickLabel;

        public InspectEntry(Panel parent) : base(parent)
        {
            Parent = parent;

            AddClass("rounded");
            AddClass("text-shadow");
            AddClass("background-color-secondary");

            _inspectIcon = Add.Image();
            _inspectIcon.AddClass("inspect-icon");

            _inspectQuickLabel = Add.Label();
            _inspectQuickLabel.AddClass("quick-label");
        }

        public void SetData(string imagePath, string description)
        {
            ImagePath = imagePath;
            Description = description;

            _inspectIcon.Style.BackgroundImage = Texture.Load(imagePath, false) ?? Texture.Load($"/ui/none.png");
            _inspectIcon.Style.Dirty();
        }

        public void SetQuickInfo(string quickString)
        {
            _inspectQuickLabel.Text = quickString;
        }
    }
}
