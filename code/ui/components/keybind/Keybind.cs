using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Keybind : TextEntry
    {
        public string BoundKey { get; private set; } = "";

        public string BoundCommand { get; set; } = "";

        public Keybind() : base()
        {
            StyleSheet.Load("/ui/components/keybind/Keybind.scss");

            AcceptsFocus = true;
        }

        public override void OnButtonTyped(string button, KeyModifiers km)
        {
            base.OnButtonTyped(button, km);

            BoundKey = button;
            Text = button;
            CaretPos = button.Length;

            Blur();

            if (!string.IsNullOrEmpty(BoundCommand))
            {
                DialogBox dialogBox = new DialogBox();
                dialogBox.TitleLabel.Text = $"Bind '{BoundCommand}'";
                dialogBox.AddText($"Are you sure that you wanna bind '{BoundCommand}' to '{BoundKey}'?");
                dialogBox.OnAgree = (panel) =>
                {
                    ConsoleSystem.Run($"bind {BoundKey} {BoundCommand}");

                    panel.Close();
                };
                dialogBox.OnDecline = (panel) => panel.Close();

                FindRootPanel().AddChild(dialogBox);

                dialogBox.Display();
            }
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class KeybindEntryConstructor
    {
        public static Keybind Keybind(this PanelCreator self, string text)
        {
            Keybind control = self.panel.AddChild<Keybind>();
            control.Text = text;

            return control;
        }
    }
}
