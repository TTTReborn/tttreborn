using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public readonly static List<InputButton> Buttons = Enum.GetValues(typeof(InputButton)).Cast<InputButton>().ToList();
        private static int SecondsTillKick => ServerSettings.Instance.AFK.MinutesTillKick * 60;

        private TimeSince _timeSinceLastAction = 0f;
        private Rotation? _lastKnownRotation;

        private void TickAFKSystem()
        {
            bool pressedAnyKeyPressed = Buttons.Any(button => Input.Pressed(button) || Input.Down(button));

            if (pressedAnyKeyPressed || (_lastKnownRotation.HasValue && _lastKnownRotation != Rotation))
            {
                _timeSinceLastAction = 0f;
                _lastKnownRotation = Rotation;
            }

            //Log.Warning($"Time: {_timeSinceLastAction}, Second to wait: {SecondsTillKick}, Bool: {_timeSinceLastAction > SecondsTillKick}");

            if (_timeSinceLastAction > SecondsTillKick)
            {
                Client client = GetClientOwner();

                if (client != null && !client.IsBot)
                {
                    Log.Warning($"Steam ID: {client.SteamId}, Name: {client.Name} was kicked from the server for being AFK.");
                    client.Kick();
                }
            }
        }
    }
}
