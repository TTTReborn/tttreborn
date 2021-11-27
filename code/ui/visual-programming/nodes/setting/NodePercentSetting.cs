using System;

using Sandbox.UI.Construct;

namespace TTTReborn.UI.VisualProgramming
{
    [NodeSetting("percent")]
    public class NodePercentSetting : NodeSetting
    {
        public Sandbox.UI.TextEntry PercentEntry;

        public NodePercentSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                PercentEntry = panelContent.Add.TextEntry(""); // TODO improve with validity checks and error toggling
                PercentEntry.AddEventListener("onchange", (panelEvent) =>
                {
                    try
                    {
                        if (Node is PercentageSelectionNode percentageSelectionNode)
                        {
                            percentageSelectionNode.OnChange();
                        }

                        Node?.RemoveHighlights();
                    }
                    catch (Exception)
                    {
                        Node?.HighlightError();
                    }
                });
            });
        }
    }
}
