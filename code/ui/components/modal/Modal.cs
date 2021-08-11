using System;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Modal : RichPanel
    {
        public Action<Modal> OnDisplay { get; set; }
        public Action<Modal> OnClose { get; set; }

        public Modal() : base()
        {

        }

        public virtual void Display()
        {
            IsShowing = true;

            OnDisplay?.Invoke(this);
        }

        public virtual void Close()
        {
            OnClose?.Invoke(this);

            IsShowing = false;
        }
    }
}
