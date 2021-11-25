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
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class RadarPoint : Panel
    {
        private readonly Vector3 _position;
        private readonly Label _distanceLabel;
        private const int BLUR_RADIUS = 10;

        public RadarPoint(Radar.RadarPointData data)
        {
            _position = data.Position;

            StyleSheet.Load("/items/perks/radar/RadarPoint.scss");

            RadarDisplay.Instance.AddChild(this);

            AddClass("circular");

            _distanceLabel = Add.Label();
            _distanceLabel.AddClass("distance-label");
            _distanceLabel.AddClass("text-shadow");

            Style.BackgroundColor = data.Color;
            Style.BoxShadow = new ShadowList()
            {
                new Shadow
                {
                    Blur = BLUR_RADIUS,
                    Color = data.Color
                }
            };
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            _distanceLabel.Text = $"{Globals.Utils.SourceUnitsToMeters(player.Position.Distance(_position)):n0}m";

            Vector3 screenPos = _position.ToScreen();
            Enabled = screenPos.z > 0f;

            if (!Enabled)
            {
                return;
            }

            Style.Left = Length.Fraction(screenPos.x);
            Style.Top = Length.Fraction(screenPos.y);
        }
    }

    public class RadarDisplay : Panel
    {
        public static RadarDisplay Instance { get; set; }

        public RadarDisplay() : base()
        {
            Instance = this;

            AddClass("fullscreen");

            Style.ZIndex = -1;
        }
    }
}
