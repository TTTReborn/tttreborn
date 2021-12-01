using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Reset()
        {
            DialogBox dialogBox = new();
            dialogBox.SetTranslationTitle(new Globalization.TranslationData("MENU_VISUALPROGRAMMING_RESET"));
            dialogBox.AddTranslation(new Globalization.TranslationData("MENU_VISUALPROGRAMMING_RESET_TEXT"));
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
