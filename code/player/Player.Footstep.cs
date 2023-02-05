using Sandbox;

namespace TTTReborn;

public partial class Player
{
	private TimeSince timeSinceLastFootstep = 0;

	/// <summary>
	/// A footstep has arrived!
	/// </summary>
	public override void OnAnimEventFootstep(Vector3 pos, int foot, float volume)
	{
		if (LifeState != LifeState.Alive || !Game.IsClient || timeSinceLastFootstep < 0.2f)
        {
			return;
        }

		volume *= FootstepVolume();

		timeSinceLastFootstep = 0;

		//DebugOverlay.Box(1, pos, -1, 1, Color.Red);
		//DebugOverlay.Text(pos, $"{volume}", Color.White, 5);

		TraceResult tr = Trace.Ray(pos, pos + Vector3.Down * 20)
			.Radius(1)
			.Ignore(this)
			.Run();

		if (!tr.Hit)
        {
            return;
        }

		tr.Surface.DoFootstep(this, tr, foot, volume);
	}

	/// <summary>
	/// Allows override of footstep sound volume.
	/// </summary>
	/// <returns>The new footstep volume, where 1 is full volume.</returns>
	public virtual float FootstepVolume() => Velocity.WithZ(0).Length.LerpInverse(0.0f, 200.0f) * 0.2f;
}
