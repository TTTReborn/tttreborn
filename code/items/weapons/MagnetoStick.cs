using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

// Credits to the PhysGun: https://github.com/Facepunch/sandbox/blob/master/code/tools/PhysGun.cs

namespace TTTReborn.Items
{
    [Library("ttt_magnetostick")]
    partial class MagnetoStick : TTTWeapon // TODO Equipment
    {
        public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
        public override WeaponType WeaponType => WeaponType.Melee;
        public override float PrimaryRate => 0.1f;
        public override float SecondaryRate => 0.1f;
        public override float DeployTime => 0.2f;
        public override int Bucket => 1;
        public override int BaseDamage => 0;

        protected int grappingDistance => 80;

        protected PhysicsBody holdBody;
        protected WeldJoint holdJoint;

        protected PhysicsBody heldBody;
        protected Vector3 heldPos;
        protected Rotation heldRot;

        protected float holdDistance;
        protected bool grabbing;

        [Net]
        public bool BeamActive { get; set; }

        [Net]
        public Entity GrabbedEntity { get; set; }

        [Net]
        public int GrabbedBone { get; set; }

        [Net]
        public Vector3 GrabbedPos { get; set; }

        public PhysicsBody HeldBody => heldBody;

        public override void Spawn()
        {
            base.Spawn();

            // TODO: EnableDrawing = false does not work.
            RenderAlpha = 0f;

            SetModel("weapons/rust_boneknife/rust_boneknife.vmdl");
        }

        public override void Simulate(Client client)
        {
            TTTPlayer owner = Owner as TTTPlayer;

		    if (owner == null)
            {
                return;
            }

            Vector3 eyePos = owner.EyePos;
            Vector3 eyeDir = owner.EyeRot.Forward;
            Rotation eyeRot = Rotation.From(new Angles(0.0f, owner.EyeRot.Angles().yaw, 0.0f));

            BeamActive = Input.Down(InputButton.Attack1);

            if (IsServer)
            {
                using (Prediction.Off())
                {
                    if (!holdBody.IsValid())
                    {
                        return;
                    }

                    if (BeamActive)
                    {
                        if (heldBody.IsValid())
                        {
                            GrabMove(eyePos, eyeDir, eyeRot);
                        }
                        else
                        {
                            TryStartGrab(owner, eyePos, eyeRot, eyeDir);
                        }
                    }
                    else if (grabbing)
                    {
                        GrabEnd();
                    }
                }
            }
        }

        private void GrabInit(PhysicsBody body, Vector3 startPos, Vector3 grabPos, Rotation rot)
        {
            if (!body.IsValid())
            {
                return;
            }

            GrabEnd();

            grabbing = true;
            heldBody = body;
            holdDistance = Vector3.DistanceBetween(startPos, grabPos);
            holdDistance = holdDistance.Clamp(0, grappingDistance);
            heldPos = heldBody.Transform.PointToLocal(grabPos);
            heldRot = rot.Inverse * heldBody.Rotation;

            holdBody.Position = grabPos;
            holdBody.Rotation = heldBody.Rotation;

            heldBody.Wake();
            heldBody.EnableAutoSleeping = false;

            holdJoint = PhysicsJoint.Weld
                .From(holdBody)
                .To(heldBody, heldPos)
                .Create();
        }

        private void GrabEnd()
        {
            if (holdJoint.IsValid())
            {
                holdJoint.Remove();
            }

            if (heldBody.IsValid())
            {
                heldBody.EnableAutoSleeping = true;
            }

            Client client = GetClientOwner();

            if (client != null && GrabbedEntity.IsValid())
            {
                client.Pvs.Remove(GrabbedEntity);
            }

            heldBody = null;
            GrabbedEntity = null;
            grabbing = false;
        }

        private void GrabMove(Vector3 startPos, Vector3 dir, Rotation rot)
        {
            if (!heldBody.IsValid())
            {
                return;
            }

            holdBody.Position = startPos + dir * holdDistance;
            holdBody.Rotation = rot * heldRot;
        }

        private void TryStartGrab(TTTPlayer owner, Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir)
        {
            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * grappingDistance)
                .UseHitboxes()
                .Ignore(owner)
                .HitLayer(CollisionLayer.Debris)
                .EntitiesOnly()
                .Run();

            if (!tr.Hit || !tr.Body.IsValid() || (tr.Entity is TTTPlayer && tr.Entity.LifeState == LifeState.Alive))
            {
                return;
            }

            Entity rootEnt = tr.Entity.Root;
            PhysicsBody body = tr.Body;

            if (tr.Entity.Parent.IsValid())
            {
                if (rootEnt.IsValid() && rootEnt.PhysicsGroup != null)
                {
                    body = rootEnt.PhysicsGroup.GetBody(0);
                }
            }

            if (!body.IsValid())
            {
                return;
            }

            //
            // Don't move keyframed
            //
            if (body.BodyType == PhysicsBodyType.Keyframed)
            {
                return;
            }

            // Unfreeze
            if (body.BodyType == PhysicsBodyType.Static)
            {
                body.BodyType = PhysicsBodyType.Dynamic;
            }

            GrabInit(body, eyePos, tr.EndPos, eyeRot);

            GrabbedEntity = rootEnt;
            GrabbedPos = body.Transform.PointToLocal(tr.EndPos);
            GrabbedBone = tr.Entity.PhysicsGroup.GetBodyIndex(body);

            Client client = GetClientOwner();

            if (client != null)
            {
                client.Pvs.Add(GrabbedEntity);
            }
        }

        public override void AttackPrimary()
        {

        }

        private void Activate()
        {
            if (!IsServer)
            {
                return;
            }

            if (!holdBody.IsValid())
            {
                holdBody = PhysicsWorld.AddBody();
                holdBody.BodyType = PhysicsBodyType.Keyframed;
            }
        }

        private void Deactivate()
        {
            if (IsServer)
            {
                GrabEnd();

                holdBody?.Remove();
                holdBody = null;
            }
        }

        public override void ActiveStart(Entity ent)
        {
            base.ActiveStart(ent);

            Activate();
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            base.ActiveEnd(ent, dropped);

            Deactivate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Deactivate();
        }
    }
}
