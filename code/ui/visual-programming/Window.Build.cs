using System;
using System.Text.Json;

using TTTReborn.VisualProgramming;

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

                NodeStack.UploadStack(JsonSerializer.Serialize(MainNode.StackNode.GetJsonData()));

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
