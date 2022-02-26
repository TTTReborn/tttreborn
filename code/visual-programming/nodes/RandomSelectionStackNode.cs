using System.Collections.Generic;
using System.Text.Json;

using TTTReborn.Player;

namespace TTTReborn.VisualProgramming
{
    [StackNode("random_selection")]
    public partial class RandomSelectionStackNode : StackNode
    {
        public List<float> PercentList { get; set; } = new();

        public RandomSelectionStackNode() : base()
        {

        }

        public override object[] Test(object[] input)
        {
            int percentListCount = PercentList.Count;

            if (percentListCount < 2)
            {
                throw new NodeStackException("Missing values in RandomSelectionStackNode.");
            }

            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            int value = 0;
            int rnd = Utils.RNG.Next(100) + 1;

            Log.Debug($"Selected random integer: '{rnd}'");

            object[] buildArray = new object[percentListCount];

            for (int i = 0; i < percentListCount; i++)
            {
                value += (int) PercentList[i];

                if (rnd <= value)
                {
                    buildArray[i] = playerList;

                    return buildArray;
                }
            }

            return buildArray;
        }

        public override object[] Evaluate(object[] input) => Test(input);

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
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
