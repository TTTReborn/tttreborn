// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Nameplate : EntityHintPanel
    {
        public TTTPlayer Player;

        private readonly Sandbox.UI.Panel _labelHolder;
        private readonly Sandbox.UI.Panel _nameHolder;
        private readonly Label _nameLabel;
        private readonly Label _damageIndicatorLabel;

        private struct HealthGroup
        {
            public string Title;
            public Color Color;
            public int MinHealth;

            public HealthGroup(string title, Color color, int minHealth)
            {
                Title = title;
                Color = color;
                MinHealth = minHealth;
            }
        }

        // Pay attention when adding new values! The highest health-based entry has to be the first item, etc.
        private HealthGroup[] HealthGroupList = new HealthGroup[]
        {
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 66),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 33),
            new HealthGroup("Near Death", Color.FromBytes(252, 42, 42), 0)
        };

        public Nameplate(TTTPlayer player) : base()
        {
            Player = player;

            StyleSheet.Load("/ui/generalhud/nameplate/Nameplate.scss");

            AddClass("text-shadow");

            _labelHolder = Add.Panel("label-holder");

            _nameHolder = _labelHolder.Add.Panel("name-holder");
            _nameLabel = _nameHolder.Add.Label("", "name");

            _damageIndicatorLabel = _labelHolder.Add.Label("", "damage-indicator");

            Enabled = false;
        }

        private HealthGroup GetHealthGroup(float health)
        {
            foreach (HealthGroup healthGroup in HealthGroupList)
            {
                if (health >= healthGroup.MinHealth)
                {
                    return healthGroup;
                }
            }

            return HealthGroupList[^1];
        }

        public override void UpdateHintPanel(TranslationData translationData)
        {
            SetClass("fade-in", Enabled);

            bool isAlive = false;

            // needed regarding https://github.com/Facepunch/sbox-issues/issues/1197
            try
            {
                isAlive = Player.LifeState == LifeState.Alive;
            }
            catch (Exception e)
            {
                Log.Warning(e.StackTrace);

                return;
            }

            // Network sync workaround
            if (Player.Health == 0 && isAlive)
            {
                _damageIndicatorLabel.Text = "";
            }
            else
            {
                float health = Player.Health / Player.MaxHealth * 100;
                HealthGroup healthGroup = GetHealthGroup(health);

                _damageIndicatorLabel.Style.FontColor = healthGroup.Color;
                _damageIndicatorLabel.Text = healthGroup.Title;
            }

            _nameLabel.Text = Player.Client?.Name ?? "";
        }
    }
}
