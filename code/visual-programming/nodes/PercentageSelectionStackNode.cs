using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    [StackNode("percentage_selection")]
    public partial class PercentageSelectionStackNode : StackNode
    {
        public List<float> PercentList { get; set; } = new();

        public override object[] Test(object[] input)
        {
            int percentListCount = PercentList.Count;

            if (percentListCount < 2)
            {
                throw new NodeStackException("Missing values in PercentageSelectionStackNode.");
            }

            if (input == null || input[0] is not List<Player> playerList)
            {
                return null;
            }

            object[] buildArray = new object[percentListCount];

            if (playerList.Count == 0)
            {
                return buildArray;
            }

            int allPlayerAmount = Game.Clients.Count; // TODO just use available players, not specs
            int[] playerAmounts = new int[percentListCount];
            int count = 0;

            for (int i = 0; i < percentListCount; i++)
            {
                playerAmounts[i] = Math.Clamp((int) MathF.Round(allPlayerAmount * (PercentList[i] / 100f)), 1, playerList.Count);
                count += playerAmounts[i];
            }

            int loopIndex = 0;
            bool loopFound = false;

            while (count > playerList.Count)
            {
                if (playerAmounts[loopIndex] > 1)
                {
                    playerAmounts[loopIndex]--;
                    count--;
                    loopFound = true;
                }

                loopIndex++;

                if (loopIndex == percentListCount)
                {
                    if (loopFound)
                    {
                        loopIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < percentListCount; i++)
            {
                if (playerList.Count == 0)
                {
                    break;
                }

                List<Player> selectedPlayers = new();

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
                (buildArray[^1] as List<Player>).AddRange(playerList);
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
                PercentList = JsonSerializer.Deserialize<List<float>>((JsonElement) percentList);
            }

            base.LoadFromJsonData(jsonData);
        }
    }
}
