using Sandbox;

namespace TTTReborn
{
	/// <summary>
	/// An entity that can be carried in the player's inventory and hands.
	/// </summary>
	[Title("Carriable"), Icon("luggage")]
	public class BaseCarriable : AnimatedEntity
	{
		public virtual string ViewModelPath => null;
		public BaseViewModel ViewModelEntity { get; protected set; }

		public override void Spawn()
		{
			base.Spawn();

			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public virtual bool CanCarry(Entity carrier)
		{
			return true;
		}

		public virtual void OnCarryStart(Entity carrier)
		{
			if (Game.IsClient)
            {
                return;
            }

			SetParent(carrier, true);

			Owner = carrier;
			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		public virtual void SimulateAnimator(CitizenAnimationHelper anim)
		{
			anim.HoldType = CitizenAnimationHelper.HoldTypes.Pistol;
			anim.Handedness = CitizenAnimationHelper.Hand.Both;
			anim.AimBodyWeight = 1.0f;
		}

		public virtual void OnCarryDrop(Entity dropper)
		{
			if (Game.IsClient)
            {
                return;
            }

			SetParent(null);

			Owner = null;
			EnableDrawing = true;
			EnableAllCollisions = true;
		}

		/// <summary>
		/// This entity has become the active entity. This most likely
		/// means a player was carrying it in their inventory and now
		/// has it in their hands.
		/// </summary>
		public virtual void ActiveStart(Entity ent)
		{
			EnableDrawing = true;

			//
			// If we're the local player (clientside) create viewmodel
			// and any HUD elements that this weapon wants
			//
			if (IsLocalPawn)
			{
				DestroyViewModel();
				DestroyHudElements();

				CreateViewModel();
				CreateHudElements();
			}
		}

		/// <summary>
		/// This entity has stopped being the active entity. This most
		/// likely means a player was holding it but has switched away
		/// or dropped it (in which case dropped = true)
		/// </summary>
		public virtual void ActiveEnd(Entity ent, bool dropped)
		{
			//
			// If we're just holstering, then hide us
			//
			if (!dropped)
			{
				EnableDrawing = false;
			}

			if (Game.IsClient)
			{
				DestroyViewModel();
				DestroyHudElements();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (Game.IsClient && ViewModelEntity.IsValid())
			{
				DestroyViewModel();
				DestroyHudElements();
			}
		}

		/// <summary>
		/// Create the viewmodel. You can override this in your base classes if you want
		/// to create a certain viewmodel entity.
		/// </summary>
		public virtual void CreateViewModel()
		{
			Game.AssertClient();

			if (string.IsNullOrEmpty(ViewModelPath))
            {
				return;
            }

            ViewModelEntity = new BaseViewModel
            {
                Position = Position,
                Owner = Owner,
                EnableViewmodelRendering = true
            };
            ViewModelEntity.SetModel(ViewModelPath);
		}

		/// <summary>
		/// We're done with the viewmodel - delete it
		/// </summary>
		public virtual void DestroyViewModel()
		{
			ViewModelEntity?.Delete();
			ViewModelEntity = null;
		}

		public virtual void CreateHudElements(){}

		public virtual void DestroyHudElements(){}

		/// <summary>
		/// Utility - return the entity we should be spawning particles from etc
		/// </summary>
		public virtual ModelEntity EffectEntity => (ViewModelEntity.IsValid() && IsFirstPersonMode) ? ViewModelEntity : this;
	}
}
