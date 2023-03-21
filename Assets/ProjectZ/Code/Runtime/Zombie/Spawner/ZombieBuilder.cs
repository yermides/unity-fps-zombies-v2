using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public class ZombieBuilder
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private ZombieBehaviour _zombieEnemyPrefab;
        private float _health;

        public ZombieBuilder()
        {
            _position = Vector3.zero;
            _rotation = Quaternion.identity;
        }

        public ZombieBuilder FromPrefab(ZombieBehaviour zombieEnemyPrefab)
        {
            _zombieEnemyPrefab = zombieEnemyPrefab;
            return this;
        }

        public ZombieBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public ZombieBuilder WithRotation(Quaternion rotation)
        {
            _rotation = rotation;
            return this;
        }

        public ZombieBuilder WithHealth(float health)
        {
            _health = health;
            return this;
        }

        public ZombieBehaviour Build()
        {
            var zombieEnemyInstance = Object.Instantiate(_zombieEnemyPrefab, _position, _rotation);
            zombieEnemyInstance.Configure(_health);
            return zombieEnemyInstance;
        }
    }
}