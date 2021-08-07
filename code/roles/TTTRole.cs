using System;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;
using TTTReborn.Language;

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

        public TTTRole()
        {
            Name = Utils.GetTypeName(GetType());

            if (TeamFunctions.GetTeamByType(DefaultTeamType) == null)
            {
                Utils.GetObjectByType<TTTTeam>(DefaultTeamType);
            }
        }

        public virtual void OnSelect(TTTPlayer player)
        {
            player.Credits = Math.Max(DefaultCredits, player.Credits);

            Event.Run("tttreborn.player.role.onselect", player);
        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }

        public virtual bool CanBuy() => false;

        public string GetRoleTranslation(string key)
        {
            return TTTLanguage.GetActiveLanguage().GetTranslation($"{key}_{Name.ToUpper()}");
        }
    }
}
