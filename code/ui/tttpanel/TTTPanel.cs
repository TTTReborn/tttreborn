using Sandbox.UI;

namespace TTTReborn.UI
{
    public class TTTPanel : Panel
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
        private bool _isShowing = false;

        public TTTPanel(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            IsShowing = false;
        }
    }
}
