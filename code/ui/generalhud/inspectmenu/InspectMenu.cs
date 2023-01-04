using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

#pragma warning disable IDE0051

namespace TTTReborn.UI
{
    [UseTemplate]
    public class InspectMenu : Panel
    {
        public static InspectMenu Instance { get; set; }

        private PlayerCorpse _playerCorpse;
        private ConfirmationData _confirmationData;
        private InspectEntry _selectedInspectEntry;

        private InspectEntry TimeSinceDeathEntry { get; set; }
        private InspectEntry SuicideEntry { get; set; }
        private InspectEntry WeaponEntry { get; set; }
        private InspectEntry HeadshotEntry { get; set; }
        private InspectEntry DistanceEntry { get; set; }
        private readonly List<InspectEntry> _perkEntries;

        private Panel BackgroundPanel { get; set; }
        private Panel InspectContainer { get; set; }
        private Image AvatarImage { get; set; }
        private Label PlayerLabel { get; set; }
        private TranslationLabel RoleLabel { get; set; }
        private Panel InspectIconsPanel { get; set; }
        private TranslationLabel InspectDetailsLabel { get; set; }

        public bool Enabled
        {
            get => this.IsEnabled();
            set
            {
                this.Enabled(value);

                SetClass("fade-in", this.IsEnabled());
                InspectContainer.SetClass("pop-in", this.IsEnabled());
            }
        }

        public InspectMenu()
        {
            Instance = this;

            #region Inspection Icons
            TimeSinceDeathEntry.SetData("assets/inspectmenu/time.png", new TranslationData());
            AddListeners(TimeSinceDeathEntry);

            SuicideEntry.Enabled(false);
            AddListeners(SuicideEntry);

            WeaponEntry.Enabled(false);
            AddListeners(WeaponEntry);

            HeadshotEntry.Enabled(false);
            AddListeners(HeadshotEntry);

            DistanceEntry.Enabled(false);
            AddListeners(DistanceEntry);
            #endregion

            _perkEntries = new List<InspectEntry>();

            Enabled = false;
        }

        private void AddListeners(InspectEntry entry)
        {
            entry.AddEventListener("onmouseover", () =>
            {
                _selectedInspectEntry = entry;

                UpdateCurrentInspectDescription();
            });

            entry.AddEventListener("onmouseout", () =>
            {
                _selectedInspectEntry = null;

                UpdateCurrentInspectDescription();
            });
        }

        public void InspectCorpse(PlayerCorpse playerCorpse)
        {
            if (playerCorpse?.DeadPlayer == null)
            {
                return;
            }

            _playerCorpse = playerCorpse;

            AvatarImage.SetTexture($"avatar:{_playerCorpse.DeadPlayer?.Client.SteamId}");

            PlayerLabel.Text = _playerCorpse.DeadPlayer?.Client.Name;

            RoleLabel.UpdateTranslation(new TranslationData(_playerCorpse.DeadPlayer?.Role.GetTranslationKey("NAME")));
            RoleLabel.Style.FontColor = _playerCorpse.DeadPlayer?.Role.Color;

            SetConfirmationData(_playerCorpse.Data);

            Enabled = true;
        }

        public void SetPlayerData(PlayerCorpse playerCorpse)
        {
            _playerCorpse = playerCorpse;

            SetConfirmationData(playerCorpse.Data);
        }

        public void SetConfirmationData(ConfirmationData data)
        {
            _confirmationData = data;

            HeadshotEntry.Enabled(data.IsHeadshot);
            HeadshotEntry.SetData("assets/inspectmenu/headshot.png", new TranslationData("CORPSE.INSPECT.IDENTIFIER.HEADSHOT"));
            HeadshotEntry.SetQuickInfo(new TranslationData("CORPSE.INSPECT.QUICKINFO.HEADSHOT"));

            SuicideEntry.Enabled(data.IsSuicide);
            SuicideEntry.SetData(string.Empty, new TranslationData("CORPSE.INSPECT.IDENTIFIER.SUICIDE"));
            SuicideEntry.SetQuickInfo(new TranslationData("CORPSE.INSPECT.QUICKINFO.SUICIDE"));

            DistanceEntry.Enabled(!data.IsSuicide);
            DistanceEntry.SetData("assets/inspectmenu/distance.png", new TranslationData("CORPSE.INSPECT.IDENTIFIER.KILLED", $"{data.Distance:n0}"));
            DistanceEntry.SetQuickInfo(new TranslationData("CORPSE.INSPECT.QUICKINFO.DISTANCE", $"{data.Distance:n0}"));

            WeaponEntry.Enabled(!string.IsNullOrEmpty(data.KillerWeapon));

            if (WeaponEntry.IsEnabled())
            {
                TranslationData translationData = new(Utils.GetTranslationKey(data.KillerWeapon, "NAME"));

                WeaponEntry.SetData($"assets/icons/{data.KillerWeapon}.png", new TranslationData("CORPSE.INSPECT.IDENTIFIER.WEAPON", translationData));
                WeaponEntry.SetQuickInfo(translationData);
            }

            // Clear and delete all perks
            foreach (InspectEntry perkEntry in _perkEntries)
            {
                perkEntry.Delete();
            }

            _perkEntries.Clear();

            // Populate perk entries
            if (data.Perks != null)
            {
                foreach (string perkName in data.Perks)
                {
                    InspectEntry perkEntry = new();
                    perkEntry.SetData($"assets/icons/{perkName}.png", new TranslationData("CORPSE.INSPECT.IDENTIFIER.PERK", new TranslationData(Utils.GetTranslationKey(perkName, "NAME"))));

                    _perkEntries.Add(perkEntry);
                    InspectIconsPanel.AddChild(perkEntry);
                    AddListeners(perkEntry);
                }
            }
        }

        private void UpdateCurrentInspectDescription()
        {
            InspectDetailsLabel.SetClass("fade-in", _selectedInspectEntry != null);

            if (_selectedInspectEntry == null)
            {
                return;
            }

            InspectDetailsLabel.UpdateTranslation(_selectedInspectEntry.TranslationData);
        }

        public override void Tick()
        {
            if (!Enabled || !_playerCorpse.IsValid() || _playerCorpse.Transform.Position.Distance(Game.LocalPawn.Owner.Position) > 100f)
            {
                Enabled = false;

                return;
            }

            string timeSinceDeath = Utils.TimerString(Time.Now - _confirmationData.Time);
            TimeSinceDeathEntry.SetTranslationData(new TranslationData("CORPSE.INSPECT.IDENTIFIER.TIMESINCEDEATH", timeSinceDeath));
            TimeSinceDeathEntry.SetQuickInfo(new TranslationData("CORPSE.INSPECT.QUICKINFO.TIME", timeSinceDeath));

            if (_selectedInspectEntry != null && _selectedInspectEntry == TimeSinceDeathEntry)
            {
                UpdateCurrentInspectDescription();
            }
        }
    }
}
