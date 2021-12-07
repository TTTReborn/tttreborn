namespace TTTReborn.UI
{
    public class Popup : Sandbox.UI.Popup
    {
        public Popup(Sandbox.UI.Panel sourcePanel, PositionMode position, float offset) : base(sourcePanel, position, offset) { }

        public override void Tick()
        {
            base.Tick();

            if (!PopupSource?.IsVisible ?? true)
            {
                Success();
            }
        }
    }
}