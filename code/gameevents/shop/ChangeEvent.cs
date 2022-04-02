namespace TTTReborn.Events.Shop
{
    [GameEvent("shop_change"), Hammer.Skip]
    public partial class ChangeEvent : GameEvent
    {
        /// <summary>
        /// Occurs when the shop is changed.
        /// </summary>
        public ChangeEvent() : base() { }
    }
}
