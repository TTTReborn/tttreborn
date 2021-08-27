using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public partial class ProgressBar : TTTPanel
    {
        private Panel _bar;
        private readonly Label _innerText;
        private readonly Color _backgroundColor;
        private Color _backgroundColorAtMax;
        

        public ProgressBar() : base()
        {
            StyleSheet.Load("/ui/components/progressbar/ProgressBar.scss");
            _bar = Add.Panel("bar");
            _innerText = Add.Label();
            _backgroundColor = Color.Orange;
            _backgroundColorAtMax = _backgroundColor;
        }

        public override void Tick()
        {
            base.Tick();
            if (_bar.Style.Width == Length.Percent(100))
            {
                _bar.Style.BackgroundColor = _backgroundColorAtMax;
            }
            else
            {
                _bar.Style.BackgroundColor = _backgroundColor;
            }
        }

        public void SetValue(float value)
        {
            _bar.Style.Width = Length.Percent(value);
            _bar.Style.Dirty();
        }

        public void SetColorAtMax(Color color) => _backgroundColorAtMax = color;

        public void SetText(string value)
        {
            _innerText.SetText(value);
            Style.Dirty();
        }
    }
}
