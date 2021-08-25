namespace TTTReborn.UI
{
    public class Panel : Sandbox.UI.Panel
    {
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                SetClass("hide", !_isShowing);
            }
        }
        private bool _isShowing = true;

        public Panel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            IsShowing = true;
        }
    }
}
