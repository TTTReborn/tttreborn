using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

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
    public abstract class BaseRole
    {
        public string Name;

        public virtual Color Color => Color.White;

        public BaseRole()
        {
            Name = RoleFunctions.GetRoleName(GetType());
        }
    }

    public static class RoleFunctions
    {
        /// <summary>
        /// Loops through every type derived from `TTTReborn.Roles.BaseRole` and collects non-abstract roles.
        /// </summary>
        /// <returns>List of all available roles</returns>
        public static List<Type> GetRoles()
        {
            List<Type> roleTypes = new();

            Library.GetAll<BaseRole>().ToList().ForEach(t =>
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    roleTypes.Add(t);
                }
            });

            return roleTypes;
        }

        /// <summary>
        /// Get a `Type` of `TTTReborn.Roles.BaseRole` by it's name (`TTTReborn.Roles.RoleAttribute`).
        /// </summary>
        /// <param name="roleName">The name of the `TTTReborn.Roles.RoleAttribute`</param>
        /// <returns>`Type` of `TTTReborn.Roles.BaseRole`</returns>
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
        /// Returns an instance of a `TTTReborn.Roles.BaseRole` object by a `TTTReborn.Roles.BaseRole` `Type`.
        /// </summary>
        /// <param name="roleType">A `TTTReborn.Roles.BaseRole` `Type`</param>
        /// <returns>Instance of a `TTTReborn.Roles.BaseRole` object</returns>
        public static BaseRole GetRoleByType(Type roleType)
        {
            return Library.Create<BaseRole>(roleType);
        }

        /// <summary>
        /// Returns the `TTTReborn.Roles.RoleAttribute`'s `Name` of a given `TTTReborn.Roles.BaseRole`'s `Type`.
        /// </summary>
        /// <param name="roleType">A `TTTReborn.Roles.BaseRole`'s `Type`</param>
        /// <returns>`TTTReborn.Roles.RoleAttribute`'s `Name`</returns>
        public static string GetRoleName(Type roleType)
        {
            return Library.GetAttribute(roleType).Name;
        }
    }
}
