using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using Steamworks;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class VoiceEntry : Panel
    {
        public Friend Friend;

        readonly Label Name;
        readonly Image Avatar;
        readonly Client client;

        private float _voiceLevel = 0.5f;
        private float _targetVoiceLevel = 0;
        private Color _deadColor = Color.FromBytes(255, 204, 3);

        RealTimeSince timeSincePlayed;

        public VoiceEntry(Panel parent, ulong steamId)
        {
            Parent = parent;

            Friend = new Friend(steamId);

            Avatar = Add.Image("", "avatar");
            Avatar.SetTexture($"avatar:{steamId}");

            Name = Add.Label(Friend.Name, "name");

            foreach (Client loopClient in Client.All)
            {
                if (loopClient.SteamId == steamId)
                {
                    client = loopClient;

                    break;
                }
            }
        }

        public void Update(float level)
        {
            timeSincePlayed = 0;
            Name.Text = Friend.Name;
            _targetVoiceLevel = level;
        }

        public override void Tick()
        {
            base.Tick();

            if (IsDeleting)
            {
                return;
            }

            float speakTimeout = 0.5f;

            float timeoutInv = 1 - (timeSincePlayed / speakTimeout);
            timeoutInv = MathF.Min(timeoutInv * 2.0f, 1.0f);

            if (timeoutInv <= 0)
            {
                Delete();

                return;
            }

            _voiceLevel = _voiceLevel.LerpTo(_targetVoiceLevel, Time.Delta * 40.0f);

            Style.Left = _voiceLevel * -32.0f * timeoutInv;

            if (client != null && client.IsValid() && client.Pawn is TTTPlayer player)
            {
                if (player.LifeState == LifeState.Dead)
                {
                    Style.BackgroundColor = _deadColor;
                }
                else if (player.Role is not NoneRole)
                {
                    Style.BackgroundColor = player.Role.Color;
                }
            }
            else
            {
                Style.BackgroundColor = null;
            }

            Style.Dirty();
        }
    }
}
