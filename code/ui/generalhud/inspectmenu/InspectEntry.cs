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

using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InspectEntry : Panel
    {
        public string DescriptionTranslationKey;
        public object[] Data;

        private readonly Sandbox.UI.Image _inspectIcon;
        private readonly TranslationLabel _inspectQuickLabel;

        public InspectEntry(Panel parent) : base(parent)
        {
            Parent = parent;

            AddClass("rounded");
            AddClass("text-shadow");
            AddClass("background-color-secondary");

            _inspectIcon = Add.Image();
            _inspectIcon.AddClass("inspect-icon");

            _inspectQuickLabel = Add.TranslationLabel();
            _inspectQuickLabel.AddClass("quick-label");
        }

        public void SetData(string imagePath, string descriptionTranslationKey, params object[] args)
        {
            SetTranslationData(descriptionTranslationKey, args);

            _inspectIcon.Style.BackgroundImage = Texture.Load(imagePath, false) ?? Texture.Load($"/ui/none.png");
        }

        public void SetTranslationData(string descriptionTranslationKey, params object[] args)
        {
            DescriptionTranslationKey = descriptionTranslationKey;
            Data = args;
        }

        public void SetQuickInfo(string translationKey, params object[] args)
        {
            _inspectQuickLabel.SetTranslation(translationKey, args);
        }
    }
}
