using Sandbox;
using Sandbox.Html;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class TranslationTabContainer : TabContainer, ITranslatable
    {
        public TranslationTabContainer() : base()
        {
            TTTLanguage.Translatables.Add(this);
        }

        public override void OnDeleted()
        {
            TTTLanguage.Translatables.Remove(this);

            base.OnDeleted();
        }

        public Tab AddTab(Sandbox.UI.Panel panel, TranslationData tabTitle, string icon = null)
        {
            int index = Tabs.Count;

            TranslationTab tab = new(this, tabTitle, panel, icon);
            Tabs.Add(tab);

            int cookieIndex = string.IsNullOrWhiteSpace(TabCookie) ? -1 : Cookie.Get($"dropdown.{TabCookie}", -1);

            panel.Parent = SheetContainer;

            if (index == 0 || cookieIndex == index)
            {
                SwitchTab(tab, false);
            }
            else
            {
                tab.Active = false;
            }

            return tab;
        }

        public override void OnTemplateSlot(INode element, string slotName, Sandbox.UI.Panel panel)
        {
            if (slotName == "tab")
            {
                AddTab(panel, new TranslationData(element.GetAttribute("tab_key", null)), element.GetAttribute("tabicon", null));
                return;
            }

            base.OnTemplateSlot(element, slotName, panel);
        }

        public void UpdateLanguage(Language language)
        {
            foreach (Tab tab in Tabs)
            {
                if (tab is TranslationTab translationTab)
                {
                    translationTab.Button.Text = language.GetFormattedTranslation(translationTab.translationData);
                }
            }
        }
    }

    public class TranslationTab : TabContainer.Tab
    {
        public readonly TranslationData translationData;

        public TranslationTab(TranslationTabContainer tabControl, TranslationData tabTitle, Sandbox.UI.Panel panel, string icon)
        : base(tabControl, TTTLanguage.ActiveLanguage.GetFormattedTranslation(tabTitle), icon, panel)
        {
            translationData = tabTitle;

            Button.AddClass("translationtab");
        }
    }
}