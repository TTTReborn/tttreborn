namespace TTTReborn.Player.Camera
{
    public abstract class ObservationCamera : Sandbox.Camera, IObservationCamera
    {
        public Vector3 GetViewPosition()
        {
            return Pos;
        }
    }
}
