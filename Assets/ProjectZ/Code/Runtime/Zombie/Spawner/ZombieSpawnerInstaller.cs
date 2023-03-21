using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public class ZombieSpawnerInstaller : MonoBehaviour
    {
        [SerializeField] private ZombieSpawnerBehaviour zombieSpawner;
        [SerializeField] private AreaBasedSpawnLocationDeciderStrategy locationDeciderStrategy;

        private void Awake()
        {
            zombieSpawner.Configure(locationDeciderStrategy);
        }
    }
}