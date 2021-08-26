namespace TTTReborn.UI
{
    public class Window : Panel
    {
        public Panel Header { get; set; }
        public Panel Content { get; set; }
        public Panel Footer { get; set; }

        public Window()
        {
            StyleSheet.Load("/ui/components/generic/Window.scss");

            SetClass("color-secondary", true);

            Header = new(this);
            Header.SetClass("header", true);
            Header.SetClass("color-primary", true);

            Content = new(this);

            Footer = new(this);
            Footer.SetClass("footer", true);
            Footer.SetClass("color-primary", true);

            AddChild(Header);
            AddChild(Content);
            AddChild(Footer);
        }
    }
}
