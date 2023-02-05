using System.Collections.Generic;

using Sandbox;

using TTTReborn.Camera;

namespace TTTReborn;

public partial class Player
{
    [Net]
    public bool IsForcedSpectator { get; set; } = false;

    private Player _spectatingPlayer;
    public Player CurrentPlayer
    {
        get => _spectatingPlayer ?? this;
        set
        {
            _spectatingPlayer = value == this ? null : value;

            GameEvent.Register(new Events.Player.Spectating.ChangeEvent(this));
        }
    }

    public bool IsSpectatingPlayer
    {
        get => _spectatingPlayer != null;
    }

    public bool IsSpectator
    {
#pragma warning disable CS0184 // 'Der angegebene Ausdruck für den 'is'-Ausdruck darf niemals der angegebene Typ sein
        get => Sandbox.Camera.Current is IObservationCamera;
#pragma warning restore CS0184 // 'Der angegebene Ausdruck für den 'is'-Ausdruck darf niemals der angegebene Typ sein
    }

    private int _targetIdx = 0;

    [Event("player_died")]
    protected static void OnPlayerDied(Player deadPlayer)
    {
        if (!Game.IsClient || Game.LocalPawn is not Player player)
        {
            return;
        }

        if (player.IsSpectatingPlayer && player.CurrentPlayer == deadPlayer)
        {
            player.UpdateObservatedPlayer();
        }
    }

    public void UpdateObservatedPlayer()
    {
        Player oldObservatedPlayer = CurrentPlayer;

        CurrentPlayer = null;

        List<Player> players = Utils.GetAlivePlayers();

        if (players.Count > 0)
        {
            if (++_targetIdx >= players.Count)
            {
                _targetIdx = 0;
            }

            CurrentPlayer = players[_targetIdx];
        }

        //if (Sandbox.Camera.Current is IObservationCamera camera)
        //{
        //    camera.OnUpdateObservatedPlayer(oldObservatedPlayer, CurrentPlayer);
        //}
    }

    public void MakeSpectator(bool useRagdollCamera = true)
    {
        EnableAllCollisions = false;
        EnableDrawing = false;
        Controller = null;
        // CameraMode = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();
        LifeState = LifeState.Dead;
        Health = 0f;
        ShowFlashlight(false, false);
    }

    public void ToggleForcedSpectator()
    {
        IsForcedSpectator = !IsForcedSpectator;

        if (IsForcedSpectator && LifeState == LifeState.Alive)
        {
            MakeSpectator(false);
            OnKilled();

            if (!Client.GetValue("forcedspectator", false))
            {
                Client.SetValue("forcedspectator", true);
            }
        }
        else if (Gamemode.TTTGame.Instance.Round is not Rounds.InProgressRound && Gamemode.TTTGame.Instance.Round is not Rounds.PostRound)
        {
            Respawn();
        }
    }
}
