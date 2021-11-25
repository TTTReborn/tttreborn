// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox;

// TODO Fix flickering on close-range walls (with fast rotation)
// TODO Add physics (avoid collision with walls or the playermodel)
namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private Flashlight _worldFlashlight;
        private Flashlight _viewFlashlight;

        private const float FLASHLIGHT_DISTANCE = 15f;
        private const float SMOOTH_SPEED = 25f;

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
                using (Prediction.Off())
                {
                    ClientShowFlashlightLocal(To.Single(this), shouldShow);
                }
            }

            if (shouldShow)
            {
                if (!HasFlashlightEntity)
                {
                    if (IsServer)
                    {
                        _worldFlashlight = new();
                        _worldFlashlight.EnableHideInFirstPerson = true;
                        _worldFlashlight.Rotation = EyeRot;
                        _worldFlashlight.Position = EyePos + EyeRot.Forward * FLASHLIGHT_DISTANCE;
                        _worldFlashlight.SetParent(this);
                    }
                    else
                    {
                        _viewFlashlight = new();
                        _viewFlashlight.EnableViewmodelRendering = false;
                        _viewFlashlight.Position = EyePos + EyeRot.Forward * FLASHLIGHT_DISTANCE;
                        _viewFlashlight.Rotation = EyeRot;
                        _viewFlashlight.SetParent(this);
                    }
                }
                else
                {
                    if (IsServer)
                    {
                        _worldFlashlight.SetParent(null);
                        _worldFlashlight.Rotation = EyeRot;
                        _worldFlashlight.Position = EyePos + EyeRot.Forward * FLASHLIGHT_DISTANCE;
                        _worldFlashlight.SetParent(this);
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

        private void TickPlayerFlashlight()
        {
            if (IsServer)
            {
                using (Prediction.Off())
                {
                    if (Input.Released(InputButton.Flashlight))
                    {
                        ToggleFlashlight();
                    }
                }

                if (IsFlashlightOn)
                {
                    _worldFlashlight.Rotation = Rotation.Slerp(_worldFlashlight.Rotation, Input.Rotation, SMOOTH_SPEED);
                    _worldFlashlight.Position = Vector3.Lerp(_worldFlashlight.Position, EyePos + Input.Rotation.Forward * FLASHLIGHT_DISTANCE, SMOOTH_SPEED);
                }
            }
        }

        public override void PostCameraSetup(ref CameraSetup camSetup)
        {
            base.PostCameraSetup(ref camSetup);

            if (IsFlashlightOn)
            {
                _viewFlashlight.Rotation = Input.Rotation;
                _viewFlashlight.Position = EyePos + Input.Rotation.Forward * FLASHLIGHT_DISTANCE;
            }
        }
    }

    [Hammer.Skip]
    [Library("ttt_flashlight")]
    public partial class Flashlight : SpotLightEntity
    {
        public Flashlight() : base()
        {
            Transmit = TransmitType.Always;
            Enabled = true;
            DynamicShadows = true;
            Range = 512f;
            Falloff = 4f;
            LinearAttenuation = 0f;
            QuadraticAttenuation = 1f;
            Brightness = 1f;
            Color = Color.White;
            InnerConeAngle = 10f;
            OuterConeAngle = 30f;
            FogStength = 1f;
        }
    }
}
