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

        private Content _content;

        public DNAScannerDisplay()
        {
            Instance = this;

            IsShowing = true;
            StyleSheet.Load("/ui/alivehud/dnascanner/DNAScannerDisplay.scss");
            _content = new(this);
        }

        public override void Tick()
        {
            base.Tick();
            IsShowing = true;
        }


        public void ChangeSelectedSlot() => _content.ChangeSelectedSlot();

        public void DisplayDNA(DNAType dna) => _content.DisplayDNA(dna);

        public void RemoveDNA() => _content.RemoveDNA();

        public int CurrentSlot => _content.SelectedSlot;

        private class Content : TTTPanel
        {
            private List<DNAPanel> DNASlots;
            public int SelectedSlot = 0;

            private Panel _header;
            private Panel _wrapper;
            private Panel _footer;

            public Content(TTTPanel parent)
            {
                Parent = parent;


                _header = Add.Panel("header");
                _wrapper = Add.Panel("wrapper");
                _footer = Add.Panel("footer");

                _header.Add.Label("Collected DNA Samples:");

                _footer.Add.Label("Select a sample to search for that DNA.");
                _footer.Add.Label("Fancy progress bar that has the scan charge here. woosh.");
                _footer.Add.Label("Left click to collect DNA. Right click to change slot.");
                _footer.Add.Label("Reload to clear selected slot.");


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
                DNAPanel slot = DNASlots.FirstOrDefault(x => !x.HasDNA);

                if (slot == null)
                {
                    Log.Warning("Received request to display DNA type but no open slot.");
                    return;
                }

                slot.HasDNA = true;

                slot.IconPanel.Style.Background = new PanelBackground
                {
                    Texture = Texture.Load($"/ui/dna/from_{dna.ToString().ToLower()}.png")
                };
                slot.IconPanel.Style.Dirty();
            }

            public void ChangeSelectedSlot()
            {
                DNASlots[SelectedSlot].IconPanel.SetClass("selected", false);

                SelectedSlot += 1;

                if(SelectedSlot >= 4)
                {
                    SelectedSlot = 0;
                }
            }

            public void RemoveDNA()
            {
                DNAPanel slot = DNASlots[SelectedSlot];

                slot.HasDNA = false;

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
                    var options = new[]
                    {
                        "weapons",
                        "corpse",
                        "ammo"
                    };

                    Parent = parent;

                    IconPanel = Add.Panel("icon");

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
