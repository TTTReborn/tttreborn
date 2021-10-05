using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Item("ttt_radar")]
    public partial class Radar : TTTCountdownPerk, IBuyableItem
    {
        private Vector3 RADARPOINT_UI_OFFSET = Vector3.Up * 45;
        public override float Countdown { get; } = 20f;

        private Vector3[] _lastPositions;

        private List<RadarPoint> _cachedPoints = new();

        public int Price => 0;

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
                List<Vector3> positions = new();

                foreach (TTTPlayer player in Globals.Utils.GetAlivePlayers())
                {
                    if (player != Owner)
                    {
                        positions.Add(player.Position);
                    }
                }

                ClientSendRadarPositions(To.Single(Owner), Owner as TTTPlayer, positions.ToArray());
            }
            else
            {
                ClearRadarPoints();

                foreach (Vector3 vector3 in _lastPositions)
                {
                    _cachedPoints.Add(new RadarPoint(vector3 + RADARPOINT_UI_OFFSET));
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
        public static void ClientSendRadarPositions(TTTPlayer player, Vector3[] positions)
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

            radar._lastPositions = positions;

            radar.UpdatePositions();
        }
    }
}
