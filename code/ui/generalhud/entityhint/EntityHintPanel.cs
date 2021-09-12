using System;

using Sandbox.UI;
using Sandbox.UI.Construct;


namespace TTTReborn.UI
{
    public class EntityHintPanel : Panel
    {
        public Panel TopPanel;
        public Panel BottomPanel;
        public Label TopLabel;
        public Label BottomLabel;

        public Action<EntityHintPanel> DoTick;

        public override void Tick()
        {
            base.Tick();
            Enabled = true; //If our panel exists, it should always be showing. Panels are disgarded immediately when the player looks away.
            DoTick?.Invoke(this); //Invoke our action if it exists.
        }

        public EntityHintPanel(string title = "", string subtitle = null)
        {
            StyleSheet.Load("/ui/generalhud/entityhint/EntityHintPanel.scss");

            //These are reversed, but it works. I don't ask questions.
            BottomPanel = new();
            BottomPanel.AddClass("topWrapper");

            TopPanel = new();
            TopPanel.AddClass("bottomWrapper");

            TopLabel = TopPanel.Add.Label(title, "topLabel");

            //Can't string.empty this. We only check this because if we leave it blank, there will be an empty space below the top label.
            if (subtitle != null)
            {
                BottomLabel = BottomPanel.Add.Label(subtitle, "bottomLabel");
            }

            Enabled = false; //Hide until first tick.
        }

        /// <summary>
        /// Add a method to be ran every UI Tick with the current EntityHintPanel as the parameter.
        /// </summary>
        /// <param name="tick">Method to run</param>
        /// <returns>Current panel</returns>
        public EntityHintPanel WithTick(Action<EntityHintPanel> tick)
        {
            DoTick = tick;
            return this;
        }

        /// <summary>
        /// Add a style rule to the main panel. Consider adding styles directly to panels/labels if this fails. Styles are automatically appended with `!important`.
        /// </summary>
        /// <param name="property">CSS property to modify</param>
        /// <param name="value">Value to set CSS property to</param>
        /// <returns>Current panel</returns>
        public EntityHintPanel WithStyle(string property, string value)
        {
            if (!value.EndsWith(" !important"))
            {
                value += " !important";
            }

            Style.Set(property, value);
            Style.Dirty();
            return this;
        }

        /// <summary>
        /// Enables a default background style
        /// </summary>
        /// <returns>Current panel</returns>
        public EntityHintPanel WithBackground()
        {
            Style.Set("background-color", "#000c !important");
            Style.Set("border-radius", "4px !important");
            Style.Set("border", "2px solid #000c !important");
            Style.Dirty();
            return this;
        }
    }
}
