using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using Steamworks;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class VoiceChatEntry : Panel
    {
        public Friend Friend;

        readonly Label Name;
        readonly Image Avatar;
        readonly Client Client;

        private float _voiceLevel = 0.5f;
        private float _targetVoiceLevel = 0;
        private float _voiceTimeout = 0.1f;

        RealTimeSince timeSincePlayed;

        public VoiceChatEntry(Sandbox.UI.Panel parent, Client client) : base(parent)
        {
            Parent = parent;

            Client = client;
            Friend = new(client.SteamId);

            Avatar = Add.Image("", "avatar");
            Avatar.SetTexture($"avatar:{client.SteamId}");
            Avatar.AddClass("circular");

            Name = Add.Label(Friend.Name, "name");
            Name.AddClass("text-shadow");

            AddClass("rounded");
            AddClass("opacity-90");
        }

        public void Update(float level)
        {
            timeSincePlayed = 0;
            Name.Text = Friend.Name;
            _targetVoiceLevel = level;

            if (Client != null && Client.IsValid() && Client.Pawn is TTTPlayer player)
            {
                if (player.IsSpeaking)
                {
                    Style.BackgroundColor = ColorScheme.Primary;
                }

                if (player.LifeState == LifeState.Dead)
                {
                    Style.BackgroundColor = ColorScheme.Spectator;
                }
                else if (player.IsTeamVoiceChatEnabled && player.Role is not NoneRole)
                {
                    Style.BackgroundColor = player.Role.Color;
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (IsDeleting)
            {
                return;
            }

            float timeoutInv = 1 - (timeSincePlayed / _voiceTimeout);
            timeoutInv = MathF.Min(timeoutInv * 2.0f, 1.0f);

            if (timeoutInv <= 0)
            {
                Delete();

                return;
            }

            _voiceLevel = _voiceLevel.LerpTo(_targetVoiceLevel, Time.Delta * 40.0f);

            Style.Left = 10f + _voiceLevel * -10.0f * timeoutInv;
            Style.Dirty();
        }
    }
}
