using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        public float HintDistance => 400f;

        public bool CanHint(TTTPlayer client)
        {
            return client.Camera is not FirstPersonSpectatorCamera;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Nameplate(this);
        }
    }
}
