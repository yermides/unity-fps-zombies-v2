using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public abstract class ZombieSpawnerBehaviour : MonoBehaviour
    {
        #region UNITY

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }

        #endregion

        #region FUNCTIONS

        public abstract void Configure(ISpawnLocationDeciderStrategy spawnLocationDeciderStrategy);
        public abstract void StartSpawn();
        public abstract void StopSpawn();

        #endregion
    }
}