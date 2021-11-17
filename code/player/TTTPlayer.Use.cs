using System.Linq;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player.Camera;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : Sandbox.Player
    {
        private ModelEntity activeUsable;

        private void TickPlayerTTTUse()
        {
            using (Prediction.Off())
            {
                Entity ent = IsLookingAtType<Entity>(DEFAULT_USE_DISTANCE);
                if (ent == null || ent is not ITTTUse || ent is not ModelEntity modelEntity)
                {
                    TTTStopUsing(this);
                    return;
                }

                activeUsable = modelEntity;

                if (IsClient)
                {
                    activeUsable.GlowColor = Color.Cyan;
                    activeUsable.GlowActive = true;
                }

                (activeUsable as ITTTUse).TickUse(this);
            }
        }

        private void TTTStopUsing(TTTPlayer player)
        {
            if (activeUsable == null)
            {
                return;
            }

            if (activeUsable is ITTTUse useable)
            {
                useable.StopUsing(player);
            }

            if (IsClient)
            {
                activeUsable.GlowActive = false;
            }

            activeUsable = null;
        }
    }
}
