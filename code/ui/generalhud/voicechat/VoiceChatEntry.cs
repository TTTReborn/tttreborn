using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Roles;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class VoiceChatEntry : Panel
    {
        public Friend Friend { get; set; }

        public string Name
        {
            get => Friend.Name;
        }
        public Image Avatar { get; set; }
        public Client Client { get; }

        private float _voiceLevel = 0.5f;
        private float _targetVoiceLevel = 0;
        private const float VOICE_TIMEOUT = 0.1f;

        private RealTimeSince _timeSincePlayed;

        public VoiceChatEntry(Client client)
        {
            Client = client;
            Friend = new(client.PlayerId);

            Avatar.SetTexture($"avatar:{client.PlayerId}");
        }

        public void Update(float level)
        {
            _timeSincePlayed = 0;
            _targetVoiceLevel = level;

            if (Client != null && Client.IsValid() && Client.Pawn is Player player)
            {
                SetClass("background-color-spectator", player.LifeState == LifeState.Dead);

                if (player.IsTeamVoiceChatEnabled && player.Role is not NoneRole)
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

            float timeoutInv = 1 - (_timeSincePlayed / VOICE_TIMEOUT);
            timeoutInv = MathF.Min(timeoutInv * 2.0f, 1.0f);

            if (timeoutInv <= 0)
            {
                Delete();

                return;
            }

            _voiceLevel = _voiceLevel.LerpTo(_targetVoiceLevel, Time.Delta * 40.0f);
        }
    }
}
