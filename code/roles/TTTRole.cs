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

        public abstract string DefaultTeamName { get; }

        public virtual int DefaultCredits => 0;

        public TTTRole()
        {
            Name = Utils.GetTypeName(GetType());

            DefaultTeam = TTTTeam.GetTeam(DefaultTeamName);
            DefaultTeam.Color = Color;
        }

        public virtual void OnSelect(TTTPlayer player)
        {
            player.Credits = Math.Max(DefaultCredits, player.Credits);

            if (player.IsTeamVoiceChatEnabled)
            {
                player.ClientToggleTeamVoiceChat(To.Single(player), false);
            }
        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }

        public virtual bool CanBuy() => false;
    }
}
