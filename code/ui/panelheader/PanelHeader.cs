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

using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class PanelHeader : Panel
    {
        public Action<PanelHeader> OnClose { get; set; }

        private TranslationLabel _title;

        private Button _closeButton;

        public PanelHeader(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/panelheader/PanelHeader.scss");

            Reload();
        }

        public void Reload()
        {
            DeleteChildren(true);

            _title = Add.TryTranslationLabel("", "title");

            OnCreateHeader();

            _closeButton = Add.Button("╳", "closeButton", () =>
            {
                OnClose?.Invoke(this);
            });
        }

        public void SetTitle(string text)
        {
            _title.Text = text;
        }

        public void SetTranslationTitle(string translationKey, params object[] translationData)
        {
            _title.SetTranslation(translationKey, translationData);
        }

        public virtual void OnCreateHeader()
        {

        }
    }
}
