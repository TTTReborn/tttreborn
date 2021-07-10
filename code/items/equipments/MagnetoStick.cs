using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

// Credits to the PhysGun: https://github.com/Facepunch/sandbox/blob/master/code/tools/PhysGun.cs

namespace TTTReborn.Items
{
    [Library("ttt_magnetostick")]
    partial class MagnetoStick : TTTEquipment
    {
        public override string ViewModelPath => "";

        private static int _grabbingDistance => 80;
        private static float _maxPropSpeed => 10f;

        private PhysicsBody _holdBody;
        private WeldJoint _holdJoint;

        private PhysicsBody _heldBody;
        private Rotation _heldRot;

        private float _holdDistance = 0f;
        private bool _isAttaching = false;

        private Vector3 _previousPosition;

        public Entity GrabbedEntity { get; set; }

        public override void Spawn()
        {
            base.Spawn();

            RenderAlpha = 0f;
        }

        public override void Simulate(Client client)
        {
            if (!IsServer)
            {
                return;
            }

            TTTPlayer owner = Owner as TTTPlayer;

            if (owner == null)
            {
                return;
            }

            Vector3 eyePos = owner.EyePos;
            Vector3 eyeDir = owner.EyeRot.Forward;
            Rotation eyeRot = Rotation.From(new Angles(0.0f, owner.EyeRot.Angles().yaw, 0.0f));

            using (Prediction.Off())
            {
                if (!_holdBody.IsValid())
                {
                    return;
                }

                if (Input.Down(InputButton.Attack1))
                {
                    if (_heldBody.IsValid())
                    {
                        if (_heldBody.Entity is PlayerCorpse playerCorpse && playerCorpse.RopeSprings.Count > 0)
                        {
                            foreach (PhysicsJoint spring in playerCorpse.RopeSprings)
                            {
                                if (Vector3.DistanceBetween(spring.Body1.Position, spring.Anchor2) > _grabbingDistance)
                                {
                                    GrabEnd();

                                    return;
                                }
                            }
                        }

                        GrabMove(eyePos, eyeDir, eyeRot);
                    }
                    else
                    {
                        TryStartGrab(owner, eyePos, eyeRot, eyeDir);
                    }

                    if (Input.Down(InputButton.Attack2))
                    {
                        if (!_isAttaching)
                        {
                            _isAttaching = true;

                            BindEntity();
                        }
                    }
                    else
                    {
                        _isAttaching = false;
                    }
                }
                else if (GrabbedEntity.IsValid())
                {
                    GrabEnd();
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

            _heldBody = body;
            _holdDistance = Vector3.DistanceBetween(startPos, grabPos);
            _holdDistance = _holdDistance.Clamp(0, _grabbingDistance);

            Vector3 heldPos = _heldBody.Transform.PointToLocal(grabPos);

            _heldRot = rot.Inverse * _heldBody.Rotation;

            _holdBody.Position = grabPos;
            _holdBody.Rotation = _heldBody.Rotation;
            _previousPosition = _heldBody.Position;

            _heldBody.Wake();
            _heldBody.EnableAutoSleeping = false;

            _holdJoint = PhysicsJoint.Weld
                .From(_holdBody)
                .To(_heldBody, heldPos)
                .WithLinearSpring(20f, 1f, 0.0f)
                .WithAngularSpring(0.0f, 0.0f, 0.0f)
                .Create();
        }

        private void GrabEnd()
        {
            if (_holdJoint.IsValid)
            {
                _holdJoint.Remove();
            }

            if (_heldBody.IsValid())
            {
                _heldBody.EnableAutoSleeping = true;
            }

            Client client = GetClientOwner();

            if (client != null && GrabbedEntity.IsValid())
            {
                client.Pvs.Remove(GrabbedEntity);
            }

            _heldBody = null;
            GrabbedEntity = null;
        }

        private void GrabMove(Vector3 startPos, Vector3 dir, Rotation rot)
        {
            if (!_heldBody.IsValid())
            {
                return;
            }

            _holdBody.Position = startPos + dir * _holdDistance;
            _holdBody.Rotation = rot * _heldRot;
            
            if (Vector3.DistanceBetween(_previousPosition, _heldBody.Position) > _maxPropSpeed)
            {
                GrabEnd();
                return;
            }

            _previousPosition = _heldBody.Position;
        }

        private void TryStartGrab(TTTPlayer owner, Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir)
        {
            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * _grabbingDistance)
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

            if (tr.Entity.Parent.IsValid() && rootEnt.IsValid() && rootEnt.PhysicsGroup != null)
            {
                body = rootEnt.PhysicsGroup.GetBody(0);
            }

            // Don't move keyframed
            if (!body.IsValid() || body.BodyType == PhysicsBodyType.Keyframed)
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

            GetClientOwner()?.Pvs.Add(GrabbedEntity);
        }

        private void BindEntity()
        {
            if (!IsServer || !_heldBody.IsValid() || _heldBody.Entity is not PlayerCorpse playerCorpse)
            {
                return;
            }

            using (Prediction.Off())
            {
                TraceResult tr = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * _grabbingDistance)
                    .Ignore(Owner)
                    .Run();

                if (!tr.Hit || !tr.Entity.IsValid())
                {
                    playerCorpse.ClearAttachments();

                    return;
                }

                Entity attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

                if (!attachEnt.IsWorld)
                {
                    playerCorpse.ClearAttachments();

                    return;
                }

                Particles rope = Particles.Create("particles/rope.vpcf");
                rope.SetEntity(0, _heldBody.Entity);
                rope.SetPosition(1, tr.Body.Transform.PointToLocal(tr.EndPos));

                playerCorpse.Ropes.Add(rope);
                playerCorpse.RopeSprings.Add(
                    PhysicsJoint.Spring
                        .From(_heldBody)
                        .To(tr.Body)
                        .WithPivot(tr.EndPos)
                        .WithDampingRatio(5f)
                        .WithFrequency(8f)
                        .Create()
                );
            }
        }

        private void Activate()
        {
            if (IsServer && !_holdBody.IsValid())
            {
                _holdBody = PhysicsWorld.AddBody();
                _holdBody.BodyType = PhysicsBodyType.Keyframed;
            }
        }

        private void Deactivate()
        {
            if (IsServer)
            {
                GrabEnd();

                _holdBody?.Remove();
                _holdBody = null;
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

        public override bool CanDrop() => false;
    }
}
