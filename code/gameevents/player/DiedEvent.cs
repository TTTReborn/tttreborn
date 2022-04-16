using System.IO;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Teams;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_died"), Hammer.Skip]
    public partial class DiedEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public string LastAttackerName { get; set; }

        public bool IsAttackerPlayer { get; set; }

        public TranslationData GetDescriptionTranslationData()
        {
            if (Player != null && LastAttackerName != null)
            {
                if (LastAttackerName != PlayerName)
                {
                    return new(GetTranslationKey(IsAttackerPlayer ? "BYPLAYER" : "BYOBJECT"), PlayerName ?? "???", LastAttackerName ?? "???");
                }
                else
                {
                    return new(GetTranslationKey("BYSUICIDE"), PlayerName ?? "???");
                }
            }

            return new(GetTranslationKey("NOREASON"), PlayerName ?? "???");
        }

        /// <summary>
        /// Occurs when a player dies.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player who died.</para>
        /// </summary>
        public DiedEvent(TTTReborn.Player player) : base(player)
        {
            if (player != null && player.LastAttacker != null)
            {
                IsAttackerPlayer = player.LastAttacker is TTTReborn.Player;

                if (IsAttackerPlayer)
                {
                    LastAttackerName = player.LastAttacker.Client?.Name ?? "???";
                }
                else
                {
                    LastAttackerName = player.LastAttacker.Name;
                }
            }
        }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public DiedEvent() : base() { }

        protected override void OnRegister()
        {
            if (Player == null || !Player.IsValid || Player.LastAttacker is not TTTReborn.Player attacker)
            {
                return;
            }

            GameEventScoring attackerScoring = new(attacker);

            if (attacker.IsTeamMember(Player))
            {
                attackerScoring.Score = -2;
                attackerScoring.Karma = -100;
            }
            else if (attacker == Player)
            {
                attackerScoring.Score = -1;
            }
            else
            {
                attackerScoring.Score = 1;
                attackerScoring.Karma = 50;
            }

            Scoring = new GameEventScoring[]
            {
                new(Player)
                {
                    Score = -1
                },
                attackerScoring
            };
        }

        public bool Contains(Client client) => PlayerName == client.Name || LastAttackerName == client.Name;

        public override void WriteData(BinaryWriter binaryWriter)
        {
            base.WriteData(binaryWriter);

            binaryWriter.Write(LastAttackerName);
            binaryWriter.Write(IsAttackerPlayer);
        }

        public override void ReadData(BinaryReader binaryReader)
        {
            base.ReadData(binaryReader);

            LastAttackerName = binaryReader.ReadString();
            IsAttackerPlayer = binaryReader.ReadBoolean();
        }
    }
}
