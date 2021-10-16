namespace TTTReborn.UI
{
    public class Window : RichPanel
    {
        public WindowHeader WindowHeader { get; set; }
        public PanelContent WindowContent { get; set; }
        public Panel WindowFooter { get; set; }

        public Window() : base()
        {
            AddClass("window");
            AddClass("rounded");
            AddClass("text-shadow");

            WindowHeader = new(this);
            WindowHeader.AddClass("rounded-top");
            WindowHeader.AddClass("background-color-secondary");

            WindowContent = new(this);
            WindowContent.AddClass("background-color-primary");
            WindowContent.AddClass("content");

            WindowFooter = new(this);
            WindowFooter.AddClass("footer");
            WindowFooter.AddClass("rounded-bottom");
            WindowFooter.AddClass("background-color-secondary");

            IsDraggable = true;
        }
    }
}
