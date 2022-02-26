using System;
using System.Collections.Generic;
using System.Text.Json;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Build()
        {
            BuildButton.Icon = "hourglass_empty";

            NodeStack.Instance.Reset();

            bool hasError = false;

            List<Node> startingNodeList = new();

            foreach (Node node in Nodes)
            {
                node.Reset();

                if (!node.HasInputsFilled())
                {
                    node.HighlightError();

                    hasError = true;

                    continue;
                }
                else if (!node.HasInputEnabled())
                {
                    startingNodeList.Add(node);
                }

                node.Prepare();
            }

            if (hasError)
            {
                BuildButton.Icon = "play_arrow";

                return;
            }

            try
            {
                Log.Debug("Building and testing NodeStack");

                bool successful = true;

                foreach (Node node in startingNodeList)
                {
                    if (!node.Build(0))
                    {
                        successful = false;
                    }
                }

                if (successful)
                {
                    Log.Debug("Uploading NodeStack");

                    NodeStack.UploadStack(JsonSerializer.Serialize(GetStackNodesJsonDictionary()));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                BuildButton.Icon = "play_arrow";
            }
        }
    }
}
