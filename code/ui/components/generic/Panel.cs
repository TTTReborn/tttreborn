namespace TTTReborn.UI
{
    public class Panel : Sandbox.UI.Panel
    {
        public virtual bool Enabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;

                SetClass("disabled", !_isEnabled);
            }
        }
        protected bool _isEnabled = true;

        public Panel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/generic/Panel.scss");

            SetClass("panel", true);
        }
    }
}
