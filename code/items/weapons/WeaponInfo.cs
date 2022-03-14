namespace TTTReborn.Items
{
    public partial class WeaponInfo : CarriableInfo
    {
        /// <summary>Duration of the draw animation</summary>
        public float DeployTime { get; set; } = 0.5f;

        /// <summary>Duration of the reload animation</summary>
        public float ReloadTime { get; set; } = 1f;

        // Strings //

        /// <summary>Reloading animation</summary>
        public string ReloadAnim { get; set; } = "reload";

        /// <summary>Draw animation</summary>
        public string DeployAnim { get; set; } = "deploy";
    }
}
