using Sandbox;

namespace TTTReborn.Events
{
    public partial class PlayerGameEvent : ParameterlessGameEvent
    {
        public int Ident { get; set; }

        public TTTReborn.Player Player
        {
            get => Utils.GetPlayerByIdent(Ident);
        }

        public PlayerGameEvent(TTTReborn.Player player) : base()
        {
            Ident = player.NetworkIdent;
        }

        public override void Run() => Event.Run(Name, Player);
    }
}
