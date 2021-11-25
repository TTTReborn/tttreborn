// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
