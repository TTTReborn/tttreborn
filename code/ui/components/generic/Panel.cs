namespace TTTReborn.UI
{
    public class Panel : Sandbox.UI.Panel
    {
        public bool Enabled
        {
            get => IsEnabled;
            set
            {
                IsEnabled = value;

                SetClass("disabled", !IsEnabled);
            }
        }
        protected bool IsEnabled = true;

        public Panel(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/generic/Panel.scss");

            SetClass("panel", true);
        }
    }
}
