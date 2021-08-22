using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class DNAScannerDisplay : TTTPanel
    {

        public DNAScannerDisplay()
        {
            IsShowing = false;
            StyleSheet.Load("/ui/alivehud/dnascanner/DNAScannerDisplay.scss");
        }
    }
}
