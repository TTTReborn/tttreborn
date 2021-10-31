using System;

namespace TTTReborn.UI.VisualProgramming
{
    public partial class Window
    {
        public void Build()
        {
            _nodeStack.Reset();

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
                MainNode.Build();

                // TODO sync _nodeStack to server and save
                // JsonSerializer.Serialize(MainNode.GetJsonData());
            }
            catch (Exception e)
            {
                Sandbox.Log.Error(e);
            }
        }
    }
}
