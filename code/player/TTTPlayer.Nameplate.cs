using TTTReborn.Hints;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        public float HintDistance => 400f;

        public bool CanHint(TTTPlayer player)
        {
            return true;
        }

        public EntityHintPanel DisplayHintPanel(TTTPlayer player)
        {
            return new Nameplate();
        }

        public void UpdateHintPanel(EntityHintPanel entityHintPanel)
        {

        }
    }
}
