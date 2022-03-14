using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_knife")]
    [Weapon(CarriableCategories.Melee)]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_boneknife/v_rust_boneknife.vmdl", "weapons/rust_boneknife/rust_boneknife.vmdl")]
    [Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
    public partial class Knife : MeleeWeapon
    {
        public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
        public override string ModelPath => "weapons/rust_boneknife/rust_boneknife.vmdl";

        public Knife() : base()
        {
            WeaponInfo.DeployTime = 0.2f;

            Primary.Damage = 45;
            Primary.Rate = 1f;
        }
    }
}
