using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.WorldUI
{
    public class RoleWorldIcon : WorldPanel
    {
        public TTTPlayer Player { get; }
        private Role Role { get; set; }

        public RoleWorldIcon(TTTPlayer player) : base()
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

            if (!Player.IsValid || Player.LifeState != LifeState.Alive || Local.Pawn is TTTPlayer localPlayer && !Player.IsTeamMember(localPlayer))
            {
                Style.Display = DisplayMode.None;
            }
            else
            {
                Style.Display = DisplayMode.Flex;

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
            Transform transform = Player.GetBoneTransform(Player.GetHitboxBone((int) HitboxIndex.Head)).WithScale(0.125f);
            transform.Position += Vector3.Up * 22.5f;
            transform.Rotation = Rotation.LookAt(-CurrentView.Rotation.Forward);

            return transform;
        }
    }
}
