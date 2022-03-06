using TTTReborn.Globalization;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public static void Reset()
        {
            DialogBox dialogBox = new();
            dialogBox.SetTranslationTitle(new TranslationData("MENU.ROLEEDITOR.RESET.TITLE"));
            dialogBox.AddTranslation(new TranslationData("MENU.ROLEEDITOR.RESET.DESCRIPTION"));
            dialogBox.OnAgree = () =>
            {
                Log.Debug("Resetting NodeStack");

                NodeStack.ServerResetStack();

                dialogBox.Close();
            };
            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            Hud.Current.RootPanel.AddChild(dialogBox);

            dialogBox.Display();
        }
    }
}
