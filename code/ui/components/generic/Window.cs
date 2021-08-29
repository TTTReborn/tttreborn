namespace TTTReborn.UI
{
    public class Window : Panel
    {
        public Panel Header { get; set; }
        public Panel Content { get; set; }
        public Panel Footer { get; set; }

        public Window() : base()
        {
            StyleSheet.Load("/ui/components/generic/Window.scss");

            SetClass("rounded", true);
            SetClass("window", true);
            Style.BackgroundColor = ColorScheme.Primary;

            Header = new(this);
            Header.SetClass("header", true);
            Header.SetClass("rounded-top", true);
            Header.Style.BackgroundColor = ColorScheme.Secondary;

            Content = new(this);

            Footer = new(this);
            Footer.SetClass("footer", true);
            Footer.SetClass("rounded-bottom", true);
            Footer.Style.BackgroundColor = ColorScheme.Secondary;

            AddChild(Header);
            AddChild(Content);
            AddChild(Footer);
        }
    }
}
