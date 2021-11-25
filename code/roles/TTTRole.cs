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
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RoleAttribute : LibraryAttribute
    {
        public RoleAttribute(string name) : base("role_" + name)
        {

        }
    }

    public abstract class TTTRole
    {
        public readonly string Name;

        public virtual Color Color => Color.Black;

        public virtual TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(NoneTeam));

        public virtual int DefaultCredits => 50;

        public static Dictionary<string, Shop> ShopDict { get; internal set; } = new();

        public virtual bool IsSelectable => true;

        public Shop Shop
        {
            get
            {
                ShopDict.TryGetValue(Name, out Shop shop);

                return shop;
            }
            internal set
            {
                ShopDict[Name] = value;
            }
        }

        public TTTRole()
        {
            Name = Utils.GetLibraryName(GetType());
        }

        public virtual void OnSelect(TTTPlayer player)
        {
            player.Credits = Math.Max(DefaultCredits, player.Credits);

            if (Host.IsServer)
            {
                player.Shop = Shop;
                player.ServerUpdateShop();
            }

            Event.Run(TTTEvent.Player.Role.Select, player);
        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }

        // serverside function
        public virtual void InitShop()
        {
            Shop.Load(this);
        }

        public virtual void CreateDefaultShop()
        {

        }

        public virtual void UpdateDefaultShop(List<Type> newItemsList)
        {

        }

        public string GetRoleTranslationKey(string key)
        {
            return $"{Name.ToUpper()}_{key}";
        }
    }
}
