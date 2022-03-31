using Sandbox;

using TTTReborn.Rounds;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_roundchange")]
    public partial class RoundChangeEvent : GameEvent
    {
        public override bool IsLogged { get; set; } = true;

        public BaseRound OldRound { get; set; }

        public BaseRound NewRound { get; set; }

        /// <summary>
        /// Called everytime the round changes.
        /// <para>Event is passed the <strong><see cref="BaseRound"/></strong> instance of the old round.</para>
        /// <para>Event is passed the <strong><see cref="BaseRound"/></strong> instance of the new round.</para>
        /// </summary>
        public RoundChangeEvent(BaseRound oldRound, BaseRound newRound) : base()
        {
            OldRound = oldRound;
            NewRound = newRound;
        }

        public override void Run() => Event.Run(Name, OldRound, NewRound);
    }
}
