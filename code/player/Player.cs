using Sandbox;

namespace TTTReborn;

/// <summary>
/// This is what you should derive your player from. This base exists in addon code
/// so we can take advantage of codegen for replication. The side effect is that we
/// can put stuff in here that we don't need to access from the engine - which gives
/// more transparency to our code.
/// </summary>
[Title("Player"), Icon("emoji_people")]
public partial class Player : AnimatedEntity
{
	/// <summary>
	/// A generic corpse entity
	/// </summary>
	public ModelEntity Corpse { get; set; }

    public static readonly Model WorldModel = Model.Load("models/citizen/citizen.vmdl");

    public Player()
    {
        Inventory = new Inventory(this);
    }

    protected override void OnDestroy()
    {
        RemovePlayerCorpse();

        base.OnDestroy();
    }

	/// <summary>
	/// Create a physics hull for this player. The hull stops physics objects and players passing through
	/// the player. It's basically a big solid box. It also what hits triggers and stuff.
	/// The player doesn't use this hull for its movement size.
	/// </summary>
	public virtual void CreateHull()
	{
		SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));

		//var capsule = new Capsule(new Vector3(0, 0, 16), new Vector3(0, 0, 72 - 16), 32);
		//var phys = SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, capsule);


		//	phys.GetBody(0).RemoveShadowController();

		// TODO - investigate this? if we don't set movetype then the lerp is too much. Can we control lerp amount?
		// if so we should expose that instead, that would be awesome.
		EnableHitboxes = true;
	}
}
