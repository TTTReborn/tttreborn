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
    public partial class DialogBox : Modal
    {
        public Action OnAgree { get; set; }
        public Action OnDecline { get; set; }

        private Button _agreeButton;
        private Button _declineButton;

        public DialogBox(Sandbox.UI.Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/components/modal/dialogbox/DialogBox.scss");

            AddClass("dialogbox");

            Header.DragHeader.IsLocked = true;

            _agreeButton = Footer.Add.ButtonWithIcon("done", "", "agree", OnClickAgree);
            _declineButton = Footer.Add.ButtonWithIcon("close", "", "decline", OnClickDecline);
        }

        public virtual void OnClickAgree()
        {
            OnAgree?.Invoke();
        }

        public virtual void OnClickDecline()
        {
            OnDecline?.Invoke();
        }

        public Label AddText(string text)
        {
            return Content.Add.Label(text, "text");
        }

        public Label AddTranslationText(string translationKey, params object[] translationData)
        {
            return Content.Add.TranslationLabel(translationKey, "text", translationData);
        }
    }
}
