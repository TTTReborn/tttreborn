using Sandbox;

namespace TTTReborn;

public partial class Player
{
	/// <summary>
	/// Called every tick to simulate the player. This is called on the
	/// client as well as the server (for prediction). So be careful!
	/// </summary>
	public override void Simulate(IClient client)
	{
        if (Game.IsClient)
        {
            TickPlayerVoiceChat();
        }
        else
        {
            TickAFKSystem();
        }

        TickEntityHints();

        if (LifeState != LifeState.Alive)
        {
            TickPlayerChangeSpectateCamera();

            return;
        }

		if (ActiveChildInput.IsValid() && ActiveChildInput.Owner == this)
		{
			ActiveChild = ActiveChildInput;
		}

		SimulateActiveChild(client, ActiveChild);

        TickItemSimulate();

        if (Game.IsServer)
        {
            TickPlayerUse();
            TickPlayerDropCarriable();
            TickPlayerFlashlight();
        }
        else
        {
            TickPlayerShop();
            TickLogicButtonActivate();
        }

		PawnController controller = GetActiveController();
		controller?.Simulate(client, this);
	}

	public override void FrameSimulate(IClient cl)
	{
		Sandbox.Camera.Rotation = ViewAngles.ToRotation();
		Sandbox.Camera.Position = EyePosition;
		Sandbox.Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
		Sandbox.Camera.FirstPersonViewer = this;
	}
}
