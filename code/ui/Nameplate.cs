using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
	public class Nameplate : Panel
	{
		public float MaxDrawDistance = 500;

        private Label nameLabel;

		public Nameplate()
		{
			StyleSheet.Load("/ui/Nameplate.scss");

            nameLabel = Add.Label("", "name");
		}

		public override void Tick()
		{
            TTTPlayer player = Local.Pawn as TTTPlayer;

            TraceResult trace = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MaxDrawDistance)
                .Ignore(player.ActiveChild)
                .Ignore(player)
                .Run();

            bool targetFound = trace.Hit && trace.Entity is TTTPlayer;

            SetClass("hide", !targetFound);

            if (!targetFound)
            {
                return;
            }

            nameLabel.Text = (trace.Entity as TTTPlayer).GetClientOwner().Name;
		}
	}
}
