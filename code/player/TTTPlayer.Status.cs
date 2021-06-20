using System;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public float MaxHealth { get; set; } = 100f;

        public void SetHealth(float health)
        {
            Health = Math.Min(health, MaxHealth);
        }
    }
}
