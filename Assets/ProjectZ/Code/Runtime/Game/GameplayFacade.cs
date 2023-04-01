using System;
using System.Collections;
using System.Threading.Tasks;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using ProjectZ.Code.Runtime.Zombie;
using ProjectZ.Code.Runtime.Zombie.Spawner;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Game
{
    // TODO: Listen to the events 
    public class GameplayFacade : MonoBehaviour, IEventListener<RoundFinishedEvent>
    {
        [SerializeField] private ZombieSpawnerBehaviour zombieSpawner;
        [SerializeField] private bool autoStartSpawn = true;
        private int _roundCurrent;

        private void Start()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Subscribe<RoundFinishedEvent>(OnEventRaised);
            
            if (autoStartSpawn)
            {
                StartCoroutine(StartSpawnCoroutine());
            }
        }

        private void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Unsubscribe<RoundFinishedEvent>(OnEventRaised);
        }

        public async void OnEventRaised(RoundFinishedEvent data)
        {
            _roundCurrent = data.RoundNumber;
            
            Debug.Log("Stopped spawning");
            // Await 5 seconds and enqueue next round event
            zombieSpawner.StopSpawn();

            await Task.Delay(3000);

            Debug.Log($"Finished Round {data.RoundNumber}");

            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new RoundStartedEvent {RoundNumber = ++_roundCurrent });
            
            zombieSpawner.StartSpawn();
        }

        // Starts spawning after 3 seconds
        private IEnumerator StartSpawnCoroutine()
        {
            yield return new WaitForSeconds(3);
            zombieSpawner.StartSpawn();
        }
    }
}