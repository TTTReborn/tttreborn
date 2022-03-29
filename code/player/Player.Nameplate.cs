using TTTReborn.Entities;
using TTTReborn.UI;

namespace TTTReborn
{
    public partial class Player : IEntityHint
    {
        public float HintDistance => 400f;

        public bool ShowGlow => false;

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new Nameplate(this);

        public void HintTick(Player player) { }
    }
}
