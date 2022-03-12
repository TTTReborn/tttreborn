using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        public float HintDistance => 400f;

        public bool ShowGlow => false;

        public bool CanHint(TTTPlayer client) => true;

        public EntityHintPanel DisplayHint(TTTPlayer client) => new Nameplate(this);

        public void TextTick(TTTPlayer player)
        {

        }
    }
}
