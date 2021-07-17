using System.Collections.Generic;

using TTTReborn.Player;

namespace TTTReborn.Teams
{
    public abstract class TTTTeam
    {
        public abstract string Name { get; }

        public abstract Color Color { get; }

        public readonly List<TTTPlayer> Members;

        public static TTTTeam Instance;

        public TTTTeam()
        {
            Instance = this;
        }
    }
}
