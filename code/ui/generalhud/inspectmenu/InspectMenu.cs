using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class InspectMenu : Panel
    {
        public static InspectMenu Instance;

        private PlayerCorpse _playerCorpse;
        private ConfirmationData _confirmationData;
        private InspectEntry _selectedInspectEntry;

        private readonly InspectEntry _timeSinceDeathEntry;
        private readonly InspectEntry _suicideEntry;
        private readonly InspectEntry _weaponEntry;
        private readonly InspectEntry _headshotEntry;
        private readonly InspectEntry _distanceEntry;
        private readonly List<InspectEntry> _perkEntries;

        private readonly Panel _backgroundPanel;
        private readonly Panel _inspectContainer;
        private readonly Image _avatarImage;
        private readonly Label _playerLabel;
        private readonly TranslationLabel _roleLabel;
        private readonly Panel _inspectIconsPanel;
        private readonly TranslationLabel _inspectDetailsLabel;

        public new bool Enabled
        {
            get => base.IsEnabled;
            set
            {
                base.IsEnabled = value;

                SetClass("fade-in", base.IsEnabled);
                _inspectContainer.SetClass("pop-in", base.IsEnabled);
            }
        }
        public InspectMenu()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/inspectmenu/InspectMenu.scss");

            AddClass("text-shadow");

            _backgroundPanel = new Panel(this);
            _backgroundPanel.AddClass("background-color-secondary");
            _backgroundPanel.AddClass("opacity-medium");
            _backgroundPanel.AddClass("fullscreen");

            _inspectContainer = new Panel(this);
            _inspectContainer.AddClass("inspect-container");

            _avatarImage = _inspectContainer.Add.Image();
            _avatarImage.AddClass("avatar-image");
            _avatarImage.AddClass("box-shadow");
            _avatarImage.AddClass("circular");

            _playerLabel = _inspectContainer.Add.Label(String.Empty);
            _playerLabel.AddClass("player-label");

            _roleLabel = _inspectContainer.Add.TranslationLabel(String.Empty);
            _roleLabel.AddClass("role-label");

            _inspectIconsPanel = new Panel(_inspectContainer);
            _inspectIconsPanel.AddClass("info-panel");

            #region Inspection Icons
            List<InspectEntry> inspectionEntries = new List<InspectEntry>();

            _timeSinceDeathEntry = new InspectEntry(_inspectIconsPanel);
            _timeSinceDeathEntry.Enabled = true; // Time since death is ALWAYS visible
            _timeSinceDeathEntry.SetData("/ui/inspectmenu/time.png", string.Empty);
            inspectionEntries.Add(_timeSinceDeathEntry);

            _suicideEntry = new InspectEntry(_inspectIconsPanel);
            _suicideEntry.Enabled = false;
            inspectionEntries.Add(_suicideEntry);

            _weaponEntry = new InspectEntry(_inspectIconsPanel);
            _weaponEntry.Enabled = false;
            inspectionEntries.Add(_weaponEntry);

            _headshotEntry = new InspectEntry(_inspectIconsPanel);
            _headshotEntry.Enabled = false;
            inspectionEntries.Add(_headshotEntry);

            _distanceEntry = new InspectEntry(_inspectIconsPanel);
            _distanceEntry.Enabled = false;
            inspectionEntries.Add(_distanceEntry);

            _perkEntries = new List<InspectEntry>();

            foreach (InspectEntry entry in inspectionEntries)
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
            #endregion

            _inspectDetailsLabel = _inspectContainer.Add.TranslationLabel();
            _inspectDetailsLabel.AddClass("inspect-details-label");

            Enabled = false;
        }

        public void InspectCorpse(PlayerCorpse playerCorpse)
        {
            if (playerCorpse?.Player == null)
            {
                return;
            }

            _playerCorpse = playerCorpse;

            _avatarImage.SetTexture($"avatar:{_playerCorpse.Player?.Client.SteamId}");

            _playerLabel.Text = _playerCorpse.Player?.Client.Name;

            _roleLabel.SetTranslation(_playerCorpse.Player?.Role.GetRoleTranslationKey("NAME"));
            _roleLabel.Style.FontColor = _playerCorpse.Player?.Role.Color;
            _roleLabel.Style.Dirty();

            SetConfirmationData(_playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks);

            Enabled = true;
        }

        public void SetPlayerData(PlayerCorpse playerCorpse)
        {
            _playerCorpse = playerCorpse;

            SetConfirmationData(playerCorpse.GetConfirmationData(), _playerCorpse.KillerWeapon, _playerCorpse.Perks);
        }

        public void SetConfirmationData(ConfirmationData confirmationData, string killerWeapon, string[] perks)
        {
            _confirmationData = confirmationData;

            _headshotEntry.Enabled = confirmationData.Headshot;
            _headshotEntry.SetData("/ui/inspectmenu/headshot.png", "CORPSE_INSPECT_IDENTIFIER_HEADSHOT");
            _headshotEntry.SetQuickInfo("CORPSE_INSPECT_QUICKINFO_HEADSHOT");

            _suicideEntry.Enabled = confirmationData.Suicide;
            _suicideEntry.SetData(String.Empty, "CORPSE_INSPECT_IDENTIFIER_SUICIDE");
            _suicideEntry.SetQuickInfo("CORPSE_INSPECT_QUICKINFO_SUICIDE");

            _distanceEntry.Enabled = !confirmationData.Suicide;
            _distanceEntry.SetData("/ui/inspectmenu/distance.png", "CORPSE_INSPECT_IDENTIFIER_KILLED", $"{confirmationData.Distance:n0}");
            _distanceEntry.SetQuickInfo("CORPSE_INSPECT_QUICKINFO_DISTANCE", $"{confirmationData.Distance:n0}");

            _weaponEntry.Enabled = !string.IsNullOrEmpty(killerWeapon);
            _weaponEntry.SetData($"/ui/weapons/{killerWeapon}.png", "CORPSE_INSPECT_IDENTIFIER_WEAPON", new TranslationData(killerWeapon.ToUpper()));
            _weaponEntry.SetQuickInfo(killerWeapon.ToUpper());

            // Clear and delete all perks
            foreach (InspectEntry perkEntry in _perkEntries)
            {
                perkEntry.Delete();
            }

            _perkEntries.Clear();

            // Populate perk entries
            if (perks != null)
            {
                foreach (string perkName in perks)
                {
                    InspectEntry perkEntry = new(this);
                    perkEntry.SetData($"/ui/weapons/{perkName}.png", "CORPSE_INSPECT_IDENTIFIER_PERK", perkName);

                    _perkEntries.Add(perkEntry);
                }
            }
        }

        private void UpdateCurrentInspectDescription()
        {
            _inspectDetailsLabel.SetClass("fade-in", _selectedInspectEntry != null);

            if (_selectedInspectEntry == null)
            {
                return;
            }

            _inspectDetailsLabel.SetTranslation(_selectedInspectEntry.DescriptionTranslationKey, _selectedInspectEntry.Data);
        }

        public override void Tick()
        {
            if (Enabled && _playerCorpse?.Transform.Position.Distance(Local.Pawn.Owner.Position) > 100f)
            {
                Enabled = false;
            }

            string timeSinceDeath = Globals.Utils.TimerString(Time.Now - _confirmationData.Time);
            _timeSinceDeathEntry.SetTranslationData("CORPSE_INSPECT_IDENTIFIER_TIMESINCEDEATH", timeSinceDeath);
            _timeSinceDeathEntry.SetQuickInfo("CORPSE_INSPECT_QUICKINFO_TIME", timeSinceDeath);

            if (_selectedInspectEntry != null && _selectedInspectEntry == _timeSinceDeathEntry)
            {
                UpdateCurrentInspectDescription();
            }
        }
    }
}
