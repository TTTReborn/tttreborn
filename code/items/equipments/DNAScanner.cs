using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_dnascanner")]
    public partial class DNAScanner : TTTEquipment, IBuyableItem
    {
        public override string ViewModelPath => "";
        public string ModelPath => "models/entities/huladoll.vmdl";
        public override SlotType SlotType => SlotType.Secondary;

        //Remove IBuyableItem once in loadout for detective.
        public virtual int Price => 0;

        //Managed by the server.
        private List<DNAInstance> StoredDNA { get; set; } = new();

        //Client UI point for DNA
        private DNAScanPoint ScanPoint = null;

        [Net]
        public float ScannerCharge { get; set; } = 1.0f;

        [ServerVar("ttt_dna_recharge_rate", Help = "The recharge rate of the DNA scanner.")]
        public static float TTTDNARechargeRate { get; set; } = 0.07f;

        //Distance our left-click trace can reach.
        private const int SELECTDISTANCE = 200;

        //Used to flag if we're waiting on data to be returned from the server to prevent a double-scan attempt being transmitted to the server.
        public bool IsScanning = false;

        private bool ShowUI = true;

        [Event.Hotload]
        public void ONHL()
        {
            ScannerCharge = 95;
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
            ScannerCharge += TTTDNARechargeRate;
            ScannerCharge = MathX.Clamp(ScannerCharge, 5, 100); //Clamp to minimum of 5 so the UI looks okay at low values. We will never remove more than 95 charge.

            if (IsClient)
            {
                DNAScannerDisplay.Instance.UpdateScannerCharge(ScannerCharge); //Update UI with current charge %.
            }

            //If we aren't currently waiting on the server for previous scan data, and we're ready to go, let's try to activate one.
            if (ScannerCharge == 100 && IsClient && !IsScanning)
            {
                //If our current selected slot has DNA in it
                if (DNAScannerDisplay.Instance.HasDNAInCurrentSlot())
                {
                    //Signal to the server to send us data.
                    ActivateScan(NetworkIdent, DNAScannerDisplay.Instance.CurrentSlot);
                    //Flag us not to run this function again until data has been received.
                    IsScanning = true;
                }
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

                if (IsServer)
                {
                    return;
                }
                //This works, but it's gross lol. Apparently the OnEquip functions do not work.
                if (Input.Pressed(InputButton.Duck))
                {
                    
                    DNAScannerDisplay.Instance.IsShowing = !DNAScannerDisplay.Instance.IsShowing;
                }
            }
        }

        private void CollectDNA()
        {
            if (!IsServer)
            {
                return;
            }

            TraceResult selectionTrace = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * SELECTDISTANCE)
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
                //Since we hit the world, let's do a small sphere check just incase they barely missed an entity.
                List<Entity> entities = Physics.GetEntitiesInSphere(selectionTrace.EndPos, 10.0f).ToList();

                if (!entities.Any())
                {
                    return;
                }

                selected = entities[0]; //If our sphere has anything, let's just take the first one.
            }
            else
            {
                selected = selectionTrace.Entity;
            }

            //Request the DNA registry for all elements from our found entity.
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
                    //Send each type of DNA to our client for UI.
                    //We can't send DNAInstance due to it being a struct with unhappy types.
                    //We don't really need to indicate much to the client at all besides "hey, you have DNA here".
                    //In future, this should pull a URL to an image for whatever weapon icon, corpse placeholder or ammo (if that stays a thing).
                    AddNewDNAToUI(To.Single(Owner), dna.Type); 
                }

                //We've pulled DNA off something, so let's stop tracking it on that entity. You own that DNA now.
                Gamemode.Game.Instance.DNA.Untrack(DNAonEnt);
            }
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

            //Delete the DNA off our UI.
            DNAScannerDisplay.Instance.RemoveDNA();
            //Signal to the server to delete our current DNA from the list.
            RemoveDNAAtIndex(NetworkIdent, DNAScannerDisplay.Instance.CurrentSlot);
        }

        //We've received new DNA from the server. Let's display it.
        [ClientRpc]
        public void AddNewDNAToUI(DNAType dna)
        {
            DNAScannerDisplay.Instance.DisplayDNA(dna);
        }

        [ClientRpc]
        public static void AddScanPoint(int scannerId, Vector3 position)
        {
            //The server is telling us we have our requested position to display.
            //Lets find our current scanner, because this has to be static and I fucking hate it.
            DNAScanner scanner = FindByIndex(scannerId) as DNAScanner;
            if (scanner == null)
            {
                Log.Warning($"Received request to activate scan for DNA scanner: {scannerId}, however it does not exist.");
            }

            //Check if we already have a point, if so, delete that shit.
            if (scanner.ScanPoint != null)
            {
                scanner.ScanPoint.Delete();
            }
            //Flag our client that they are no longer waiting on data.
            scanner.IsScanning = false;
            //Create new UI point.
            scanner.ScanPoint = new DNAScanPoint(position);
        }

        [ServerCmd]
        public static void ActivateScan(int scannerId, int index)
        {
            //A client has requested we send them position data for DNA
            DNAScanner scanner = FindByIndex(scannerId) as DNAScanner;
            if (scanner == null)
            {
                Log.Warning($"Received request to activate scan for DNA scanner: {scannerId}, however it does not exist.");
            }

            DNAInstance dna = scanner.StoredDNA[index]; //Find their requested index in their scanner's table on the server.
            scanner.ScannerCharge -= 25; //Reduce charge. Later this will be calculated over distance.
            AddScanPoint(To.Single(scanner.Owner), scannerId, dna.Player.Position); //Tell our client where to place their marker.
        }

        [ServerCmd]
        public static void RemoveDNAAtIndex(int scannerId, int index)
        {
            //A client has requested we remove DNA from their current index. Let's find their scanner.
            DNAScanner scanner = FindByIndex(scannerId) as DNAScanner;
            if (scanner == null)
            {
                Log.Warning($"Received request to remove DNA for scanner: {scannerId}, however it does not exist.");
            }

            //Quick little check to make sure their is data at their index.
            if (!scanner.StoredDNA.ElementAtOrDefault(index).Equals(default(DNAInstance)))
            {
                scanner.StoredDNA.RemoveAt(index);
            }
        }

        public override bool CanDrop() => true;
    }
}
