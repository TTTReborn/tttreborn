using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.Teams;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_radar")]
    [Buyable(Price = 0)]
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

                foreach (TTTPlayer player in Globals.Utils.GetPlayers((pl) => pl.LifeState == LifeState.Alive))
                {
                    if (player.Client.UserId == owner.Client.UserId)
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
