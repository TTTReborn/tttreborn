using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RoleAttribute : LibraryAttribute
    {
        public RoleAttribute(string name) : base("ttt_role_" + name)
        {

        }
    }

    public abstract class Role
    {
        public readonly string Name;

        public virtual Color Color => Color.Black;

        public virtual Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(NoneTeam));

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

        public Role()
        {
            Name = Utils.GetLibraryName(GetType());
        }

        public virtual void OnSelect(Player player)
        {
            player.Credits = Math.Max(DefaultCredits, player.Credits);

            if (Host.IsServer)
            {
                player.Shop = Shop;
                player.ServerUpdateShop();
            }

            Event.Run(TTTEvent.Player.Role.SELECT, player);
        }

        public virtual void OnDeselect(Player player)
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

        public string GetTranslationKey(string key = null) => Utils.GetTranslationKey(Name, key);

        public bool CheckWin(Player player)
        {
            return player.Team != null && player.Team.CheckWin(player);
        }

        public bool CheckPreventWin(Player player)
        {
            return player.Team != null && player.Team.CheckPreventWin(player);
        }
    }
}
