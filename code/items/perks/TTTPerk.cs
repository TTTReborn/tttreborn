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

using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PerkAttribute : ItemAttribute
    {
        public PerkAttribute() : base()
        {

        }
    }

    [Hammer.Skip]
    public abstract class TTTPerk : IItem
    {
        public string LibraryName { get; }
        public Entity Owner { get; private set; }

        protected TTTPerk()
        {
            LibraryName = Utils.GetLibraryName(GetType());
        }

        public void Equip(TTTPlayer player)
        {
            Owner = player;

            OnEquip();
        }

        public virtual void OnEquip()
        {
            if (Host.IsClient)
            {
                InventoryWrapper.Instance.Effects.AddEffect(this);
            }
        }

        public void Remove()
        {
            OnRemove();
        }

        public virtual void OnRemove()
        {
            Owner = null;
        }

        public void Delete()
        {
            if (Host.IsClient)
            {
                InventoryWrapper.Instance.Effects.RemoveEffect(this);
            }
        }

        public virtual void Simulate(Client owner)
        {

        }
    }
}
