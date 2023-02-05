using Sandbox;

namespace TTTReborn
{
	[Library]
	public abstract class BasePlayerController : PawnController
	{
		[ConVar.Replicated("debug_playercontroller")]
		public static bool Debug { get; set; } = false;

		/// <summary>
		/// Any bbox traces we do will be offset by this amount.
		/// todo: this needs to be predicted
		/// </summary>
		public Vector3 TraceOffset;

		/// <summary>
		/// Traces the bbox and returns the trace result.
		/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
		/// position. This is good when tracing down because you won't be tracing through the ceiling above.
		/// </summary>
		public virtual TraceResult TraceBBox(Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f)
		{
			if (liftFeet > 0)
			{
				start += Vector3.Up * liftFeet;
				maxs = maxs.WithZ(maxs.z - liftFeet);
			}

			TraceResult tr = Trace.Ray(start + TraceOffset, end + TraceOffset)
                .Size(mins, maxs)
                .WithAnyTags("solid", "playerclip", "passbullets", "player")
                .Ignore(Pawn)
                .Run();

			tr.EndPosition -= TraceOffset;

			return tr;
		}

		/// <summary>
		/// This calls TraceBBox with the right sized bbox. You should derive this in your controller if you
		/// want to use the built in functions
		/// </summary>
		public virtual TraceResult TraceBBox(Vector3 start, Vector3 end, float liftFeet = 0.0f) => TraceBBox(start, end, Vector3.One * -1, Vector3.One, liftFeet);

		/// <summary>
		/// This is temporary, get the hull size for the player's collision
		/// </summary>
		public virtual BBox GetHull() => new(-10, 10);


		public override void FrameSimulate()
		{
			base.FrameSimulate();

			Player pl = Pawn as Player;
			EyeRotation = pl.ViewAngles.ToRotation();
		}
	}
}
