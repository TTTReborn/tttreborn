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
        private bool _isShowing = true;

        public TTTPanel(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            IsShowing = true;
        }

        public void AddChild(Panel child, int index)
        {
            AddChild(child);

            // int childCount = ChildCount;

            // if (childCount > 1)
            // {
            //     if (index < 0 || index >= childCount)
            //     {
            //         return;
            //     }

            //     Panel tmp = _children[index];
            //     _children[index++] = this;

            //     while (index < childCount)
            //     {
            //         _children[index] = tmp;
            //         tmp = _children[++index];
            //     }

            //     _children[index] = tmp;
            // }
        }
    }
}
