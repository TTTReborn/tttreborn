using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class PlayerRoleDisplay : Panel
    {
        private TranslationLabel RoleLabel { get; set; }
        private Roles.Role _role;

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not Player player)
            {
                return;
            }

            this.Enabled(!player.IsSpectator && !player.IsSpectatingPlayer && Gamemode.Game.Instance.Round is Rounds.InProgressRound);

            if (this.IsEnabled())
            {
                if (player.Role.Name != _role?.Name)
                {
                    _role = player.Role;

                    Style.BackgroundColor = _role.Color;

                    RoleLabel.UpdateTranslation(new TranslationData(_role.GetTranslationKey("NAME")));
                }
            }
        }
    }
}
