// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    public class Hint : EntityHintPanel
    {
        private readonly TranslationLabel _label;

        public Hint(TranslationData translationData)
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            _label = Add.TranslationLabel(translationData.Key, string.Empty, translationData.Data);
            _label.Style.Padding = 10;

            Enabled = false;
        }

        public override void UpdateHintPanel(TranslationData translationData)
        {
            if (translationData == null)
            {
                return;
            }

            _label.SetTranslation(translationData.Key, translationData.Data);
        }
    }
}
