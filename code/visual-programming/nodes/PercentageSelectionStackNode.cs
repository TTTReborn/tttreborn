using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.VisualProgramming
{
    [StackNode("percentage_selection")]
    public partial class PercentageSelectionStackNode : StackNode
    {
        public List<float> PercentList { get; set; } = new();

        public PercentageSelectionStackNode() : base()
        {

        }

        public override object[] Test(object[] input)
        {
            int percentListCount = PercentList.Count;

            if (percentListCount < 2)
            {
                throw new NodeStackException("Missing values in PercentageSelectionStackNode.");
            }

            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            object[] buildArray = new object[percentListCount];

            if (playerList.Count == 0)
            {
                return buildArray;
            }

            int allPlayerAmount = Client.All.Count; // TODO just use available players, not specs
            int[] playerAmounts = new int[percentListCount];

            for (int i = 0; i < percentListCount; i++)
            {
                if (playerList.Count == 0)
                {
                    return buildArray;
                }

                playerAmounts[i] = Math.Clamp((int) MathF.Ceiling(allPlayerAmount * (PercentList[i] / 100f)), 1, playerList.Count) - 1;

                int rnd = Utils.RNG.Next(playerList.Count);

                List<TTTPlayer> selectedPlayers = new()
                {
                    playerList[rnd]
                };

                playerList.RemoveAt(rnd);

                buildArray[i] = selectedPlayers;
            }

            for (int i = 0; i < percentListCount; i++)
            {
                if (playerList.Count == 0)
                {
                    break;
                }

                List<TTTPlayer> selectedPlayers = new();

                for (int index = 0; index < playerAmounts[i]; index++)
                {
                    if (playerList.Count == 0)
                    {
                        break;
                    }

                    int rnd = Utils.RNG.Next(playerList.Count);

                    selectedPlayers.Add(playerList[rnd]);
                    playerList.RemoveAt(rnd);
                }

                buildArray[i] = selectedPlayers.Count > 0 ? selectedPlayers : null;
            }

            if (playerList.Count > 0)
            {
                (buildArray[^1] as List<TTTPlayer>).AddRange(playerList);
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
