namespace TTTReborn.Events
{
    public static partial class Shop
    {
        [GameEvent("shop_reloaded")]
        public partial class ChangeEvent : ParameterlessGameEvent
        {
            /// <summary>
            /// Occurs when the shop is changed.
            /// <para>No data is passed to this event.</para>
            /// </summary>
            public ChangeEvent() : base() { }
        }
    }
}
