using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.UI;

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

        private bool ShowUI = true;

        [Event.Hotload]
        public void onHL()
        {
            StoredDNA = new();
        }

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

                if (Input.Pressed(InputButton.Duck))
                {
                    ShowUI = !ShowUI;
                }
            }
        }

        private void CollectDNA()
        {
            if (!IsServer)
            {
                return;
            }

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

            List<DNAInstance> DNAonEnt = Gamemode.Game.Instance.DNA.GetDNA(selected);

            if (DNAonEnt.Any())
            {
                //Truncate our list of collected DNA if we got a bunch more than we can hold.
                int openSlots = DNARegistry.MAXIMUMSLOTS - StoredDNA.Count;

                if (DNAonEnt.Count > openSlots)
                {
                    DNAonEnt.RemoveRange(openSlots, DNAonEnt.Count - openSlots);
                }

                StoredDNA.AddRange(DNAonEnt); //Store on the server side of the weapon our DNA.

                foreach (DNAInstance dna in DNAonEnt)
                {
                    AddNewDNAToUI(To.Single(Owner), dna.Type); //Send each type of DNA to our client for UI. 
                }

                //We've pulled DNA off something, so let's stop tracking it on that entity. You own that DNA now.
                Gamemode.Game.Instance.DNA.Untrack(DNAonEnt);
            }
        }

        [ClientRpc]
        public void AddNewDNAToUI(DNAType dna)
        {
            DNAScannerDisplay.Instance.DisplayDNA(dna);
        }

        private void SwapDNA()
        {
            if (IsServer)
            {
                return;
            }

            DNAScannerDisplay.Instance.ChangeSelectedSlot();
        }

        private void ClearDNA()
        {
            if (IsServer)
            {
                return;
            }

            DNAScannerDisplay.Instance.RemoveDNA();
            RemoveDNAAtIndex(NetworkIdent, DNAScannerDisplay.Instance.CurrentSlot);
        }

        [ServerCmd]
        public static void RemoveDNAAtIndex(int scannerId, int index)
        {
            DNAScanner scanner = FindByIndex(scannerId) as DNAScanner;
            if (scanner == null)
            {
                Log.Warning($"Received request to update DNA for scanner: {scannerId}, however it does not exist.");
            }

            if (!scanner.StoredDNA.ElementAtOrDefault(index).Equals(default(DNAInstance)))
            {
                scanner.StoredDNA.RemoveAt(index);
            }

        }

        public override bool CanDrop() => true;
    }
}
