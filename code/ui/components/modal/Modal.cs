using System;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Modal : TTTPanel
    {
        public Action<Modal> OnDisplay { get; set; }
        public Action<Modal> OnClose { get; set; }

        public Modal(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;
        }

        public void Display()
        {
            IsShowing = true;

            OnDisplay?.Invoke(this);
        }

        public void Close()
        {
            OnClose?.Invoke(this);

            IsShowing = false;
        }
    }
}
