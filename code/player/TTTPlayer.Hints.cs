using System.Collections.Generic;

using Sandbox;
using Sandbox.Component;

using TTTReborn.Globalization;
using TTTReborn.Camera;
using TTTReborn.UI;

namespace TTTReborn
{
    public partial class Player
    {
        private const float MAX_HINT_DISTANCE = 2048f;

        private EntityHintPanel _currentHintPanel;
        private Entity _currentTarget;

        private void TickEntityHints()
        {
            if (CameraMode is ThirdPersonSpectateCamera)
            {
                DeleteHint();

                return;
            }

            IEntityHint hint = IsLookingAtHintableEntity(MAX_HINT_DISTANCE);
            Entity target = hint as Entity;

            if (hint == null)
            {
                IUse use = IsLookingAtUsableEntity(87.5f);

                if (use == null || !use.IsUsable(this))
                {
                    DeleteHint();

                    return;
                }

                target = use as Entity;
            }
            else if (!hint.CanHint(this))
            {
                DeleteHint();

                return;
            }

            if (target != null && target == _currentTarget)
            {
                if (hint != null)
                {
                    hint.TextTick(this);

                    if (IsClient)
                    {
                        _currentHintPanel.UpdateHintPanel(hint.TextOnTick);
                    }
                }
                else if (target is DoorEntity doorEntity)
                {
                    if (IsClient)
                    {
                        TranslationData translationData;

                        if (doorEntity.State == DoorEntity.DoorState.Closed)
                        {
                            translationData = new("DOOR.OPEN");
                        }
                        else
                        {
                            translationData = new("DOOR.CLOSE");
                        }

                        if (doorEntity.Locked)
                        {
                            translationData = new("DOOR.LOCKED");

                            if (_currentHintPanel is GlyphHint glyphHint && glyphHint.InputButtons.Contains(InputButton.Use))
                            {
                                glyphHint.InputButtons.Remove(InputButton.Use);
                            }
                        }
                        else if (_currentHintPanel is GlyphHint glyphHint && !glyphHint.InputButtons.Contains(InputButton.Use))
                        {
                            glyphHint.InputButtons.Add(InputButton.Use);
                        }

                        _currentHintPanel.UpdateHintPanel(translationData);
                    }
                }

                return;
            }

            DeleteHint();

            if (IsClient)
            {
                if ((hint == null || hint.ShowGlow) && target is ModelEntity model && model.IsValid())
                {
                    Glow glow = model.Components.GetOrCreate<Glow>();
                    glow.Color = Color.White; // TODO: Let's let people change this in their settings.
                    glow.Active = true;
                }

                if (hint != null)
                {
                    _currentHintPanel = hint.DisplayHint(this);
                    _currentHintPanel.Parent = HintDisplay.Instance;
                    _currentHintPanel.Enabled(true);
                }
                else if (target != null)
                {
                    TranslationData translationData = new("ENTITY.USE", Utils.GetLibraryName(target.GetType()));
                    List<InputButton> inputButtons = new()
                    {
                        InputButton.Use
                    };

                    if (target is DoorEntity doorEntity)
                    {
                        translationData = doorEntity.State == DoorEntity.DoorState.Open ? new("DOOR.CLOSE") : new("DOOR.OPEN");

                        if (doorEntity.Locked)
                        {
                            translationData = new("DOOR.LOCKED");
                            inputButtons.Clear();
                        }
                    }

                    _currentHintPanel = new GlyphHint(translationData, inputButtons.ToArray())
                    {
                        Parent = HintDisplay.Instance
                    };
                    _currentHintPanel.Enabled(true);
                }
            }

            _currentTarget = target;
        }

        private void DeleteHint()
        {
            if (IsClient)
            {
                if (_currentTarget != null && _currentTarget is ModelEntity model && model.IsValid())
                {
                    Glow glow = model.Components.Get<Glow>();

                    if (glow != null)
                    {
                        glow.Active = false;
                    }
                }

                _currentHintPanel?.Delete(true);
                _currentHintPanel = null;
            }

            _currentTarget = null;
        }
    }
}
