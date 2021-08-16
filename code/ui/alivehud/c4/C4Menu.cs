using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.UI.Construct;

namespace TTTReborn.UI
{
    public class C4Menu : TTTPanel
    {
        private C4ControlPanel _controlPanel;
        private C4Entity _entity;

        public C4Menu()
        {
            IsShowing = false;

            StyleSheet.Load("/ui/alivehud/c4/C4Menu.scss");

            _controlPanel = new C4ControlPanel(this);
        }

        public void Open(C4Entity entity)
        {
            _entity = entity;

            switch(entity.State)
            {
                case C4State.Armed:
                    // Here goes the defuse minigame
                    break;

                default:
                    _controlPanel.Open();
                    break;
            }
        }

        private class C4ControlPanel : TTTPanel
        {
            private readonly Header _header;

            public C4ControlPanel(Panel parent)
            {
                IsShowing = false;

                Parent = parent;

                _header = new Header(this);
            }

            private class Header : TTTPanel
            {
                public Label _title;
                public Button _closeButton;

                public Header(Panel parent)
                {
                    Parent = parent;

                    _title = Add.Label("C4 Controls", "title");
                    _closeButton = Add.Button("X", () => { ((C4ControlPanel)Parent).Close(); });
                }
            }
        }
    }
}
