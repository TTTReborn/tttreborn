using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Teams;

namespace TTTReborn.UI
{

    public class ChatGroup : Panel
    {
        public string Name;

        private readonly Panel _textMessages;

        private readonly Panel _head;

        public readonly Image Avatar;
        public readonly Label NameLabel;
        public readonly List<ChatEntry> Messages = new();


        public ChatGroup()
        {
            _head = Add.Panel("head");
            Avatar = _head.Add.Image();
            NameLabel = _head.Add.Label("", "name");
            SetClass("showHead", true);

            _textMessages = Add.Panel("chatGroup_textMessages");
        }

        public void AddMessage(string text, string team)
        {
            ChatEntry chatEntry = new ChatEntry(text);

            Messages.Add(chatEntry);
            _textMessages.AddChild(chatEntry);

            if (!string.IsNullOrEmpty(team))
            {
                chatEntry.Style.BorderLeftWidth = Length.Pixels(4f);
                chatEntry.Style.BorderLeftColor = TeamFunctions.GetTeam(team).Color;
                chatEntry.Style.Dirty();
            }
        }
    }
    public partial class ChatEntry : Panel
    {
        // public string Name;
        public string Text;

        private TimeSince _chatPosted = 0f;
        public bool IsNew;
        // public readonly Panel HeadHolder;
        // public readonly Image Avatar;
        // public readonly Label NameLabel;
        public readonly Label Message;

        public ChatEntry(string text)
        {
            // HeadHolder = Add.Panel("head");
            // Avatar = HeadHolder.Add.Image();
            // NameLabel = HeadHolder.Add.Label("", "name");
            Message = Add.Label(text, "message");
            IsNew = true;
        }

        // public override void Tick()
        // {
        //     // base.Tick();

        //     // IsNew = _chatPosted < ChatBox.MAX_DISPLAY_TIME;

        //     // bool displayMsg = ChatBox.Instance.IsOpened || IsNew;
        //     // Log.Info(displayMsg);
        //     // SetClass("hide", !displayMsg);
        // }
    }
}
