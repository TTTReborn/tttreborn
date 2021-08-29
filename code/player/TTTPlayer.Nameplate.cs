using Sandbox;
using Sandbox.UI;

using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer : IEntityHint
    {
        private readonly Color BORDER_COLOR_NONE = Color.FromBytes(0, 0, 0, 204);

        private struct HealthGroup
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
        }

        // Pay attention when adding new values! The highest health-based entry has to be the first item, etc.
        private HealthGroup[] HealthGroupList = new HealthGroup[]{
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 70),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 20),
            new HealthGroup("Near death", Color.FromBytes(252, 42, 42), 0)
        };

        private HealthGroup GetHealthGroup(float health)
        {
            foreach (HealthGroup healthGroup in HealthGroupList)
            {
                if (health >= healthGroup.MinHealth)
                {
                    return healthGroup;
                }
            }

            return HealthGroupList[HealthGroupList.Length - 1];
        }


        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public TTTPanel DisplayHint(TTTPlayer client)
        {
            return new EntityHintPanel("Name", "Health")
                .WithBackground()
                .WithTick((display) =>
               {
                   if (Health == 0 && LifeState == LifeState.Alive)
                   {
                       display.BottomLabel.SetText(""); // network-sync workaround
                    }
                   else
                   {
                        //Set health group related data on bottom label.
                        float health = Health / MaxHealth * 100;
                       HealthGroup healthGroup = GetHealthGroup(health);

                       display.BottomLabel.Style.FontColor = healthGroup.Color;
                       display.BottomLabel.Text = healthGroup.Title;
                       display.BottomLabel.Style.FontSize = Length.Pixels(12);
                       display.BottomLabel.Style.Dirty();
                   }

                   display.TopLabel.SetText(GetClientOwner()?.Name ?? "");

                   display.Style.BorderColor = Role is not Roles.NoneRole ? Role.Color : BORDER_COLOR_NONE;
                   display.Style.Dirty();
               });
        }
    }
}
