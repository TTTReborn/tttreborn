using System;

namespace TTTReborn.UI
{
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay;
        public bool IsDeletedOnClose;

        public Modal(bool isDeletedOnClose = true) : base()
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
