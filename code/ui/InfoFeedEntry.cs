using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
	public partial class InfoFeedEntry : Panel
	{
		public List<Label> Labels { get; internal set; } = new();

		public RealTimeSince TimeSinceBorn = 0;

		public InfoFeedEntry()
		{

		}

        public Label AddLabel(string text, string classname)
        {
            Label label = Add.Label(text, classname);

            Labels.Add(label);

            return label;
        }

		public override void Tick()
		{
			base.Tick();

			if (TimeSinceBorn > 6)
			{
				Delete();
			}
		}
	}
}
