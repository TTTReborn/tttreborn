namespace TTTReborn.Events.Shop
{
    [GameEvent("shop_change")]
    public partial class ChangeEvent : ParameterlessGameEvent
    {
        /// <summary>
        /// Occurs when the shop is changed.
        /// </summary>
        public ChangeEvent() : base() { }
    }
}
