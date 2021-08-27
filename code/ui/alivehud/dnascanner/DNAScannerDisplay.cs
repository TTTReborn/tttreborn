using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class DNAScannerDisplay : TTTPanel
    {
        public static DNAScannerDisplay Instance;

        public Content ScannerContent { get; set; }

        public DNAScannerDisplay()
        {
            Instance = this;
            IsShowing = true;
            StyleSheet.Load("/ui/alivehud/dnascanner/DNAScannerDisplay.scss");

            ScannerContent = new(this);
        }

        //There's something interesting to consider in this section,
        //This is nothing more than "wrapper" functions to access pieces of Content.
        //And even then, content has to be public for some of these to even work.
        //I don't think there is any issue if I just...delete these, and move everything
        //up from `Content` into `DNAScannerDisplay`, but I am too afraid to try.
        public void ChangeSelectedSlot() => ScannerContent.ChangeSelectedSlot();
        public void DisplayDNA(DNAType dna) => ScannerContent.DisplayDNA(dna);
        public void RemoveDNA() => ScannerContent.RemoveDNA();
        public bool HasDNAInCurrentSlot() => ScannerContent.HasDNAInCurrentSlot();
        public int CurrentSlot => ScannerContent.SelectedSlot;
        public void UpdateScannerCharge(float charge)
        {
            ScannerContent.Charge.SetValue(charge);
            ScannerContent.Charge.SetText($"Charge: {charge.ToString("0.00")}%");
        }

        public class Content : TTTPanel
        {
            private List<DNAPanel> DNASlots;
            public int SelectedSlot = 0;

            private Panel _header;
            private Panel _wrapper;
            private Panel _footer;

            public ProgressBar Charge;

            public bool HasDNAInCurrentSlot() => DNASlots[SelectedSlot].HasDNA;

            public Content(TTTPanel parent)
            {
                Parent = parent;

                _header = Add.Panel("header");
                _wrapper = Add.Panel("wrapper");
                _footer = Add.Panel("footer");

                //Don't need to set these to variables. Never needed ever again.
                _header.Add.Label("Collected DNA Samples:");
                _footer.Add.Label("Select a sample to search for that DNA.");

                //Disgusting ProgressBar implementation.
                Charge = _footer.AddChild<ProgressBar>();
                Charge.SetValue(1);
                Charge.SetText("Charge: ");
                Charge.SetColorAtMax(Color.Green); //new Color(r, g, b) doesn't work. Sets it to white.

                _footer.Style.Dirty();

                //Delete this in favor of something else? How else are people going to know the controls?
                _footer.Add.Label("Left click to collect DNA. Right click to change slot.");
                _footer.Add.Label("Reload to clear selected slot.");

                //I am not ashamed by this. TODO: Set this to pull from DNARegistry.MAXIMUMSLOTS
                //TODO: unfuck the UI if you have any number different than 4.
                DNASlots = new()
                {
                    new DNAPanel(_wrapper),
                    new DNAPanel(_wrapper),
                    new DNAPanel(_wrapper),
                    new DNAPanel(_wrapper),
                };
            }

            public override void Tick()
            {
                base.Tick();
                DNASlots[SelectedSlot].IconPanel.SetClass("selected", true);
            }
                    
            public void DisplayDNA(DNAType dna)
            {
                //Find an open slot.
                DNAPanel slot = DNASlots.FirstOrDefault(x => !x.HasDNA);

                //Server should have caught that we didn't have an open slot. Fail but warn.
                if (slot == null)
                {
                    Log.Warning("Received request to display DNA type but no open slot.");
                    return;
                }

                //Flag our slot as containing DNA.
                slot.HasDNA = true;

                //Set our background to our passed enum. This should really be a full URL as noted in the calling method.
                slot.IconPanel.Style.Background = new PanelBackground
                {
                    Texture = Texture.Load($"/ui/dna/from_{dna.ToString().ToLower()}.png")
                };
                slot.IconPanel.Style.Dirty();
            }

            public void ChangeSelectedSlot()
            {
                //We're not giving advanced control. So clicking moves.
                //If you're upset that you can't go left, go around.
                DNASlots[SelectedSlot].IconPanel.SetClass("selected", false);

                SelectedSlot += 1;

                if(SelectedSlot >= 4)
                {
                    SelectedSlot = 0;
                }
            }

            public void RemoveDNA()
            {
                //We need to remove a DNA from display, so let's find it.
                DNAPanel slot = DNASlots[SelectedSlot];

                //Flag it as appropriate.
                slot.HasDNA = false;

                //I don't want to straight up delete the DNAPanel, so set the background to transparent.
                //The server tracks the specific values in each slot, the HasDNA flag is only for selection assistance.
                slot.IconPanel.Style.Background = new PanelBackground
                {
                    Texture = Texture.Transparent
                };
                slot.IconPanel.Style.Dirty();
            }

            private class DNAPanel : TTTPanel
            {
                public Panel IconPanel { get; set; }
                public bool HasDNA = false;

                public DNAPanel(Panel parent)
                {
                    Parent = parent;

                    IconPanel = Add.Panel("icon");

                    //Have nothing in here by default.
                    IconPanel.Style.Background = new PanelBackground
                    {
                        Texture = Texture.Transparent
                    };
                    IconPanel.Style.Dirty();
                }
            }
        }
    }
}
