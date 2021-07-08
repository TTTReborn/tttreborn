using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_test2")]
    public class Test2 : TTTPerk, IBuyableItem
    {
        public Test2() : base()
        {

        }

        public override void OnEquip(TTTPlayer player)
        {
            VertexBuffer vb = new VertexBuffer(); // this should be called vertex builder or something
            vb.Init(true);
            vb.AddCube(Vector3.Zero, Vector3.One * 50.0f, Rotation.Identity);

            var mesh = new Mesh(Material.Load("materials/default/vertex_color.vmat"));
            mesh.CreateBuffers(vb);
            mesh.SetBounds(Vector3.One * -25, Vector3.One * 25);

            var model = new ModelBuilder()
            .AddMesh(mesh)
            .AddCollisionBox(Vector3.One * 25.0f)
            .Create();

            AnimEntity entity = new AnimEntity();
            entity.SetModel(model);
            entity.SetupPhysicsFromModel(PhysicsMotionType.Static);
            entity.GlowActive = true;
            entity.GlowColor = Color.Red;
            entity.GlowState = GlowStates.GlowStateOn;
            entity.GlowDistanceStart = 1;
            entity.GlowDistanceEnd = 10000;
            entity.Position = player.Position + player.EyeRot.Forward * 100f;
        }

        public int Price => 0;
    }
}
