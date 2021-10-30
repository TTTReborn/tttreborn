using Sandbox;

namespace TTTReborn.Items
{
    /// <summary>
	/// A prop that physically simulates as a single rigid body. It can be constrained to other physics objects using hinges
	/// or other constraints. It can also be configured to break when it takes enough damage.
	/// Note that the health of the object will be overridden by the health inside the model, to ensure consistent health game-wide.
	/// If the model used by the prop is configured to be used as a prop_dynamic (i.e. it should not be physically simulated) then it CANNOT be
	/// used as a prop_physics. Upon level load it will display a warning in the console and remove itself. Use a prop_dynamic instead.
	/// </summary>
	[Library("ttt_prop_physics")]
    [Hammer.Skip]
    public abstract partial class Prop : Sandbox.Prop
    {
        public abstract string ModelPath { get; }

        public Prop() : base()
        {

        }

        public override void Spawn()
        {
            Tags.Add(IItem.ITEM_TAG);

            base.Spawn();
        }
    }
}

