using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    [StackNode("playeramount_selection")]
    public partial class PlayerAmountSelectionStackNode : StackNode
    {
        public List<int> PlayerAmountList { get; set; } = new();

        public override object[] Test(object[] input)
        {
            int playerAmountListCount = PlayerAmountList.Count;

            if (playerAmountListCount < 2)
            {
                throw new NodeStackException("Missing values in PlayerAmountSelectionStackNode.");
            }

            if (input == null || input[0] is not List<Player> playerList)
            {
                return null;
            }

            int allPlayerAmount = Game.Clients.Count; // TODO just use available players, not specs

            object[] buildArray = new object[playerAmountListCount];

            for (int i = playerAmountListCount - 1; i > 0; i--)
            {
                if (allPlayerAmount >= PlayerAmountList[i])
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
            dict.Add("PlayerAmountList", PlayerAmountList);

            return dict;
        }

        public override void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("PlayerAmountList", out object playerAmountList);

            if (playerAmountList != null)
            {
                PlayerAmountList = JsonSerializer.Deserialize<List<int>>((JsonElement) playerAmountList);
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
