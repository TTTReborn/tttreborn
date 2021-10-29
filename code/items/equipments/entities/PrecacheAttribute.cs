using System;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PrecachedAttribute : Attribute
    {
        public readonly string[] PrecachedFiles;

        public PrecachedAttribute(params string[] precachedFiles) : base()
        {
            PrecachedFiles = precachedFiles;
        }
    }
}
