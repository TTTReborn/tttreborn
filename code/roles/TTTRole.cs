using System;
using System.Collections.Generic;
using System.Text.Json;

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
        public RoleAttribute(string name) : base(name)
        {

        }
    }

    [Role("Base")]
    public abstract class TTTRole
    {
        public readonly string Name;

        public virtual Color Color => Color.Black;

        public abstract Type DefaultTeamType { get; }

        public virtual int DefaultCredits => 0;

        public static Dictionary<string, Shop> ShopDict { get; internal set; } = new();

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
            Name = Utils.GetTypeName(GetType());

            if (TeamFunctions.GetTeamByType(DefaultTeamType) == null)
            {
                Utils.GetObjectByType<TTTTeam>(DefaultTeamType);
            }

            using (Prediction.Off())
            {
                if (!ShopDict.ContainsKey(Name))
                {
                    InitShop();
                }
            }
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
            return $"{key}_{Name.ToUpper()}";
        }
    }
}
