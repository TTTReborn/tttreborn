using System.Linq;

using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private static int SecondsTillKick => ServerSettings.Instance.AFK.MinutesTillKick * 60;
        private float TimeToKick => _timeSinceLastAction + SecondsTillKick;

        private float _timeSinceLastAction = 60.0f;
        private Rotation? LastKnownRotation;

        private void TickAFKSystem()
        {
            bool pressedAnyKeyPressed = Buttons.Any(button => Input.Pressed(button) || Input.Down(button));

            if (pressedAnyKeyPressed || (LastKnownRotation.HasValue && LastKnownRotation != Rotation))
            {
                _timeSinceLastAction = Time.Now;
                LastKnownRotation = Rotation;
            }


            if (TimeToKick < Time.Now)
            {
                KickClient();
            }
        }

        [ServerCmd]
        public static void KickClient()
        {
            Log.Warning($"{ConsoleSystem.Caller.Name}-{ConsoleSystem.Caller.SteamId} was kicked from the server for being AFK.");
            ConsoleSystem.Caller.Kick();
        }
    }
}
