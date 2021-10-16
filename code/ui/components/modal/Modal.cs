using System;

namespace TTTReborn.UI
{
    public partial class Modal : Window
    {
        public Action<Modal> OnDisplay { get; set; }

        public Modal() : base()
        {

        }

        public virtual void Display()
        {
            Enabled = true;

            OnDisplay?.Invoke(this);
        }

        public virtual void Close(bool deleteOnClose = false)
        {
            WindowHeader.NavigationHeader.OnClose?.Invoke(WindowHeader.NavigationHeader);

            if (deleteOnClose)
            {
                Delete(true);
            }
            else
            {
                Enabled = false;
            }
        }
    }
}
