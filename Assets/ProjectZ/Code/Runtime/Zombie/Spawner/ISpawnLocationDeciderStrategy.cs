using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public interface ISpawnLocationDeciderStrategy
    {
        public Transform GetNextLocation();
    }
}