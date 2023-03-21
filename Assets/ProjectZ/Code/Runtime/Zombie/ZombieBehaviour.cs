using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie
{
    public abstract class ZombieBehaviour : MonoBehaviour
    {
        #region UNITY

        protected virtual void Awake()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new ZombieSpawnedEvent { InstanceID = GetInstanceID() });
        }
        
        protected virtual void Start() { }
        protected virtual void Update() { }

        protected virtual void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new ZombieDiedEvent() { InstanceID = GetInstanceID() });
        }

        #endregion

        #region FUNCTIONS
        public abstract void Configure(float health);
        public abstract void Attack();

        #endregion

    }
}