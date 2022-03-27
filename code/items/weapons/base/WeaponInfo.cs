namespace TTTReborn.Items
{
    public partial class WeaponInfo
    {
        /// <summary>Duration of the draw animation</summary>
        public virtual float DeployTime { get; set; } = 0.5f;

        // Strings //

        /// <summary>Draw animation</summary>
        public virtual string DeployAnim { get; set; } = "deploy";
    }
}
