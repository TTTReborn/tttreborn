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

using System.Linq;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public class VoiceChatDisplay : Panel
    {
        public static VoiceChatDisplay Instance { get; internal set; }

        public VoiceChatDisplay() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/voicechat/VoiceChatDisplay.scss");
        }

        public void OnVoicePlayed(Client client, float level)
        {
            VoiceChatEntry entry = ChildrenOfType<VoiceChatEntry>().FirstOrDefault(x => x.Friend.Id == client.PlayerId) ?? new VoiceChatEntry(this, client);

            entry.Update(level);
        }
    }
}
