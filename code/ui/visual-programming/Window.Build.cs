using System;
using System.Text.Json;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Build()
        {
            BuildButton.Text = "hourglass_empty";

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
                Log.Debug("Building and testing NodeStack");

                if (!MainNode.Build())
                {
                    return;
                }

                Log.Debug("Uploading NodeStack");

                NodeStack.UploadStack(JsonSerializer.Serialize(MainNode.StackNode.GetJsonData()));
            }
            catch (Exception e)
            {
                Log.Error(e);

                BuildButton.Text = "play_arrow";
            }
        }
    }
}
