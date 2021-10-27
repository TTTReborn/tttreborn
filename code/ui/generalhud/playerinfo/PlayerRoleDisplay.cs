using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class PlayerRoleDisplay : Panel
    {
        private TranslationLabel _roleLabel;

        public PlayerRoleDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerRoleDisplay.scss");

            AddClass("rounded");
            AddClass("centered-horizontal");
            AddClass("opacity-heavy");
            AddClass("text-shadow");

            _roleLabel = Add.TranslationLabel();
            _roleLabel.AddClass("centered");
            _roleLabel.AddClass("role-label");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Enabled = Local.Pawn is TTTPlayer && !player.IsSpectator && !player.IsSpectatingPlayer && Gamemode.Game.Instance.Round is Rounds.InProgressRound;

            Style.BackgroundColor = player.Role.Color;

            _roleLabel.SetTranslation(player.Role.GetRoleTranslationKey("NAME"));
        }
    }
}
