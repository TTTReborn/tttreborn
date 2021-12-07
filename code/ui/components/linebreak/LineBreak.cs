namespace TTTReborn.UI
{
    public class LineBreak : Panel
    {
        public LineBreak()
        {
            AddClass("linebreak");
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class LineBreakConstructor
    {
        public static LineBreak TranslationButton(this PanelCreator self)
        {
            LineBreak lineBreak = new();

            self.panel.AddChild(lineBreak);

            return lineBreak;
        }
    }
}
