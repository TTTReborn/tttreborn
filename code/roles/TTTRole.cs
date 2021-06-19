using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

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
        public string Name;

        public virtual Color Color => Color.Black;

        public virtual TTTTeam DefaultTeam => null;

        public TTTRole()
        {
            Name = RoleFunctions.GetRoleName(GetType());
        }

        public virtual void OnSelect(TTTPlayer player)
        {

        }

        public virtual void OnDeselect(TTTPlayer player)
        {

        }
    }

    public static class RoleFunctions
    {
        /// <summary>
        /// Loops through every type derived from `TTTReborn.Roles.TTTRole` and collects non-abstract roles.
        /// </summary>
        /// <returns>List of all available roles</returns>
        public static List<Type> GetRoles()
        {
            List<Type> roleTypes = new();

            Library.GetAll<TTTRole>().ToList().ForEach(t =>
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    roleTypes.Add(t);
                }
            });

            return roleTypes;
        }

        /// <summary>
        /// Get a `Type` of `TTTReborn.Roles.TTTRole` by it's name (`TTTReborn.Roles.RoleAttribute`).
        /// </summary>
        /// <param name="roleName">The name of the `TTTReborn.Roles.RoleAttribute`</param>
        /// <returns>`Type` of `TTTReborn.Roles.TTTRole`</returns>
        public static Type GetRoleTypeByName(string roleName)
        {
            foreach (Type roleType in GetRoles())
            {
                if (GetRoleName(roleType) == roleName)
                {
                    return roleType;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an instance of a `TTTReborn.Roles.TTTRole` object by a `TTTReborn.Roles.TTTRole` `Type`.
        /// </summary>
        /// <param name="roleType">A `TTTReborn.Roles.TTTRole` `Type`</param>
        /// <returns>Instance of a `TTTReborn.Roles.TTTRole` object</returns>
        public static TTTRole GetRoleByType(Type roleType)
        {
            if (roleType == null)
            {
                return null;
            }

            return Library.Create<TTTRole>(roleType);
        }

        /// <summary>
        /// Returns the `TTTReborn.Roles.RoleAttribute`'s `Name` of a given `TTTReborn.Roles.TTTRole`'s `Type`.
        /// </summary>
        /// <param name="roleType">A `TTTReborn.Roles.TTTRole`'s `Type`</param>
        /// <returns>`TTTReborn.Roles.RoleAttribute`'s `Name`</returns>
        public static string GetRoleName(Type roleType)
        {
            if (roleType == null)
            {
                return null;
            }

            return Library.GetAttribute(roleType).Name;
        }
    }
}
