using System;
using System.Text.Json;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Build()
        {
            MainNode.StackNode.Reset();

            bool hasError = false;

            foreach (Node node in Nodes)
            {
                node.RemoveHighlights();

                if (!node.HasInput() && node != MainNode)
                {
                    node.HighlightError();

                    hasError = true;
                }
            }

            if (hasError)
            {
                return;
            }

            try
            {
                Log.Debug("Building NodeStack");

                if (!MainNode.Build())
                {
                    return;
                }

                Log.Debug("Uploading NodeStack");

                Log.Error(JsonSerializer.Serialize(MainNode.StackNode.GetJsonData()));

                // TODO sync _nodeStack to server
                // TODO test on server again
                // TODO if test passed, save stacknode and give feedback to client
                // TODO add a reset button
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
