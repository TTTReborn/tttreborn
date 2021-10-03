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

        private TimeSince _timeSinceLastAction = 0f;

        private void TickAFKSystem()
        {
            if (Client == null || Client.IsBot)
            {
                return;
            }

            if (IsForcedSpectator || IsSpectator)
            {
                _timeSinceLastAction = 0;

                return;
            }

            bool isAnyKeyPressed = Buttons.Any(button => Input.Down(button));
            bool isMouseMoving = Input.MouseDelta != Vector3.Zero;

            if (isAnyKeyPressed || isMouseMoving)
            {
                _timeSinceLastAction = 0f;

                return;
            }

            if (_timeSinceLastAction > ServerSettings.Instance.AFK.SecondsTillKick)
            {
                bool shouldKick = ServerSettings.Instance.AFK.ShouldKickPlayers;

                if (shouldKick)
                {
                    Log.Warning($"Steam ID: {Client.SteamId}, Name: {Client.Name} was kicked from the server for being AFK.");

                    Client.Kick();
                }
                else
                {
                    Log.Warning($"Steam ID: {Client.SteamId}, Name: {Client.Name} was moved to spectating for being AFK.");

                    ToggleForcedSpectator();
                }
            }
        }
    }
}
