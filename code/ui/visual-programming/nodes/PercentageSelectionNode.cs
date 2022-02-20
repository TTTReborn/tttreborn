using System.Collections.Generic;
using System.Text.Json;

using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [Spawnable]
    [Node("percentage_selection")]
    public class PercentageSelectionNode : Node
    {
        public List<float> PercentList { get; set; } = new();

        public PercentageSelectionNode() : base(new PercentageSelectionStackNode())
        {
            SetTitle("PercentageSelection Node");

            AddSetting<NodePercentSetting>();
            AddSetting<NodePercentSetting>().ToggleInput(false);
            // TODO add a way to add new entries via GUI
        }

        internal void OnChange()
        {
            PercentList.Clear();

            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                NodePercentSetting nodePercentSetting = nodeSetting as NodePercentSetting;

                float percent = float.Parse(nodePercentSetting.PercentEntry.Text);

                PercentList.Add(percent);
            }
        }

        public override void Prepare()
        {
            (StackNode as PercentageSelectionStackNode).PercentList = PercentList;

            base.Prepare();
        }

        public override Dictionary<string, object> GetJsonData()
        {
            Dictionary<string, object> dict = base.GetJsonData();
            dict.Add("PercentList", PercentList);

            return dict;
        }

        public override void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("PercentList", out object percentList);

            if (percentList != null)
            {
                PercentList = JsonSerializer.Deserialize<List<float>>(((JsonElement) percentList).GetRawText());

                if (NodeSettings.Count < PercentList.Count)
                {
                    for (int i = 0; i < PercentList.Count - NodeSettings.Count; i++)
                    {
                        AddSetting<NodePercentSetting>().ToggleInput(false);
                    }
                }

                for (int i = 0; i < NodeSettings.Count; i++)
                {
                    (NodeSettings[i] as NodePercentSetting).PercentEntry.Text = PercentList.Count > i ? PercentList[i].ToString() : "";
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
