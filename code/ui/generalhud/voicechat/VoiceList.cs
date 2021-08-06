using System.Linq;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public class VoiceList : TTTPanel
    {
        public static VoiceList Current { get; internal set; }

        public VoiceList()
        {
            Current = this;

            StyleSheet.Load("/ui/generalhud/voicechat/VoiceList.scss");
        }

        public void OnVoicePlayed(Client client, float level)
        {
            VoiceEntry entry = ChildrenOfType<VoiceEntry>().FirstOrDefault(x => x.Friend.Id == client.SteamId) ?? new VoiceEntry(this, client);

            entry.Update(level);
        }
    }
}
