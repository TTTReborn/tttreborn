using System.ComponentModel;

using Sandbox;

namespace TTTReborn;

public partial class Player
{
	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }
	public Angles OriginalViewAngles { get; private set; }

	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable(false)]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld(EyeLocalPosition);
		set => EyeLocalPosition = Transform.PointToLocal(value);
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable(false)]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable(false)]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld(EyeLocalRotation);
		set => EyeLocalRotation = Transform.RotationToLocal(value);
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable(false)]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new(EyePosition, EyeRotation.Forward);

	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput()
	{
		OriginalViewAngles = ViewAngles;
		InputDirection = Input.AnalogMove;

		if (Input.StopProcessing)
        {
			return;
        }

		Angles look = Input.AnalogLook;

		if (ViewAngles.pitch > 90f || ViewAngles.pitch < -90f)
		{
			look = look.WithYaw(look.yaw * -1f);
		}

		Angles viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp(-89f, 89f);
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

		ActiveChild?.BuildInput();

		GetActiveController()?.BuildInput();
	}
}
