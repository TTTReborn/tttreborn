using Sandbox;
using Sandbox.UI;

using TTTReborn.UI.Menu;

namespace TTTReborn.UI
{
    public class BindingKeyImage : Image
    {
        public InputButton InputButton { get; set; }

        public BindingKeyImage(InputButton inputButton) : base()
        {
            InputButton = inputButton;
            Texture = Input.GetGlyph(InputButton, InputGlyphSize.Small, GlyphStyle.Light.WithNeutralColorABXY());

            AddClass("glyph");
        }

        public override void Tick()
        {
            base.Tick();

            Texture texture = Input.GetGlyph(InputButton, InputGlyphSize.Small, GlyphStyle.Light.WithNeutralColorABXY());

            if (Texture != texture)
            {
                Texture = texture;

                if (Parent is BindingPanel bindingPanel)
                {
                    bindingPanel.FinalLayout(0);
                }
            }
        }
    }
}
