using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PreRound : BaseRound
    {
        public override string RoundName => "Preparing";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.PreRoundTime;
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            _ = StartRespawnTimer(player);

            player.MakeSpectator();

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                List<TTTAmmoRandom> randomAmmo = new();
                List<TTTWeaponRandom> randomWeapons = new();

                foreach (Entity entity in Entity.All)
                {
                    if (entity.Tags.Has(IItem.ITEM_TAG))
                    {
                        entity.Delete();
                    }
                    if (entity is TTTAmmoRandom rammo)
                    {
                        randomAmmo.Add(rammo); //Throws `Collection was Modified` if we activate here. Worth looking further into cleanup wise.
                    }
                    if (entity is TTTWeaponRandom rwep)
                    {
                        randomWeapons.Add(rwep); //See above comment.
                    }
                }

                randomAmmo.ForEach(x => x.Activate());
                randomWeapons.ForEach(x => x.Activate());

                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player)
                    {
                        player.Respawn();
                    }
                }
            }
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            Gamemode.Game.Instance.ChangeRound(new InProgressRound());
        }

        private static async Task StartRespawnTimer(TTTPlayer player)
        {
            await Task.Delay(1000);

            if (player.IsValid() && Gamemode.Game.Instance.Round is PreRound)
            {
                player.Respawn();
            }
        }

        public override void OnPlayerSpawn(TTTPlayer player)
        {
            AddPlayer(player);

            base.OnPlayerSpawn(player);
        }
    }
}
