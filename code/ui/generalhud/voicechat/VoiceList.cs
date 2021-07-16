using System.Linq;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public class VoiceList : Panel
    {
        public static VoiceList Current { get; internal set; }

        public VoiceList()
        {
            Current = this;

            StyleSheet.Load("/ui/generalhud/voicechat/VoiceList.scss");
        }

        public void OnVoicePlayed(ulong steamId, float level)
        {
            VoiceEntry entry = ChildrenOfType<VoiceEntry>().FirstOrDefault(x => x.Friend.Id == steamId) ?? new VoiceEntry(this, steamId);

            entry.Update(level);
        }
    }
}
