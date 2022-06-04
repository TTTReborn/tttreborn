using Sandbox.UI.Construct;

namespace TTTReborn.UI.VisualProgramming
{
    [NodeSetting("all_players"), HideInEditor]
    public class NodeAllPlayersSetting : NodeSetting
    {
        public NodeAllPlayersSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("All Players");
            });
        }
    }
}
