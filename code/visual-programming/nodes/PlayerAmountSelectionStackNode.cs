using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.VisualProgramming
{
    [StackNode("playeramount_selection")]
    public partial class PlayerAmountSelectionStackNode : StackNode
    {
        public List<int> PlayerAmountList { get; set; } = new();

        public PlayerAmountSelectionStackNode() : base()
        {

        }

        public override object[] Test(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            int playerAmountListCount = PlayerAmountList.Count;

            if (playerAmountListCount < 2)
            {
                throw new NodeStackException("Missing values in PlayerAmountSelectionStackNode.");
            }

            int allPlayerAmount = Client.All.Count; // TODO just use available players, not specs

            object[] buildArray = new object[playerAmountListCount];

            for (int i = 0; i < playerAmountListCount; i++)
            {
                if (allPlayerAmount >= PlayerAmountList[i])
                {
                    buildArray[i] = playerList;

                    return buildArray;
                }
            }

            return buildArray;
        }

        public override object[] Evaluate(params object[] input) => Test(input);

        public override Dictionary<string, object> GetJsonData(List<StackNode> proceedNodes = null)
        {
            Dictionary<string, object> dict = base.GetJsonData(proceedNodes);
            dict.Add("PlayerAmountList", PlayerAmountList);

            return dict;
        }

        public override void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("PlayerAmountList", out object playerAmountList);

            if (playerAmountList != null)
            {
                PlayerAmountList = JsonSerializer.Deserialize<List<int>>(((JsonElement) playerAmountList).GetRawText());
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
