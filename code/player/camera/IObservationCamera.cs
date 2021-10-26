namespace TTTReborn.Player.Camera
{
    public interface IObservationCamera
    {
        Vector3 GetViewPosition();

        void OnUpdateObservatedPlayer(TTTPlayer oldObservatedPlayer, TTTPlayer newObservatedPlayer)
        {

        }
    }
}
