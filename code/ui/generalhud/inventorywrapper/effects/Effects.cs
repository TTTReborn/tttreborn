using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effects : Panel
    {
        private readonly List<Effect> _effectList = new();

        public Effects()
        {
            StyleSheet.Load("/ui/generalhud/inventorywrapper/effects/Effects.scss");

            if (Local.Pawn is not Player player)
            {
                return;
            }

            PerksInventory perks = player.Inventory.Perks;

            for (int i = 0; i < perks.Count(); i++)
            {
                AddEffect(perks.Get(i));
            }
        }

        public void AddEffect(Perk perk)
        {
            _effectList.Add(new Effect(this, perk));
        }

        public void RemoveEffect(Perk perk)
        {
            foreach (Effect effect in _effectList)
            {
                if (effect.Item.LibraryName == perk.LibraryName)
                {
                    _effectList.Remove(effect);
                    effect.Delete();

                    return;
                }
            }
        }
    }
}
