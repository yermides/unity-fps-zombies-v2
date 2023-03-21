using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Interactables
{
    public abstract class InteractableBehaviour : MonoBehaviour
    {
        public abstract void DoInteract(CharacterBehaviour character);

        private void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new InteractableDestroyedEvent { InstanceID = GetInstanceID() });
        }
    }
}