namespace TTTReborn.UI
{
    public class Panel : Sandbox.UI.Panel
    {
        private bool _isShowing = true;
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                SetClass("hidden", !_isShowing);
            }
        }

        public Panel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/generic/Generic.scss");
        }
    }
}
