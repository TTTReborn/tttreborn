using Sandbox;

namespace TTTReborn;

public partial class Player
{
    private void TickPlayerChangeSpectateCamera()
    {
        if (!Input.Pressed(InputButton.Jump) || !Game.IsServer)
        {
            return;
        }

        // using (Prediction.Off())
        // {
        //    CameraMode = CameraMode switch
        //    {
        //        RagdollSpectateCamera => new FreeSpectateCamera(),
        //        FreeSpectateCamera => new ThirdPersonSpectateCamera(),
        //        ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
        //        FirstPersonSpectatorCamera => new FreeSpectateCamera(),
        //        _ => CameraMode
        //    };
        // }
    }
}
