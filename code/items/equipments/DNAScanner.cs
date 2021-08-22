using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_dnascanner")]
    partial class DNAScanner : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public string ModelPath => "models/entities/huladoll.vmdl";
        public override SlotType SlotType => SlotType.Secondary;
        public virtual int Price => 0;

        private List<DNAInstance> StoredDNA = new();
        private const int SelectDistance = 200;

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
            CollisionGroup = CollisionGroup.Weapon;
            SetInteractsAs(CollisionLayer.Debris);
        }

        public override void Simulate(Client client)
        {
            if (!IsServer || Owner is not TTTPlayer player)
            {
                return;
            }

            
            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    CollectDNA();
                }

                if (Input.Pressed(InputButton.Attack2))
                {
                    SwapDNA();
                }

                if (Input.Pressed(InputButton.Reload))
                {
                    ClearDNA();
                }
            }
        }

        private void CollectDNA()
        {
            TraceResult selectionTrace = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * SelectDistance)
                     .Ignore(Owner)
                     .UseHitboxes()
                     .Run();

            if (!selectionTrace.Hit)
            {
                return;
            }

            Entity selected;

            if (selectionTrace.Entity.IsWorld)
            {
                List<Entity> entities = Physics.GetEntitiesInSphere(selectionTrace.EndPos, 10.0f).ToList();
                if (!entities.Any())
                {
                    return;
                }

                selected = entities[0];
            }
            else
            {
                selected = selectionTrace.Entity;
            }

            var DNAonEnt = Gamemode.Game.Instance.DNA.GetDNA(selected);

            if (DNAonEnt.Count > 0)
            {
                StoredDNA.AddRange(DNAonEnt);
            }
        }

        private void SwapDNA()
        {
            Log.Error("Swapping DNA");
        }

        private void ClearDNA()
        {
            Log.Error("Clearing DNA");
        }

        public override bool CanDrop() => true;
    }
}
