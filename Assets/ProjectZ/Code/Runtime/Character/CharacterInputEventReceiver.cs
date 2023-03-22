using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectZ.Code.Runtime.Character
{
    public class CharacterInputEventReceiver : MonoBehaviour
        , ICharacterInputEvents
        , CharacterInput.IPlayerActions
        , CharacterInput.IUIActions
    {
        private CharacterInput _characterInput;

        #region EVENTS
        
        public event Action<Vector2> MoveEvent;
        public event Action<Vector2> LookEvent;
        public event Action FireStartedEvent;
        public event Action FirePerformedEvent;
        public event Action FireCanceledEvent;
        public event Action RunStartedEvent;
        public event Action RunCanceledEvent;
        public event Action JumpPerformedEvent;
        public event Action MeleePerformedEvent;
        public event Action InteractPerformedEvent;
        public event Action<float> CyclePerformedEvent;
        public event Action ReloadPerformedEvent;
        public event Action AimStartedEvent;
        public event Action AimCanceledEvent;

        #endregion

        #region UNITY

        private void Awake()
        {
            // _characterInput?.Dispose();
            Configure();
        }

        // private void OnEnable()
        // {
        //     if (_characterInput == null)
        //     {
        //         Configure();
        //     }
        // }

        private void Configure()
        {
            // It auto-configures itself to listen to the callbacks, no need to drag to editor events in PlayerInput
            // I honestly could do this inside Awake and let the class user (or a CharacterInstaller)
            // instantiate a CharacterInputReader but it works this way as well
            // although it will always listen to the events just by existing
            _characterInput = new CharacterInput();
            _characterInput.Player.SetCallbacks(this);
            _characterInput.UI.SetCallbacks(this);
                
            // Enable player actions
            _characterInput.Player.Enable();
            _characterInput.UI.Disable();
        }

        // private void OnDisable()
        // {
        //     Debug.LogWarning("OnDisable");
        //
        //     if (_characterInput != null)
        //     {
        //         _characterInput.Player.SetCallbacks(null);
        //         _characterInput.UI.SetCallbacks(null);
        //         _characterInput.Player.Disable();
        //         _characterInput.UI.Disable();
        //         _characterInput.Dispose();
        //         _characterInput = null;
        //     }
        // }

        private void OnDestroy()
        {
            _characterInput?.Dispose();
        }

        #endregion

        #region PLAYER

        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            MoveEvent?.Invoke(value);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            LookEvent?.Invoke(value);
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            var phase = context.phase;

            if (phase == InputActionPhase.Started)
            {
                FireStartedEvent?.Invoke();
            } 
            else if (phase == InputActionPhase.Performed)
            {
                FirePerformedEvent?.Invoke();
            } 
            else if (phase == InputActionPhase.Canceled)
            {
                FireCanceledEvent?.Invoke();
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            var phase = context.phase;

            if (phase == InputActionPhase.Started)
            {
                RunStartedEvent?.Invoke();
            }
            else if (phase == InputActionPhase.Canceled)
            {
                RunCanceledEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                JumpPerformedEvent?.Invoke();
            }
        }

        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                MeleePerformedEvent?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                InteractPerformedEvent?.Invoke();
            }
        }

        public void OnCycle(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) 
                    ? Mathf.Sign(context.ReadValue<Vector2>().y) 
                    : 1.0f;
                
                CyclePerformedEvent?.Invoke(scrollValue);
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ReloadPerformedEvent?.Invoke();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            var phase = context.phase;
            
            if (phase == InputActionPhase.Started)
            {
                AimStartedEvent?.Invoke();
            }
            else if (phase == InputActionPhase.Canceled)
            {
                AimCanceledEvent?.Invoke();
            }
        }

        #endregion

        #region UI

        public void OnNavigate(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}