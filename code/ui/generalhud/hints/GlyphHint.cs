using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public struct GlyphHintData
    {
        public TranslationData TranslationData { get; set; }

        public List<InputButton> InputButtons { get; set; }

        public GlyphHintData(TranslationData translationData, params InputButton[] inputButtons)
        {
            TranslationData = translationData;
            InputButtons = new(inputButtons);
        }
    }

    public class GlyphHint : EntityHintPanel
    {
        public GlyphHintData[] Data;

        public GlyphHint(params GlyphHintData[] data) : base()
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("column");
            AddClass("padding");

            Data = data;

            CreateContent();

            this.Enabled(false);
        }

        private void CreateContent()
        {
            DeleteChildren(true);

            for (int glyphCount = 0; glyphCount < Data.Length; glyphCount++)
            {
                GlyphHintData glyphHintData = Data[glyphCount];

                if (glyphHintData.TranslationData == null)
                {
                    continue;
                }

                AddGlyphEntry(glyphHintData);
            }
        }

        private void AddGlyphEntry(GlyphHintData glyphHintData)
        {
            Panel panel = Add.Panel();
            panel.AddClass("text-color-info");
            panel.AddClass("text-shadow");

            for (int i = 0; i < glyphHintData.InputButtons.Count; i++)
            {
                panel.AddChild(new BindingKeyImage(glyphHintData.InputButtons[i]));

                if (i != glyphHintData.InputButtons.Count - 1)
                {
                    Label label = panel.Add.Label(" + ", "text-color-info");
                    label.Style.PaddingTop = 10;
                    label.Style.PaddingLeft = 5;
                }
            }

            TranslationLabel translationLabel = panel.Add.TranslationLabel(glyphHintData.TranslationData);
            translationLabel.Style.Padding = 10;
            translationLabel.Style.PaddingLeft = 15;
        }

        public override void UpdateHintPanel(params TranslationData[] translationData)
        {
            for (int i = 0; i < translationData.Length; i++)
            {
                Data[i].TranslationData = translationData[i];
            }

            CreateContent();
        }
    }
}
