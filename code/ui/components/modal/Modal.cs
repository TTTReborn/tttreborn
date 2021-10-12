using System;

namespace TTTReborn.UI
{
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay { get; set; }
        public bool IsDeletedOnClose { get; set; } = true;

        public Modal(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Action<PanelHeader> action = Header.NavigationHeader.OnClose;

            Header.NavigationHeader.OnClose = (panelHeader) =>
            {
                action?.Invoke(panelHeader);

                if (IsDeletedOnClose)
                {
                    Delete(true);
                }
            };

            Enabled = false;
        }

        public virtual void Display()
        {
            OnDisplay?.Invoke(this);

            Enabled = true;
        }

        public virtual void Close()
        {
            Enabled = false;

            Header.NavigationHeader.OnClose?.Invoke(Header.NavigationHeader);
        }
    }
}
