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

        public Radar() : base()
        {

        }

        public override void OnEquip()
        {
            base.OnEquip();

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            if (Host.IsClient)
            {
                return;
            }

            List<Vector3> positions = new();

            foreach (TTTPlayer player in Globals.Utils.GetAlivePlayers())
            {
                positions.Add(player.Position);
            }

            ClientSendRadarPositions(positions.ToArray());
        }

        public int Price => 0;

        [ClientRpc]
        public static void ClientSendRadarPositions(Vector3[] positions)
        {
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
