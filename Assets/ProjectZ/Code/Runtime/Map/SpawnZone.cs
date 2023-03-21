using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Map
{
    public class SpawnZone : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPoints;
        private int _spawnPointsCount;
        private int _spawnIndex;

        private void Awake()
        {
            _spawnPointsCount = spawnPoints.Count;
        }

        // Deprecated, use GetNextSpawn to also get the orientation
        public Vector3 GetNextSpawnPosition() => GetNextSpawn().position;

        public Transform GetNextSpawn()
        {
            if (_spawnPointsCount == 0)
            {
                throw new IndexOutOfRangeException("Spawn Zone has no valid locations!");
            }

            var nextPosition = spawnPoints[_spawnIndex % _spawnPointsCount];
            _spawnIndex++;
            
            return nextPosition;
        }

        public List<Transform> GetSpawnPoints() => spawnPoints;
    }
}