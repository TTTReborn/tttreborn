using System;

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

        public TTTTeam DefaultTeam { get; protected set; }

        public virtual int DefaultCredits => 0;

        public TTTRole()
        {
            Name = Utils.GetTypeName(GetType());
        }

        public virtual void OnSelect(TTTPlayer player)
        {
            player.Credits = Math.Max(DefaultCredits, player.Credits);
        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }

        public virtual bool CanBuy() => false;
    }
}
