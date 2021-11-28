using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Reset()
        {
            Log.Debug("Resetting NodeStack");

            NodeStack.ServerResetStack();
        }
    }
}
