using Sandbox;

using TTTReborn.Roles;
using TTTReborn.WorldUI;

namespace TTTReborn;

public partial class Player
{
    public bool IsInitialSpawning { get; set; } = false;

    public RoleWorldIcon RoleWorldIcon { get; set; }

	public override void Spawn()
	{
		EnableLagCompensation = true;

		Tags.Add("player");

		base.Spawn();
	}

    // Important: Server-side only
    public void InitialSpawn()
    {
        bool isPostRound = Gamemode.TTTGame.Instance.Round is Rounds.PostRound;

        IsInitialSpawning = true;
        IsForcedSpectator = isPostRound || Gamemode.TTTGame.Instance.Round is Rounds.InProgressRound;

        NetworkableGameEvent.RegisterNetworked(new Events.Player.InitialSpawnEvent(Client));

        Respawn();

        // sync roles
        using (Prediction.Off())
        {
            foreach (Player player in Utils.GetPlayers())
            {
                if (isPostRound || player.IsConfirmed)
                {
                    player.SendClientRole(To.Single(this));
                }
            }

            Client.SetValue("forcedspectator", IsForcedSpectator);
        }

        IsInitialSpawning = false;
        IsForcedSpectator = false;
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        if (IsLocalPawn)
        {
            return;
        }

        RoleWorldIcon = new(this);
    }

    [Event("player_spawn")]
    protected static void OnPlayerSpawn(Player player)
    {
        if (!player.IsValid())
        {
            return;
        }

        player.IsMissingInAction = false;
        player.IsConfirmed = false;
        player.CorpseConfirmer = null;

        player.SetRole(new NoneRole());
    }

	/// <summary>
	/// Sets LifeState to Alive, Health to Max, nulls velocity, and calls Gamemode.PlayerRespawn
	/// </summary>
	// Important: Server-side only
    // TODO: Convert to a player.RPC, event based system found inside of...
    // TODO: https://github.com/TTTReborn/ttt-reborn/commit/1776803a4b26d6614eba13b363bbc8a4a4c14a2e#diff-d451f87d88459b7f181b1aa4bbd7846a4202c5650bd699912b88ff2906cacf37R30
    public virtual void Respawn()
	{
        Model = WorldModel;

        //Animator = new StandardPlayerAnimator();

        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = false;
        EnableDrawing = true;

        Credits = 0;

        SetRole(new NoneRole());

        IsMissingInAction = false;

        using (Prediction.Off())
        {
            NetworkableGameEvent.RegisterNetworked(new Events.Player.SpawnEvent(this));
            SendClientRole();
        }

		Game.AssertServer();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();

		GameManager.Current?.MoveToSpawnpoint(this);
		ResetInterpolation();

        if (!IsForcedSpectator)
        {
            Controller = new DefaultWalkController();
            //CameraMode = new FirstPersonCamera();

            EnableAllCollisions = true;
            EnableDrawing = true;
        }
        else
        {
            MakeSpectator(false);
        }

        RemovePlayerCorpse();
        Inventory.DeleteContents();
        Gamemode.TTTGame.Instance.Round.OnPlayerSpawn(this);

        switch (Gamemode.TTTGame.Instance.Round)
        {
            case Rounds.PreRound:
            case Rounds.WaitingRound:
                IsConfirmed = false;
                CorpseConfirmer = null;

                Client.SetValue("forcedspectator", IsForcedSpectator);

                break;
        }
	}
}
