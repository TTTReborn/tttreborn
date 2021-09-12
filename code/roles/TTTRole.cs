using System;
using System.Collections.Generic;

using Sandbox;

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

    [RoleAttribute("Base")]
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

            Event.Run("tttreborn.player.role.onselect", player);
        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }

        // serverside function
        public virtual void InitShop()
        {
            Shop = null;

            string fileName = $"settings/server/shop/{Name.ToLower()}.json";

            if (FileSystem.Data.FileExists(fileName))
            {
                Shop = Shop.InitializeFromJSON(FileSystem.Data.ReadAllText(fileName));
            }
        }

        public string GetRoleTranslationKey(string key)
        {
            return $"{key}_{Name.ToUpper()}";
        }
    }
}
