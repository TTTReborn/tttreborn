using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Effects : Panel
    {
        private List<Effect> EffectList = new();

        public Effects()
        {
            StyleSheet.Load("/ui/Effects.scss");
        }

        public void AddEffect(TTTPerk perk)
        {
            Effect effect = new Effect(this);
            effect.item = perk;

            EffectList.Add(effect);
        }

        public void RemoveEffect(TTTPerk perk)
        {
            foreach (Effect effect in EffectList)
            {
                if (effect.item.Name == perk.Name)
                {
                    EffectList.Remove(effect);

                    effect.Delete();

                    return;
                }
            }
        }

        public void OnHotReloaded()
        {
            PerksInventory perks = ((Local.Pawn as TTTPlayer).Inventory as Inventory).Perks;

            for (int i = 0; i < perks.Count(); i++)
            {
                AddEffect(perks.Get(i));
            }
        }
    }
}
