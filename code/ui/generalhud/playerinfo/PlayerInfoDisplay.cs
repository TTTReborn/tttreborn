using Sandbox;
using Sandbox.UI;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public class PlayerInfoDisplay : Panel
    {
        private string Health { get; set; }
        private string Credits { get; set; }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not Player player)
            {
                return;
            }

            this.Enabled(player.CurrentPlayer.LifeState == LifeState.Alive);

            Health = $"✚ {player.CurrentPlayer.Health.CeilToInt()}";
            Credits = $"$ {player.CurrentPlayer.Credits}";
        }
    }
}
