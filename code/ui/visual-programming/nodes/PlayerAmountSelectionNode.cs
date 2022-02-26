using System.Collections.Generic;
using System.Text.Json;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [Spawnable]
    [Node("playeramount_selection")]
    public class PlayerAmountSelectionNode : Node
    {
        public List<int> PlayerAmountList { get; set; } = new();

        public PlayerAmountSelectionNode() : base(new PlayerAmountSelectionStackNode())
        {
            SetTitle("PlayerAmountSelection Node");

            AddSetting<NodePercentSetting>();
            AddSetting<NodePercentSetting>().ToggleInput(false);
            // TODO add a way to add new entries via GUI
        }

        internal void OnChange()
        {
            PlayerAmountList.Clear();

            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                NodePercentSetting nodePercentSetting = nodeSetting as NodePercentSetting;

                int playerAmount = int.Parse(nodePercentSetting.PercentEntry.Text);

                PlayerAmountList.Add(playerAmount);
            }
        }

        public override void Prepare()
        {
            (StackNode as PlayerAmountSelectionStackNode).PlayerAmountList = PlayerAmountList;

            base.Prepare();
        }

        public override Dictionary<string, object> GetJsonData()
        {
            Dictionary<string, object> dict = base.GetJsonData();
            dict.Add("PlayerAmountList", PlayerAmountList);

            return dict;
        }

        public override void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("PlayerAmountList", out object playerAmountList);

            if (playerAmountList != null)
            {
                PlayerAmountList = JsonSerializer.Deserialize<List<int>>(((JsonElement) playerAmountList).GetRawText());

                if (NodeSettings.Count < PlayerAmountList.Count)
                {
                    for (int i = 0; i < PlayerAmountList.Count - NodeSettings.Count; i++)
                    {
                        AddSetting<NodePercentSetting>().ToggleInput(false);
                    }
                }

                for (int i = 0; i < NodeSettings.Count; i++)
                {
                    (NodeSettings[i] as NodePercentSetting).PercentEntry.Text = PlayerAmountList.Count > i ? PlayerAmountList[i].ToString() : "";
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
