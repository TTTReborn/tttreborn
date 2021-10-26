using System;

namespace TTTReborn.Items
{

    /// <summary>
    /// Prevents this Entity to be spawned from a TTTWeaponRandom
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SpawnableAttribute : Attribute
    {

    }
}
