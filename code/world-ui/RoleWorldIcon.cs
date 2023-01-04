using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.WorldUI
{
    public class RoleWorldIcon : WorldPanel
    {
        public Player Player { get; }
        private Role Role { get; set; }

        public RoleWorldIcon(Player player) : base()
        {
            Player = player;
            Role = player.Role;

            StyleSheet.Load("/world-ui/RoleWorldIcon.scss");

            CreateIcon();
        }

        private void CreateIcon()
        {
            Add.Image($"assets/icons/{Role.Name}.png", "role-icon");
        }

        public override void Tick()
        {
            base.Tick();

            if (!Player.IsValid || Player.LifeState != LifeState.Alive || Game.LocalPawn is Player localPlayer && !Player.IsTeamMember(localPlayer))
            {
                this.Enabled(false);
            }
            else
            {
                this.Enabled(true);

                Transform = GetTransform();

                if (Role != Player.Role)
                {
                    Role = Player.Role;

                    Style.BackgroundColor = Role.Color;

                    DeleteChildren(true);
                    CreateIcon();
                }
            }
        }

        private Transform GetTransform()
        {
            Transform transform = Player.GetBoneTransform((int) HitboxIndex.Head).WithScale(0.125f);
            transform.Position += Vector3.Up * 22.5f;
            transform.Rotation = Sandbox.Camera.Rotation;

            return transform;
        }
    }
}
