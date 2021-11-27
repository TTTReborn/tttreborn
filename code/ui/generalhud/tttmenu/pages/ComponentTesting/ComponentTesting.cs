using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ComponentTesting : Panel
    {
        public bool CheckedValue { get; set; } = true;
        public float FloatValue { get; set; } = 0f;
    }
}
