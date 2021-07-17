using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using Steamworks;

namespace TTTReborn.UI
{
    public class VoiceEntry : Panel
    {
        public Friend Friend;

        readonly Label Name;
        readonly Image Avatar;

        private float _voiceLevel = 0.5f;
        private float _targetVoiceLevel = 0;

        RealTimeSince timeSincePlayed;

        public VoiceEntry(Panel parent, ulong steamId)
        {
            Parent = parent;

            Friend = new Friend(steamId);

            Avatar = Add.Image("", "avatar");
            Avatar.SetTexture($"avatar:{steamId}");

            Name = Add.Label(Friend.Name, "name");
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

            float speakTimeout = 2.0f;

            float timeoutInv = 1 - (timeSincePlayed / speakTimeout);
            timeoutInv = MathF.Min(timeoutInv * 2.0f, 1.0f);

            if (timeoutInv <= 0)
            {
                Delete();

                return;
            }

            _voiceLevel = _voiceLevel.LerpTo(_targetVoiceLevel, Time.Delta * 40.0f);

            Style.Left = _voiceLevel * -32.0f * timeoutInv;
            Style.Dirty();
        }
    }
}
