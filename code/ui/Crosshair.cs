using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Crosshair : TTTPanel
    {
        public class Properties
        {
            public bool ShowTop { get; private set; }
            public bool ShowDot { get; private set; }
            public bool ShowOutline { get; private set; }

            public uint Size { get; private set; }
            public uint Thickness { get; private set; }
            public uint OutlineThickness { get; private set; }

            public int Gap { get; private set; }

            public float ChargeTime { get; private set; }

            public Color Color { get; private set; }

            public Properties(
                bool showTop = true,
                bool showDot = false,
                bool showOutline = false,
                uint size = 20,
                uint thickness = 2,
                uint outlineThickness = 0,
                int gap = 2,
                float chargeTime = 0,
                Color? color = null)
            {
                ShowTop = showTop;
                ShowDot = showDot;
                ShowOutline = showOutline;
                Size = size;
                Thickness = thickness;
                OutlineThickness = outlineThickness;
                Gap = gap;
                ChargeTime = chargeTime;
                Color = color ?? Color.White;
            }
        }

        public static Crosshair Current;

        private Panel ChargeBar;
        private float ChargeTime;

        public Crosshair()
        {
            Current = this;
            StyleSheet.Load("/ui/Crosshair.scss");
        }

        public TTTPanel SetupCrosshair(Properties crosshairProperties)
        {
            int crossHairLinesToCreate = crosshairProperties.ShowTop ? 4 : 3;

            for (int i = 0; i < crossHairLinesToCreate; i++)
            {
                bool isHorizontal = i % 2 == 0;
                Panel crossHairLine = Add.Panel("element");
                crossHairLine.Style.BackgroundColor = crosshairProperties.Color;
                crossHairLine.Style.Width = isHorizontal
                    ? crosshairProperties.Size
                    : crosshairProperties.Thickness;
                crossHairLine.Style.Height = isHorizontal ? crosshairProperties.Thickness : crosshairProperties.Size;

                switch (i)
                {
                    case 0: // Left element
                        crossHairLine.Style.Left = Length.Pixels(crosshairProperties.Size + crosshairProperties.Gap);
                        break;
                    case 1: // Bottom element
                        crossHairLine.Style.Top = Length.Pixels(crosshairProperties.Size + crosshairProperties.Gap);
                        break;
                    case 2: // Right element
                        crossHairLine.Style.Left = Length.Pixels(-crosshairProperties.Size - crosshairProperties.Gap);
                        break;
                    case 3: // Top element
                        crossHairLine.Style.Top = Length.Pixels(-crosshairProperties.Size - crosshairProperties.Gap);
                        break;
                }

                if (crosshairProperties.ShowOutline)
                {
                    crossHairLine.Style.BorderColor = Color.Black;
                    crossHairLine.Style.BorderWidth = crosshairProperties.OutlineThickness;
                }

                crossHairLine.Style.Dirty();
            }

            if (crosshairProperties.ShowDot)
            {
                Panel dot = Add.Panel("element");
                dot.Style.BackgroundColor = crosshairProperties.Color;
                dot.Style.Width = crosshairProperties.Thickness;
                dot.Style.Height = crosshairProperties.Thickness;
                dot.Style.Dirty();
            }

            if (crosshairProperties.ChargeTime > 0)
            {
                ChargeTime = crosshairProperties.ChargeTime;

                Panel chargeBarOutline = Add.Panel("element");
                chargeBarOutline.Style.BorderWidth = 1;
                chargeBarOutline.Style.BorderColor = Color.Black;
                chargeBarOutline.Style.Top = crosshairProperties.Size * 2.5f;
                chargeBarOutline.Style.Width = crosshairProperties.Size * 5f;
                chargeBarOutline.Style.Height = crosshairProperties.Size;
                chargeBarOutline.Style.Dirty();
                ChargeBar = chargeBarOutline.Add.Panel("element");
                ChargeBar.Style.BackgroundColor = crosshairProperties.Color;
                ChargeBar.Style.Height = Length.Percent(100);
                ChargeBar.Style.Dirty();
            }

            Style.Dirty();

            return this;
        }

        private float ChargeStartTime = 0;
        public override void Tick()
        {
            base.Tick();

            TTTPlayer player = Local.Pawn as TTTPlayer;

            if (player == null)
            {
                return;
            }

            BaseCarriable carr = player.ActiveChild as BaseCarriable;

            this.Style.Display = carr == null ? DisplayMode.None : DisplayMode.Flex;

            if (ChargeTime > 0)
            {
                if (Input.Down(InputButton.Attack1))
                {
                    if (ChargeStartTime == 0)
                    {
                        ChargeStartTime = Time.Now;
                    }
                    ChargeBar.Style.Width = Length.Percent(MathX.Clamp((Time.Now - ChargeStartTime) / ChargeTime, 0, 1) * 100);
                    ChargeBar.Style.Dirty();
                }
                else
                {
                    ChargeStartTime = 0;
                    ChargeBar.Style.Width = Length.Percent(0);
                    ChargeBar.Style.Dirty();
                }
            }
        }
    }
}
