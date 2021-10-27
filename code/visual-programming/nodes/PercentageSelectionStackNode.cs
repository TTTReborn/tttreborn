using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.VisualProgramming
{
    public partial class PercentageSelectionStackNode : StackNode
    {
        public List<float> PercentList { get; set; } = new();

        public PercentageSelectionStackNode() : base()
        {

        }

        public override object[] Build(params object[] input)
        {
            if (input == null || input[0] is not List<TTTPlayer> playerList)
            {
                return null;
            }

            int percentListCount = PercentList.Count;

            if (percentListCount < 2)
            {
                throw new NodeStackException("Missing values in RandomNode.");
            }

            Random random = new();

            int allPlayerAmount = Client.All.Count;

            object[] buildArray = new object[percentListCount];

            for (int i = 0; i < percentListCount; i++)
            {
                int playerAmount = (int) MathF.Floor((float) allPlayerAmount * (PercentList[i] / 100f));

                List<TTTPlayer> selectedPlayers = new();

                for (int index = 0; index < playerAmount; index++)
                {
                    int rnd = random.Next(playerList.Count);

                    selectedPlayers.Add(playerList[rnd]);
                    playerList.RemoveAt(rnd);
                }

                buildArray[i] = selectedPlayers;
            }

            if (playerList.Count > 0)
            {
                (buildArray[^1] as List<TTTPlayer>).AddRange(playerList);
            }

            return base.Build(buildArray);
        }
    }
}
