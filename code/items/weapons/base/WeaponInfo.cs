namespace TTTReborn.Items
{
    public partial class WeaponInfo
    {
        public virtual float DeployTime { get; set; } = 0.5f;

        public virtual string DeployAnim { get; set; } = "deploy";
    }
}
