using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class PlayerRoleDisplay : Panel
    {
        private readonly TranslationLabel _roleLabel;

        public PlayerRoleDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerRoleDisplay.scss");

            AddClass("rounded");
            AddClass("centered-horizontal");
            AddClass("opacity-heavy");
            AddClass("text-shadow");

            _roleLabel = Add.TranslationLabel(new TranslationData());
            _roleLabel.AddClass("centered");
            _roleLabel.AddClass("role-label");

            OnRoleUpdate(Local.Pawn as Player);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not Player player)
            {
                return;
            }

            this.Enabled(!player.IsSpectator && !player.IsSpectatingPlayer && Gamemode.Game.Instance.Round is Rounds.InProgressRound);
        }

        [Event(typeof(Events.Player.Role.SelectEvent))]
        protected void OnRoleUpdate(Player player)
        {
            if (player == null || player != Local.Pawn as Player)
            {
                return;
            }

            Style.BackgroundColor = player.Role.Color;

            _roleLabel.UpdateTranslation(new TranslationData(player.Role.GetTranslationKey("NAME")));
        }
    }
}
