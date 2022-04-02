namespace TTTReborn.UI
{
    public class HealthGroup
    {
        public string Title;
        public Color Color;
        public int MinHealth;

        public HealthGroup(string title, Color color, int minHealth)
        {
            Title = title;
            Color = color;
            MinHealth = minHealth;
        }

        // Pay attention when adding new values! The highest health-based entry has to be the first item, etc.
        public static HealthGroup[] GetHealthGroups() => new[]
        {
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 66),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 33),
            new HealthGroup("Near Death", Color.FromBytes(252, 42, 42), 0)
        };

        public static HealthGroup Get(float health)
        {
            HealthGroup[] healthGroups = GetHealthGroups();

            foreach (HealthGroup healthGroup in healthGroups)
            {
                if (health >= healthGroup.MinHealth)
                {
                    return healthGroup;
                }
            }

            return healthGroups[^1];
        }
    }
}
