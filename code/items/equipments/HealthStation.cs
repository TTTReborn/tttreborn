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

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    /// <summary>
    /// Healthkit equipment definition, for the physical entity, see items/equipments/entities/HealthstationEntity.cs
    /// </summary>
    [Library("equipment_healthstation")]
    [Equipment(SlotType = SlotType.UtilityEquipment)]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class HealthStation : TTTEquipment
    {
        public override string ViewModelPath => "";

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            if (Owner is not TTTPlayer owner || !IsServer)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    owner.Inventory.DropEntity(this, typeof(HealthstationEntity));
                }
            }
        }

        public override bool CanDrop() => false;
    }
}
