using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Keybind : TextEntry
    {
        public string BoundKey { get; private set; } = "";

        public string BoundCommand { get; set; } = "";

        public string DefaultText { get; set; } = "";

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
                dialogBox.OnAgree = () =>
                {
                    ConsoleSystem.Run($"bind {BoundKey} {BoundCommand}");

                    dialogBox.Close();
                };
                dialogBox.OnDecline = () =>
                {
                    BoundKey = "";
                    Text = DefaultText;
                    CaretPos = Text.Length;

                    dialogBox.Close();
                };

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
            Keybind keybind = self.panel.AddChild<Keybind>();
            keybind.DefaultText = text;
            keybind.Text = text;

            return keybind;
        }
    }
}
