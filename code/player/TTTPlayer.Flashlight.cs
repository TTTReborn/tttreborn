using Sandbox;

using TTTReborn.Items;

// credits to https://github.com/Facepunch/sbox-hidden/blob/main/code/player/Player.Flashlight.cs
// improved and bugfixed
namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        private Flashlight _worldFlashlight;
        private Flashlight _viewFlashlight;

        public bool HasFlashlightEntity
        {
            get
            {
                if (IsLocalPawn)
                {
                    return _viewFlashlight != null && _viewFlashlight.IsValid();
                }

                return _worldFlashlight != null && _worldFlashlight.IsValid();
            }
        }

        public bool IsFlashlightOn
        {
            get
            {
                if (IsLocalPawn)
                {
                    return HasFlashlightEntity && _viewFlashlight.Enabled;
                }

                return HasFlashlightEntity && _worldFlashlight.Enabled;
            }
        }

        public void ToggleFlashlight()
        {
            ShowFlashlight(!IsFlashlightOn);
        }

        public void ShowFlashlight(bool shouldShow, bool playSounds = true)
        {
            if (IsFlashlightOn == shouldShow)
            {
                return;
            }

            if (IsFlashlightOn)
            {
                if (IsServer)
                {
                    _worldFlashlight.TurnOff();
                }
                else
                {
                    _viewFlashlight.TurnOff();
                }
            }

            if (IsServer)
            {
                ClientShowFlashlightLocal(To.Single(this), shouldShow);
            }

            if (shouldShow)
            {
                if (!HasFlashlightEntity)
                {
                    if (IsServer)
                    {
                        _worldFlashlight = new Flashlight();
                        _worldFlashlight.EnableHideInFirstPerson = true;
                        _worldFlashlight.Rotation = EyeRot;
                        _worldFlashlight.SetParent(this);
                        _worldFlashlight.Position = EyePos + EyeRot.Forward * 10f;
                    }
                    else
                    {
                        _viewFlashlight = new Flashlight();
                        _viewFlashlight.EnableViewmodelRendering = false;
                        _viewFlashlight.Rotation = EyeRot;
                        _viewFlashlight.SetParent(this);
                        _viewFlashlight.Position = EyePos + EyeRot.Forward * 10f;
                    }
                }
                else
                {
                    if (IsServer)
                    {
                        // TODO: This is a weird hack to make sure the rotation is right.
                        _worldFlashlight.SetParent(null);
                        _worldFlashlight.Rotation = EyeRot;
                        _worldFlashlight.SetParent(this);
                        _worldFlashlight.Position = EyePos + EyeRot.Forward * 10f;
                        _worldFlashlight.TurnOn();
                    }
                    else
                    {
                        _viewFlashlight.TurnOn();
                    }
                }

                if (IsServer && playSounds)
                {
                    PlaySound("flashlight-on");
                }
            }
            else if (IsServer && playSounds)
            {
                PlaySound("flashlight-off");
            }
        }

        [ClientRpc]
        private void ClientShowFlashlightLocal(bool shouldShow)
        {
            ShowFlashlight(shouldShow);
        }

        private void TickPlayerFlashlight()
        {
            if (Input.Released(InputButton.Flashlight))
            {
                using (Prediction.Off())
                {
                    ToggleFlashlight();
                }
            }

            using (Prediction.Off())
            {
                if (IsFlashlightOn)
                {
                    if (IsServer)
                    {
                        _worldFlashlight.Rotation = EyeRot;
                        _worldFlashlight.Position = EyePos + EyeRot.Forward * 15f;
                    }
                    else
                    {
                        _viewFlashlight.Rotation = EyeRot;
                        _viewFlashlight.Position = EyePos + EyeRot.Forward * 15f;
                    }
                }
            }
        }
    }

    [Library("ttt_flashlight")]
    public partial class Flashlight : SpotLightEntity
    {
        public Flashlight() : base()
        {
            Transmit = TransmitType.Always;
            Enabled = true;
            DynamicShadows = true;
            Range = 1024;
            Falloff = 1.0f;
            LinearAttenuation = 0.0f;
            QuadraticAttenuation = 1.0f;
            Brightness = 1;
            Color = Color.White;
            InnerConeAngle = 10;
            OuterConeAngle = 30;
            FogStength = 1.0f;
        }
    }
}
