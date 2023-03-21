using System;
using System.Collections.Generic;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Interactables;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public sealed class Interactor : InteractorBehaviour, IEventListener<InteractableDestroyedEvent>
    {
        [SerializeField] private CharacterBehaviour character;
        private List<InteractableBehaviour> _interactives = new List<InteractableBehaviour>(4); // just to initialize capacity

        private void Reset()
        {
            character = GetComponent<CharacterBehaviour>();
        }

        private void Start()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<InteractableDestroyedEvent>(OnEventRaised);
        }

        private void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<InteractableDestroyedEvent>(OnEventRaised);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out InteractableBehaviour interactive)) return;

            _interactives.Add(interactive);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out InteractableBehaviour interactive)) return;

            _interactives.Remove(interactive);
        }

        public override void DoInteract()
        {
            if (_interactives.Count < 1) return;

            var interactive = _interactives[0];
            interactive.DoInteract(character);
        }

        public void OnEventRaised(InteractableDestroyedEvent data)
        {
            _interactives.RemoveAll(interactive => interactive.GetInstanceID() == data.InstanceID);
        }
    }
}