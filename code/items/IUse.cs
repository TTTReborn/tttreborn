using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface ITTTUse
    {
        public void StopUsing(TTTPlayer player);
        public void TickUse(TTTPlayer player);
    }
}
