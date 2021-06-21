using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
	public partial class InfoFeedEntry : Panel
	{
        private readonly List<Label> _labels = new();

        private readonly RealTimeSince _timeSinceBorn = 0;

		public InfoFeedEntry()
		{

		}

        public Label AddLabel(string text, string classname)
        {
            Label label = Add.Label(text, classname);

            _labels.Add(label);

            return label;
        }

		public override void Tick()
		{
			base.Tick();

			if (_timeSinceBorn > 6)
			{
				Delete();
			}
		}
	}
}
