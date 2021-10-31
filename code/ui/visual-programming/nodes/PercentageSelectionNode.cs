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

            HighlightError();
        }

        public override void Build(params object[] input)
        {
            PercentList.Clear();

            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                NodePercentSetting nodePercentSetting = nodeSetting as NodePercentSetting;

                float percent = float.Parse(nodePercentSetting.PercentEntry.Text);

                PercentList.Add(percent);
            }

            (StackNode as PercentageSelectionStackNode).PercentList = PercentList;

            base.Build(input);
        }

        public override Dictionary<string, object> GetJsonData(List<Node> proceedNodes = null)
        {
            Dictionary<string, object> dict = base.GetJsonData(proceedNodes);
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
                    string text = "";

                    if (PercentList.Count > i)
                    {
                        text = PercentList[i].ToString();
                    }

                    (NodeSettings[i] as NodePercentSetting).PercentEntry.Text = text;
                }
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
