using Sandbox.UI;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class ChatEntry : Panel
    {
        public Image Avatar { get; set; }
        public Panel Message { get; set; }
        public Label Header { get; set; }
        public Label Content { get; set; }
        public ChatBox.Channel Channel { get; set; }
    }
}
