using TTTReborn.Hints;

namespace TTTReborn.UI
{
    public abstract class EntityHintPanel : Panel
    {
        public virtual void UpdateHintPanel(IEntityHint hint)
        {
            hint.UpdateHintPanel(this);
        }
    }
}
