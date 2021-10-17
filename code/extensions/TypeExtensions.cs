using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTReborn.Extensions
{
    public static class TypeExtensions
    {

        public static bool HasAttribute<T>(this Type type) where T : Attribute => type.HasAttribute<T>(false);

        public static bool HasAttribute<T>(this Type type, bool inherit) where T : Attribute => type.IsDefined(typeof(T), inherit);

    }
}
