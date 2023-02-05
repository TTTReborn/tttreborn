using Sandbox;

namespace TTTReborn
{
	public class Unstuck
	{
		public BasePlayerController Controller;

		public bool IsActive; // replicate

		internal int StuckTries = 0;

		public Unstuck(BasePlayerController controller)
		{
			Controller = controller;
		}

		public virtual bool TestAndFix()
		{
			TraceResult result = Controller.TraceBBox( Controller.Position, Controller.Position );

			// Not stuck, we cool
			if (!result.StartedSolid)
			{
				StuckTries = 0;

				return false;
			}

			if (result.StartedSolid)
			{
				if (BasePlayerController.Debug)
				{
					DebugOverlay.Text($"[stuck in {result.Entity}]", Controller.Position, Color.Red);
					DebugOverlay.Box(result.Entity, Color.Red);
				}
			}

			//
			// Client can't jiggle its way out, needs to wait for
			// server correction to come
			//
			if (Game.IsClient)
            {
				return true;
            }

			int AttemptsPerTick = 20;

			for (int i = 0; i < AttemptsPerTick; i++)
			{
				Vector3 pos = Controller.Position + Vector3.Random.Normal * (StuckTries / 2.0f);

				// First try the up direction for moving platforms
				if (i == 0)
				{
					pos = Controller.Position + Vector3.Up * 5;
				}

				result = Controller.TraceBBox(pos, pos);

				if (!result.StartedSolid)
				{
					if (BasePlayerController.Debug)
					{
						DebugOverlay.Text($"unstuck after {StuckTries} tries ({StuckTries* AttemptsPerTick} tests)", Controller.Position, Color.Green, 5.0f);
						DebugOverlay.Line(pos, Controller.Position, Color.Green, 5.0f, false);
					}

					Controller.Position = pos;

					return false;
				}
				else
				{
					if (BasePlayerController.Debug)
					{
						DebugOverlay.Line(pos, Controller.Position, Color.Yellow, 0.5f, false);
					}
				}
			}

			StuckTries++;

			return true;
		}
	}
}
