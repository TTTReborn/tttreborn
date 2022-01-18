using Sandbox;

using SWB_Base;

namespace TTTReborn.Items
{
    [Library("weapon_knife")]
    [Weapon(SlotType = SlotType.Melee)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/swb/hands/swat/v_hands_swat.vmdl", "weapons/swb/melee/bayonet/v_bayonet.vmdl", "weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    [Hammer.EditorModel("weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    public class Knife : TTTWeaponBaseMelee
    {
        public override int Bucket => 0;
        public override HoldType HoldType => HoldType.Fists; // just use fists for now
        public override string HandsModelPath => "weapons/swb/hands/swat/v_hands_swat.vmdl";
        public override string ViewModelPath => "weapons/swb/melee/bayonet/v_bayonet.vmdl";
        public override AngPos ViewModelOffset => new()
        {
            Angle = new Angles(0, -15, 0),
            Pos = new Vector3(-4, 0, 0)
        };
        public override string WorldModelPath => "weapons/swb/melee/bayonet/w_bayonet.vmdl";
        public override string Icon => "/swb_weapons/textures/bayonet.png";
        public override int FOV => 75;
        public override float WalkAnimationSpeedMod => 1.25f;

        public override string SwingAnimationHit => "swing";
        public override string SwingAnimationMiss => "swing_miss";
        public override string StabAnimationHit => "stab";
        public override string StabAnimationMiss => "stab_miss";
        public override string SwingSound => "bayonet.hit";
        public override string StabSound => "bayonet.stab";
        public override string MissSound => "bayonet.slash";
        public override string HitWorldSound => "bayonet.hitwall";
        public override float SwingSpeed => 0.5f;
        public override float StabSpeed => 1f;
        public override float SwingDamage => 25f;
        public override float StabDamage => 50f;
        public override float SwingForce => 25f;
        public override float StabForce => 50f;
        public override float DamageDistance => 35f;
        public override float ImpactSize => 10f;

        public Knife()
        {
            UISettings = new UISettings
            {
                ShowCrosshairLines = false,
                ShowHitmarker = false,
                ShowAmmoCount = false,
                ShowFireMode = false
            };
        }
    }
}
