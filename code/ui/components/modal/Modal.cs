using System;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay;
        public bool IsDeletedOnClose;

        public Modal(Panel parent = null, bool isDeletedOnClose = true) : base(parent)
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

            Content.Style.FlexDirection = Sandbox.UI.FlexDirection.Column;

            this.Enabled(false);
        }

        public virtual void Display()
        {
            OnDisplay?.Invoke(this);

            this.Enabled(true);
        }

        public virtual void Close()
        {
            this.Enabled(false);

            Header.NavigationHeader.OnClose?.Invoke(Header.NavigationHeader);
        }
    }
}
