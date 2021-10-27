using System;

namespace TTTReborn.UI
{
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay;
        public bool IsDeletedOnClose;

        public Modal(Sandbox.UI.Panel parent = null, bool isDeletedOnClose = true) : base(parent)
        {
            IsDeletedOnClose = isDeletedOnClose;

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
