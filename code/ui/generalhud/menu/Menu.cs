using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class Menu : Panel
    {
        public static Menu Instance;

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                SetClass("hide", !_isShowing);
            }
        }

        private bool _isShowing = false;

        private KeybindEntry _keybindEntry;

        public Menu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _keybindEntry = Add.KeybindEntry("Press a button");

            SetClass("hide", true);
        }
    }
}

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ClientCmd(Name = "ttt_keybinds")]
        public static void KeybindsMenu()
        {
            UI.Menu.Instance.IsShowing = !UI.Menu.Instance.IsShowing;
        }
    }
}

namespace Sandbox.UI
{
    public partial class KeybindEntry : TextEntry
    {
        public string BoundKey { get; private set; } = "";

        public string BoundCommand { get; set; } = "";

        public KeybindEntry() : base()
        {
            AcceptsFocus = true;
        }

        public override void OnButtonTyped(string button, KeyModifiers km)
        {
            base.OnButtonTyped(button, km);

            BoundKey = button;
            Text = button;
            CaretPos = button.Length;

            //Log.Error(ConsoleSystem.Run($"bind {BoundKey}").Value ?? ""); // This does not work as ConsoleResult's data always is null

            if (!string.IsNullOrEmpty(BoundCommand))
            {
                //ConsoleSystem.Run($"bind {BoundKey} {BoundCommand}"); // we don't wanna override already set keys, so we have to wait until we are able to access console output or bound keys
            }

            Blur();
        }
    }

    namespace Construct
    {
        public static class KeybindEntryConstructor
        {
            public static KeybindEntry KeybindEntry(this PanelCreator self, string text)
            {
                KeybindEntry control = self.panel.AddChild<KeybindEntry>();
                control.Text = text;

                return control;
            }
        }
    }
}
