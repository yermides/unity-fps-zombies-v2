using System;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    /// <summary>
    /// This class receives the callbacks from the unity new input system and propagates those events
    /// therefore hiding the InputAction.CallbackContext and the logic to control when events are emitter
    /// for the end user.
    /// <br/>
    /// It is expected the player configures itself to the public events on it's awake/start/onenable.
    /// </summary>
    public interface ICharacterInputEvents
    {
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
    }
}