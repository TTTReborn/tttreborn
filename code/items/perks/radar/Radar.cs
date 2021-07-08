using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_radar")]
    public partial class Radar : TTTPerk, IBuyableItem
    {
        private static List<RadarPoint> _cachedPoints = new();

        private static TimeSince _lastUpdate;

        private const float RADAR_UPDATE_DELAY = 20f;

        public int Price => 0;

        public Radar() : base()
        {

        }

        public override void OnEquip()
        {
            base.OnEquip();

            if (Host.IsServer)
            {
                UpdatePositions();
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();

            ClearRadarPoints();
        }

        private void UpdatePositions()
        {
            List<Vector3> positions = new();

            foreach (TTTPlayer player in Globals.Utils.GetAlivePlayers())
            {
                if (player != Owner)
                {
                    positions.Add(player.Position);
                }
            }

            Log.Error($"Send data to {Owner.GetClientOwner().Name}, Server?: {Host.IsServer}");

            Radar.ClientSendRadarPositions(To.Single(Owner.GetClientOwner()), positions.ToArray());
        }

        public override void Simulate(Client owner)
        {
            base.Simulate(owner);

            if (Host.IsServer && _lastUpdate >= RADAR_UPDATE_DELAY)
            {
                _lastUpdate = 0f;

                UpdatePositions();
            }
        }

        [ClientRpc]
        public static void ClientSendRadarPositions(Vector3[] positions)
        {
            Log.Error("Received data");

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            ClearRadarPoints();

            foreach (Vector3 vector3 in positions)
            {
                _cachedPoints.Add(new RadarPoint(vector3));
            }
        }

        public static void ClearRadarPoints()
        {
            foreach (RadarPoint radarPoint in _cachedPoints)
            {
                radarPoint.Delete();
            }

            _cachedPoints.Clear();
        }
    }
}
