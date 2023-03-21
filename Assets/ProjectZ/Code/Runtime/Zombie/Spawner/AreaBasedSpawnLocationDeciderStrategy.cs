using System.Collections.Generic;
using System.Linq;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Map;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    // TODO: copy logic from ZombieSpawner
    public class AreaBasedSpawnLocationDeciderStrategy : MonoBehaviour
        , ISpawnLocationDeciderStrategy
        , IEventListener<PlayerEnteredAreaEvent>
        , IEventListener<PlayerLeftAreaEvent>
    {
        [SerializeField] private Area defaultArea;
        [SerializeField] private Area[] areas;
        private List<Area> _areasWithPlayers;
        private List<Area> _areasWithoutPlayers;

        private void Awake()
        {
            _areasWithoutPlayers = new List<Area>(areas);
            _areasWithPlayers = new List<Area>(areas.Length);
        }

        private void Start()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<PlayerEnteredAreaEvent>(OnEventRaised);
            eventQueue.Subscribe<PlayerLeftAreaEvent>(OnEventRaised);
        }

        private void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<PlayerLeftAreaEvent>(OnEventRaised);
            eventQueue.Unsubscribe<PlayerEnteredAreaEvent>(OnEventRaised);
        }

        public Transform GetNextLocation()
        {
            Transform location = null;
            
            if (_areasWithPlayers.Count > 0)
            {
                // TODO: consider more areas where players can be
                var spawnZones = _areasWithPlayers[0].GetSpawnZones();
                var spawnZoneIndex = Random.Range(0, spawnZones.Count);
                location = spawnZones[spawnZoneIndex].GetNextSpawn();
            }

            // _areasWithoutPlayers
            return location;
        }

        public void OnEventRaised(PlayerEnteredAreaEvent data)
        {
            // Find area that was entered
            var area = areas.First((elem) => elem.GetID() == data.AreaID);

            // We now know it's an area that holds a player
            _areasWithoutPlayers.Remove(area);
            _areasWithPlayers.Add(area);
        }

        public void OnEventRaised(PlayerLeftAreaEvent data)
        {
            // Find area that was entered
            var area = areas.First((elem) => elem.GetID() == data.AreaId);

            if (data.CharacterCount == 0)
            {
                _areasWithPlayers.Remove(area);
                _areasWithoutPlayers.Add(area);
            }
        }
    }
}