using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_radar")]
    public partial class Radar : TTTPerk, IBuyableItem
    {
        private const float RADAR_UPDATE_DELAY = 20f;

        private List<RadarPoint> _cachedPoints = new();

        private TimeSince _lastUpdate;

        public int Price => 0;

        public Radar() : base()
        {

        }

        public override void OnRemove()
        {
            base.OnRemove();

            if (Host.IsClient)
            {
                ClearRadarPoints();
            }
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

            ClientSendRadarPositions(To.Single(Owner.GetClientOwner()), Owner as TTTPlayer, positions.ToArray());
        }

        public override void Simulate(Client owner)
        {
            base.Simulate(owner);

            if (Host.IsServer && _lastUpdate >= RADAR_UPDATE_DELAY)
            {
                _lastUpdate = 0f;

                using (Prediction.Off())
                {
                    UpdatePositions();
                }
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
        public static void ClientSendRadarPositions(TTTPlayer player, Vector3[] positions)
        {
            if (!player.IsValid() || player != Local.Pawn)
            {
                return;
            }

            TTTPerk perk = (player.Inventory as Inventory).Perks.Find("ttt_radar");

            if (perk is not Radar radar)
            {
                return;
            }

            radar.ClearRadarPoints();

            foreach (Vector3 vector3 in positions)
            {
                radar._cachedPoints.Add(new RadarPoint(vector3));
            }
        }
    }
}
