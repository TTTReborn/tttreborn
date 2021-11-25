// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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
            Friend = new(client.PlayerId);

            Avatar = Add.Image("", "avatar");
            Avatar.SetTexture($"avatar:{client.PlayerId}");
            Avatar.AddClass("circular");

            Name = Add.Label(Friend.Name, "name");

            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("opacity-heavy");
            AddClass("text-shadow");
        }

        public void Update(float level)
        {
            timeSincePlayed = 0;
            Name.Text = Friend.Name;
            _targetVoiceLevel = level;

            if (Client != null && Client.IsValid() && Client.Pawn is TTTPlayer player)
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

            float timeoutInv = 1 - (timeSincePlayed / _voiceTimeout);
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
