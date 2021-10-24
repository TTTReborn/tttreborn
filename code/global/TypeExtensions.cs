using System;

namespace TTTReborn.Globals
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<T>(this Type type, bool inherit = false) where T : Attribute => type.IsDefined(typeof(T), inherit);
    }
}
