using System.Text.Json;

using Sandbox;

using TTTReborn.UI.VisualProgramming;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        [ServerCmd]
        public static void RequestStack()
        {
            if (ConsoleSystem.Caller == null)
            {
                return;
            }

            To to = To.Single(ConsoleSystem.Caller);

            if (!ConsoleSystem.Caller.HasPermission("visualprogramming"))
            {
                InitializeNodesFromStack(to, false);

                return;
            }

            InitializeNodesFromStack(to, true, JsonSerializer.Serialize(Instance.GetJsonData()));
        }

        [ClientRpc]
        public static void InitializeNodesFromStack(bool access, string jsonData = null)
        {
            Window window = Window.Instance;

            if (!access)
            {
                Log.Info("No access to visual programming");

                return;
            }

            if (window != null)
            {
                window.Delete(true);
            }

            new Window(UI.Hud.Current.RootPanel, jsonData);
        }

        [ServerCmd]
        public static void UploadStack()
        {

        }

        [ClientRpc]
        public static void SendStackBuildResult()
        {

        }
    }
}
