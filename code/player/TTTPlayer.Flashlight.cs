using Sandbox;
using TTTReborn.Roles;
using TTTReborn.Teams;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        private Flashlight Flashlight { get; set; }

        private void UpdateFlashlight()
        {
            if (Flashlight != null)
            {
                Flashlight.SetPosition(EyePos + EyeRot.Forward * 35);
                Flashlight.SetRotation(EyeRot);
            }
        }
    }

    partial class Flashlight : Entity
    {
        private SpotLightEntity _localLight;
        private SpotLightEntity _worldLight;

        public Flashlight()
        {
            _localLight = CreateLight();
            _localLight.EnableViewmodelRendering = false;

            _worldLight = CreateLight();
            _worldLight.EnableHideInFirstPerson = true;
        }

        public void TurnOn()
        {
            _localLight.Enabled = true;
            _worldLight.Enabled = true;
        }

        public void TurnOff()
        {
            _localLight.Enabled = false;
            _worldLight.Enabled = false;
        }

        public void SetPosition(Vector3 pos)
        {
            _localLight.Position = pos;
            _worldLight.Position = pos;
        }

        public void SetRotation(Rotation rot)
        {
            _localLight.Rotation = rot;
            _worldLight.Rotation = rot;
        }

        private SpotLightEntity CreateLight()
        {
            return new SpotLightEntity
            {
                Enabled = true,
                DynamicShadows = true,
                Range = 1024,
                Falloff = 1.0f,
                LinearAttenuation = 0.0f,
                QuadraticAttenuation = 1.0f,
                Brightness = 1,
                Color = Color.White,
                InnerConeAngle = 10,
                OuterConeAngle = 30,
                FogStength = 1.0f,
            };
        }
    }
}
