using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class HomePage : Panel
    {
        public void GoToCompontentTesting()
        {
            TTTMenu.Instance.AddPage(new ComponentTesting());
        }
    }
}
