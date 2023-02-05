using Sandbox;

namespace TTTReborn
{
    [Library]
	public class Duck : BaseNetworkable
	{
		public BasePlayerController Controller;

		public bool IsActive; // replicate

		public Duck(BasePlayerController controller)
		{
			Controller = controller;
		}

		public virtual void PreTick()
		{
			bool wants = Input.Down(InputButton.Duck);

			if (wants != IsActive)
			{
				if (wants)
                {
                    TryDuck();
                }
				else
                {
                    TryUnDuck();
                }
			}

			if (IsActive)
			{
				Controller.SetTag("ducked");

				Controller.EyeLocalPosition *= 0.5f;
			}
		}

		protected virtual void TryDuck()
		{
			IsActive = true;
		}

		protected virtual void TryUnDuck()
		{
			TraceResult pm = Controller.TraceBBox(Controller.Position, Controller.Position, originalMins, originalMaxs);

            if (pm.StartedSolid)
            {
                return;
            }

			IsActive = false;
		}

		// Uck, saving off the bbox kind of sucks
		// and we should probably be changing the bbox size in PreTick
		private Vector3 originalMins;
		private Vector3 originalMaxs;

		public virtual void UpdateBBox(ref Vector3 mins, ref Vector3 maxs, float scale)
		{
			originalMins = mins;
			originalMaxs = maxs;

			if (IsActive)
            {
				maxs = maxs.WithZ(36 * scale);
            }
		}

		//
		// Coudl we do this in a generic callback too?
		//
		public virtual float GetWishSpeed()
		{
			if (!IsActive)
            {
                return -1;
            }

            return 96.0f;
		}
	}
}
