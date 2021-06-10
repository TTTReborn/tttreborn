using System;
using System.Reflection;
using System.Collections.Generic;

namespace TTTReborn.Roles
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RoleAttribute : Attribute
    {
        public string Name { get; set; }
    }

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
            Type[] types = Assembly
                .GetExecutingAssembly()
                .GetTypes();

            List<Type> roleTypes = new();

            foreach (Type t in types)
            {
                if (t.Namespace == "TTTReborn.Roles" && !t.IsAbstract && t.BaseType == typeof(BaseRole))
                {
                    roleTypes.Add(t);
                }
            }

            return roleTypes;
        }

        public static Type GetRoleTypeByName(string roleName)
        {
            foreach (Type roleType in GetRoles())
            {
                foreach (Attribute attribute in System.Attribute.GetCustomAttributes(roleType))
                {
                    if (attribute is RoleAttribute roleAttribute)
                    {
                        if (roleAttribute.Name == roleName)
                        {
                            return roleType;
                        }

                        // If it failed, skip to the next role
                        break;
                    }
                }
            }

            return null;
        }

        public static BaseRole GetRoleByType(Type roleType)
        {
            return (BaseRole) Activator.CreateInstance(roleType);
        }
    }
}
