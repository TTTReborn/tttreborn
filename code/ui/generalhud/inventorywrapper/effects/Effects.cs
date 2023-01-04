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

            AddClass("effects");

            if (Game.LocalPawn is not Player player)
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
            Effect effect = new(perk);

            _effectList.Add(effect);
            AddChild(effect);
        }

        public void RemoveEffect(Perk perk)
        {
            foreach (Effect effect in _effectList)
            {
                if (effect.Item.Info.LibraryName == perk.Info.LibraryName)
                {
                    _effectList.Remove(effect);
                    effect.Delete();

                    return;
                }
            }
        }
    }
}
