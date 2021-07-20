using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;
using TTTReborn.Player.Camera;

namespace TTTReborn.UI
{
    public partial class ObservablePanel : Panel
    {
        public static readonly List<ObservablePanel> List = new();

        public bool AutoHideWithoutObservablePlayer { get; set; } = false;

        public TTTPlayer ObservedPlayer
        {
            get
            {
                return _observedPlayer;
            }
            internal set
            {
                if (_observedPlayer != value)
                {
                    TTTPlayer oldObservedPlayer = _observedPlayer;

                    _observedPlayer = value;

                    OnUpdatedObservedPlayer(oldObservedPlayer);
                }
            }
        }

        private TTTPlayer _observedPlayer;

        public ObservablePanel() : base()
        {
            UpdateObservedPlayer();

            List.Add(this);
        }

        public override void Delete(bool immediate = false)
        {
            List.Remove(this);

            base.Delete(immediate);
        }

        public bool IsObserving
        {
            get
            {
                return ObservedPlayer != Local.Pawn;
            }
        }

        public static TTTPlayer GetObservedPlayer(Entity pawn)
        {
            if (pawn is not TTTPlayer player)
            {
                return null;
            }

            if (player.LifeState == LifeState.Alive)
            {
                return player;
            }

            if (player.Camera is ThirdPersonSpectateCamera spectateCamera)
            {
                return spectateCamera.TargetPlayer;
            }

            return null;
        }

        public void UpdateObservedPlayer()
        {
            ObservedPlayer = GetObservedPlayer(Local.Pawn);
        }

        public virtual void OnUpdatedObservedPlayer(TTTPlayer oldPlayer)
        {
            if (!AutoHideWithoutObservablePlayer)
            {
                return;
            }

            Style.Display = ObservedPlayer == null ? DisplayMode.None : DisplayMode.Flex;
            Style.Dirty();
        }
    }
}
