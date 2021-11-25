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

using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.Teams;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("perk_radar")]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class Radar : TTTCountdownPerk
    {
        public struct RadarPointData
        {
            public Color Color;
            public Vector3 Position;
        }

        public override float Countdown { get; } = 20f;

        private RadarPointData[] _lastPositions;
        private readonly List<RadarPoint> _cachedPoints = new();
        private readonly Color _defaultRadarColor = Color.FromBytes(124, 252, 0);
        private readonly Vector3 _radarPointOffset = Vector3.Up * 45;

        public Radar() : base()
        {

        }

        public override void OnRemove()
        {
            if (Host.IsClient)
            {
                ClearRadarPoints();
            }

            base.OnRemove();
        }

        private void UpdatePositions()
        {
            if (Host.IsServer)
            {
                if (Owner is not TTTPlayer owner)
                {
                    return;
                }

                List<RadarPointData> pointData = new();

                foreach (TTTPlayer player in Globals.Utils.GetAlivePlayers())
                {
                    if (player.Client.PlayerId == owner.Client.PlayerId)
                    {
                        continue;
                    }

                    pointData.Add(new RadarPointData
                    {
                        Position = player.Position + _radarPointOffset,
                        Color = player.Team.Name == owner.Team.Name ? owner.Team.Color : _defaultRadarColor
                    });
                }

                if (owner.Team is not TraitorTeam)
                {
                    List<Vector3> decoyPositions = Entity.All.Where(x => x.GetType() == typeof(DecoyEntity))?.Select(x => x.Position).ToList();

                    foreach (Vector3 decoyPosition in decoyPositions)
                    {
                        pointData.Add(new RadarPointData
                        {
                            Position = decoyPosition + _radarPointOffset,
                            Color = _defaultRadarColor
                        });
                    }
                }

                ClientSendRadarPositions(To.Single(Owner), owner, pointData.ToArray());
            }
            else
            {
                ClearRadarPoints();

                foreach (RadarPointData pointData in _lastPositions)
                {
                    _cachedPoints.Add(new RadarPoint(pointData));
                }
            }
        }

        public override void OnEquip()
        {
            base.OnEquip();

            if (Host.IsServer)
            {
                UpdatePositions();
            }
        }

        public override void OnCountdown()
        {
            if (Host.IsServer)
            {
                UpdatePositions();
            }
        }

        private void ClearRadarPoints()
        {
            foreach (RadarPoint radarPoint in _cachedPoints)
            {
                radarPoint.Delete();
            }

            _cachedPoints.Clear();
        }

        [ClientRpc]
        public static void ClientSendRadarPositions(TTTPlayer player, RadarPointData[] points)
        {
            if (!player.IsValid() || player != Local.Pawn)
            {
                return;
            }

            Radar radar = player.Inventory.Perks.Find<Radar>();

            if (radar == null)
            {
                return;
            }

            radar._lastPositions = points;
            radar.UpdatePositions();
        }
    }
}
