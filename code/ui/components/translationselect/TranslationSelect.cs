using System;

using Sandbox.Html;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationSelect : DropDown
    {
        public override bool OnTemplateElement(INode element)
        {
            Options.Clear();

            foreach (INode child in element.Children)
            {
                if (!child.IsElement)
                {
                    continue;
                }

                //
                // 	<select> <-- this DropDown control
                //		<option value="#f00">Red</option> <-- option
                //		<option value="#ff0">Yellow</option> <-- option
                //		<option value="#0f0">Green</option> <-- option
                // </select>
                //
                if (child.Name.Equals("option", StringComparison.OrdinalIgnoreCase))
                {
                    Option o = new Option();

                    o.Title = TTTLanguage.ActiveLanguage.TryFormattedTranslation(child.GetAttribute("key"), true, Array.Empty<object>());
                    o.Value = child.GetAttribute("value", o.Title);
                    o.Icon = child.GetAttribute("icon", null);

                    Options.Add(o);
                }
            }

            Select(Value);
            return true;
        }
    }
}
