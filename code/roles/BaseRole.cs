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
        public virtual string Name => "Unknown Role";

        public BaseRole()
        {

        }
    }

    public static class RoleFunctions
    {
        public static List<Type> GetRoles()
        {
            List<Type> roleTypes = new();

            Library.GetAll<BaseRole>().ToList().ForEach(t =>
            {
                if (t.Namespace == "TTTReborn.Roles" && !t.IsAbstract && t.BaseType == typeof(BaseRole))
                {
                    roleTypes.Add(t);
                }
            });

            return roleTypes;
        }

        public static Type GetRoleTypeByName(string roleName)
        {
            foreach (Type roleType in GetRoles())
            {
                if (Library.GetAttribute(roleType).Name == roleName)
                {
                    return roleType;
                }
            }

            return null;
        }

        public static BaseRole GetRoleByType(Type roleType)
        {
            return Library.Create<BaseRole>(roleType);
        }
    }
}
