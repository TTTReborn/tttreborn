using System.Linq;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public class VoiceChatDisplay : Panel
    {
        public static VoiceChatDisplay Current { get; internal set; }

        public VoiceChatDisplay() : base()
        {
            Current = this;

            StyleSheet.Load("/ui/generalhud/voicechat/VoiceChatDisplay.scss");
        }

        public void OnVoicePlayed(Client client, float level)
        {
            VoiceChatEntry entry = ChildrenOfType<VoiceChatEntry>().FirstOrDefault(x => x.Friend.Id == client.SteamId) ?? new VoiceChatEntry(this, client);

            entry.Update(level);
        }
    }
}
