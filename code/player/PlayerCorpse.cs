using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sandbox;

using TTTReborn.Entities;
using TTTReborn.Globalization;
using TTTReborn.Items;
using TTTReborn.UI;

namespace TTTReborn
{
    public class ConfirmationData
    {
        public int Ident { get; set; }
        public string Name { get; set; }
        public bool IsIdentified { get; set; } = false;
        public bool IsHeadshot { get; set; } = false;
        public bool IsSuicide { get; set; } = false;
        public float Time { get; set; } = 0f;
        public float Distance { get; set; } = 0f;
        public int Credits { get; set; } = 0;
        public string[] Perks { get; set; }
        public float KilledTime { get; set; }
        public string KillerWeapon { get; set; }
        public string RoleName { get; set; }
        public string TeamName { get; set; }
        // TODO damage type

        [JsonIgnore]
        public Player Player
        {
            get => Utils.GetPlayerByIdent(Ident);
        }
    }

    public partial class PlayerCorpse : ModelEntity, IEntityHint
    {
        public Player DeadPlayer { get; set; }
        public List<Particles> Ropes = new();
        public List<PhysicsJoint> RopeSprings = new();
        public List<Player> CovertConfirmers { get; set; } = new();
        public ConfirmationData Data { get; set; } = new();

        public PlayerCorpse()
        {
            MoveType = MoveType.Physics;
            UsePhysicsCollision = true;

            SetInteractsAs(CollisionLayer.Debris);
            SetInteractsWith(CollisionLayer.WORLD_GEOMETRY);
            SetInteractsExclude(CollisionLayer.Player);

            Data.KilledTime = Time.Now;
        }

        public void CopyFrom(Player player)
        {
            DeadPlayer = player;

            Data.Ident = player.NetworkIdent;
            Data.Name = player.Client.Name;
            Data.RoleName = player.Role.Name;
            Data.TeamName = player.Team.Name;
            Data.KillerWeapon = player.LastDamageWeapon?.Info.LibraryName;
            Data.IsHeadshot = player.LastDamageWasHeadshot;
            Data.Distance = player.LastDistanceToAttacker;
            Data.IsSuicide = player.LastAttacker == this;
            Data.Credits = player.Credits;

            PerksInventory perksInventory = player.Inventory.Perks;

            Data.Perks = new string[perksInventory.Count()];

            for (int i = 0; i < Data.Perks.Length; i++)
            {
                Data.Perks[i] = perksInventory.Get(i).Info.LibraryName;
            }

            SetModel(player.GetModelName());
            TakeDecalsFrom(player);

            this.CopyBonesFrom(player);
            this.SetRagdollVelocityFrom(player);

            List<C4Entity> attachedC4s = new();

            foreach (Entity child in player.Children)
            {
                if (child is C4Entity c4 && c4.AttachedBone > -1)
                {
                    attachedC4s.Add(c4);
                }

                if (child is ModelEntity e)
                {
                    string model = e.GetModelName();

                    if (model == null || !model.Contains("clothes"))
                    {
                        continue;
                    }

                    ModelEntity clothing = new();
                    clothing.SetModel(model);
                    clothing.SetParent(this, true);
                }
            }

            foreach (C4Entity c4 in attachedC4s)
            {
                c4.SetParent(this, c4.AttachedBone);
            }
        }

        public void ApplyForceToBone(Vector3 force, int forceBone)
        {
            PhysicsGroup.AddVelocity(force);

            if (forceBone < 0)
            {
                return;
            }

            PhysicsBody corpse = GetBonePhysicsBody(forceBone);

            if (corpse != null)
            {
                corpse.ApplyForce(force * 1000);
            }
            else
            {
                PhysicsGroup.AddVelocity(force);
            }
        }

        public void ClearAttachments()
        {
            foreach (Particles rope in Ropes)
            {
                rope.Destroy(true);
            }

            foreach (PhysicsJoint spring in RopeSprings)
            {
                spring.Remove();
            }

            Ropes.Clear();
            RopeSprings.Clear();
        }

        protected override void OnDestroy()
        {
            ClearAttachments();
        }

        public float HintDistance => 80f;

        public TranslationData[] TextOnTick => new TranslationData[] { new(Data.IsIdentified ? "CORPSE.USE.INSPECT" : "CORPSE.USE.IDENTIFY"), Data.IsIdentified ? null : new("CORPSE.USE.COVERTINSPECT") };

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new GlyphHint(new GlyphHintData[] { new(TextOnTick[0], InputButton.Use), new(TextOnTick[1], InputButton.Duck, InputButton.Use) });

        public void HintTick(Player confirmingPlayer)
        {
            using (Prediction.Off())
            {
                if (IsClient && !Input.Down(InputButton.Use))
                {
                    if (InspectMenu.Instance != null)
                    {
                        InspectMenu.Instance.Enabled = false;
                    }

                    return;
                }

                if (!Data.IsIdentified && confirmingPlayer.LifeState == LifeState.Alive && Input.Down(InputButton.Use))
                {
                    bool covert = Input.Down(InputButton.Duck);

                    if (IsServer)
                    {
                        if (!covert)
                        {
                            Data.IsIdentified = true;
                        }

                        if (!covert || !CovertConfirmers.Contains(confirmingPlayer))
                        {
                            if (covert)
                            {
                                RPCs.ClientConfirmPlayer(To.Single(confirmingPlayer), confirmingPlayer, this, GetSerializedData(), true);
                            }
                            else
                            {
                                if (DeadPlayer != null && DeadPlayer.IsValid())
                                {
                                    DeadPlayer.IsConfirmed = true;
                                    DeadPlayer.CorpseConfirmer = confirmingPlayer;
                                }

                                RPCs.ClientConfirmPlayer(confirmingPlayer, this, GetSerializedData());
                            }

                            // TODO move this out of the Data.IsIdentified check, e.g. if the PlayerCorpse is getting confirmed otherwise, the credits would be lost
                            // TODO add custom networking as a credits found event
                            if (Data.Credits > 0)
                            {
                                confirmingPlayer.Credits += Data.Credits;
                                Data.Credits = 0;
                            }
                        }
                    }

                    if (covert)
                    {
                        CovertConfirmers.Add(confirmingPlayer);
                    }
                }

                if (Input.Down(InputButton.Use) && (Data.IsIdentified || CovertConfirmers.Contains(confirmingPlayer)))
                {
                    Player.ClientEnableInspectMenu(this);
                }
            }
        }

        public string GetSerializedData() => JsonSerializer.Serialize(Data, new JsonSerializerOptions()
        {
            WriteIndented = false
        });

        public static ConfirmationData GetDezerializedData(string json) => JsonSerializer.Deserialize<ConfirmationData>(json);
    }
}
