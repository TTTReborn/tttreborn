namespace TTTReborn.UI
{
    public class Window : RichPanel
    {
        public WindowHeader Header;
        public PanelContent Content;
        public Panel Footer;

        public Window(Sandbox.UI.Panel parent = null) : base(parent)
        {
            AddClass("window");
            AddClass("rounded");
            AddClass("text-shadow");

            Header = new(this);
            Header.AddClass("header");
            Header.AddClass("rounded-top");
            Header.AddClass("background-color-secondary");

            Content = new(this);
            Content.AddClass("content");
            Content.AddClass("background-color-primary");

            Footer = new(this);
            Footer.AddClass("footer");
            Footer.AddClass("rounded-bottom");
            Footer.AddClass("background-color-secondary");

            IsDraggable = false;
        }

        public void SetTitle(string title)
        {
            Header.NavigationHeader.SetTitle(title);
        }

        public void SetTranslationTitle(string translationKey, params object[] translationData)
        {
            Header.NavigationHeader.SetTranslationTitle(translationKey, translationData);
        }
    }
}
