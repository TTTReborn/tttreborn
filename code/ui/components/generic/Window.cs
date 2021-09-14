using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Window : Panel
    {
        public Panel Header { get; set; }
        public Sandbox.UI.Label WindowLabel { get; set; }
        public Sandbox.UI.Button CloseButton { get; set; }
        public Panel Content { get; set; }
        public Panel Footer { get; set; }

        public Window() : base()
        {
            AddClass("window");
            AddClass("rounded");
            AddClass("centered");
            AddClass("text-shadow");

            Header = new(this);
            Header.AddClass("header");
            Header.AddClass("rounded-top");
            Header.AddClass("background-color-secondary");

            WindowLabel = Header.Add.Label("Window");

            CloseButton = Header.Add.Button("✕");
            CloseButton.AddClass("button");
            CloseButton.AddEventListener("onclick", () =>
            {
                Enabled = false;
            });

            Content = new(this);
            Content.AddClass("background-color-primary");
            Content.AddClass("content");

            Footer = new(this);
            Footer.AddClass("footer");
            Footer.AddClass("rounded-bottom");
            Footer.AddClass("background-color-secondary");
        }
    }
}
