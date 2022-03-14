using System;

using Sandbox;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WeaponAttribute : CarriableAttribute
    {
        public string PrimaryAmmoName { get; private set; }
        public string SecondaryAmmoName { get; private set; }

        public Type PrimaryAmmoType { get; set; }
        public Type SecondaryAmmoType { get; set; }

        public WeaponAttribute(CarriableCategories category, bool defaultAmmoType = true) : base(category)
        {
            if (defaultAmmoType)
            {
                PrimaryAmmoType = GetAmmoType(Category);
            }

            if (PrimaryAmmoType != null && Library.GetAttribute(PrimaryAmmoType) != null)
            {
                PrimaryAmmoName = Utils.GetLibraryName(PrimaryAmmoType);
            }

            if (SecondaryAmmoType != null && Library.GetAttribute(SecondaryAmmoType) != null)
            {
                SecondaryAmmoName = Utils.GetLibraryName(SecondaryAmmoType);
            }
        }

        public static Type GetAmmoType(CarriableCategories category)
        {
            return category switch
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
}
