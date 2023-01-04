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

            AddClass("voicechat-display");
        }

        public void OnVoicePlayed(IClient client, float level)
        {
            VoiceChatEntry entry = ChildrenOfType<VoiceChatEntry>().FirstOrDefault(x => x.Friend.Id == client.SteamId);

            if (entry == null)
            {
                entry = new(client);

                AddChild(entry);
            }

            entry.Update(level);
        }
    }
}
