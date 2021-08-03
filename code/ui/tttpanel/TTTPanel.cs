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

        public void AddChild(Panel child, int index)
        {
            AddChild(this);

            Panel tmp = _children[index];
            _children[index] = this;

            for (++index; index < ChildCount; index++)
            {
                _children[index] = tmp;
                tmp = _children[index + 1];
            }

            _children[index] = tmp;
            _children[++index] = null;
        }
    }
}
