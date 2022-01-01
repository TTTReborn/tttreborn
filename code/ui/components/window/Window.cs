using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class Window : RichPanel
    {
        public WindowHeader Header;
        public PanelContent Content;
        public Panel Footer;

        public Window(Panel parent = null) : base(parent)
        {
            AddClass("panel");
            AddClass("window");
            AddClass("rounded");
            AddClass("text-shadow");

            Header = new(this);
            Header.AddClass("header");
            Header.AddClass("rounded-top");
            Header.AddClass("background-color-primary");

            Content = new(this);
            Content.AddClass("content");
            Content.AddClass("background-color-primary");

            Footer = new(this);
            Footer.AddClass("footer");
            Footer.AddClass("rounded-bottom");
            Footer.AddClass("background-color-primary");

            IsDraggable = false;
        }

        public void SetTitle(string title)
        {
            Header.NavigationHeader.SetTitle(title);
        }

        public void SetTranslationTitle(TranslationData translationData)
        {
            Header.NavigationHeader.SetTranslationTitle(translationData);
        }
    }
}
