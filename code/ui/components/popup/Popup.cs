namespace TTTReborn.UI
{
    public class Popup : Sandbox.UI.Popup
    {
        public Popup(Sandbox.UI.Panel sourcePanel, PositionMode position, float offset) : base(sourcePanel, position, offset) { }

        public override void Tick()
        {
            base.Tick();

            // Close any popups if the parent panel is closed.
            if (!PopupSource?.IsVisible ?? true)
            {
                CloseAll();
            }
        }
    }
}