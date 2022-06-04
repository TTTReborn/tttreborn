namespace TTTReborn.Events.Shop
{
    [GameEvent("shop_change"), HideInEditor]
    public partial class ChangeEvent : GameEvent
    {
        /// <summary>
        /// Occurs when the shop is changed.
        /// </summary>
        public ChangeEvent() : base() { }
    }
}
