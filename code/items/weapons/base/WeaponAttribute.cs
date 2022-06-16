using System;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {
        public Type PrimaryAmmoType { get; set; }
        public Type SecondaryAmmoType { get; set; }

        public WeaponAttribute(CarriableCategories category, bool defaultAmmoType = true) : base(category)
        {
            if (defaultAmmoType)
            {
                PrimaryAmmoType = GetAmmoType(Category);
            }
        }

        public static Type GetAmmoType(CarriableCategories category) => category switch
        {
            CarriableCategories.Pistol => typeof(PistolAmmo),
            CarriableCategories.SMG => typeof(SMGAmmo),
            CarriableCategories.Shotgun => typeof(ShotgunAmmo),
            CarriableCategories.Sniper => typeof(SniperAmmo),
            CarriableCategories.OffensiveEquipment => typeof(OffensiveEquipmentAmmo),
            _ => null
        };
    }
}
