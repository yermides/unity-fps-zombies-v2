using UnityEngine;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public sealed class ZombieSpawner : ZombieSpawnerBehaviour
        , IEventListener<ZombieSpawnedEvent>
        , IEventListener<ZombieDiedEvent>
        , IEventListener<RoundStartedEvent>
    {
        #region FIELDS SERIALIZED
        
        [SerializeField] private ZombieBehaviour zombiePrefab;
        [SerializeField] private RoundDataFactoryBase roundDataFactory; // Should be the interface and be configured outside
        [SerializeField, Min(1)] private int roundToStartAt = 1;

        #endregion
        
        #region FIELDS

        private const int ZombiesAliveMax = 24;
        
        private RoundData _currentRoundData;
        private int _currentRoundNumber;
        private int _zombiesSpawnedCount;
        private int _zombiesAliveCount;
        private bool _isSpawning;
        private ZombieBuilder _zombieBuilder;
        private ISpawnLocationDeciderStrategy _spawnLocationDeciderStrategy;
        private bool _canSpawn;
        private float _lastSpawnTime;

        #endregion

        #region UNITY
        
        protected override void Awake()
        {
            _currentRoundNumber = roundToStartAt;
            _currentRoundData = roundDataFactory.CreateRoundData(_currentRoundNumber);
            _zombieBuilder = new ZombieBuilder().FromPrefab(zombiePrefab).WithHealth(_currentRoundData.ZombieHealth);
        }

        protected override void Start()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            eventQueue.Subscribe<ZombieSpawnedEvent>(OnEventRaised);
            eventQueue.Subscribe<ZombieDiedEvent>(OnEventRaised);
            eventQueue.Subscribe<RoundStartedEvent>(OnEventRaised);
        }

        protected override void Update()
        {
            if (!CanSpawnNextZombie()) return;
            
            SpawnZombie();
        }

        protected override void OnDestroy()
        {
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            eventQueue.Unsubscribe<ZombieSpawnedEvent>(OnEventRaised);
            eventQueue.Unsubscribe<ZombieDiedEvent>(OnEventRaised);
            eventQueue.Unsubscribe<RoundStartedEvent>(OnEventRaised);
        }

        #endregion
        
        public override void Configure(ISpawnLocationDeciderStrategy spawnLocationDeciderStrategy)
        {
            _spawnLocationDeciderStrategy = spawnLocationDeciderStrategy;
        }

        public override void StartSpawn()
        {
            _canSpawn = true;
            // if(_isSpawning) return;
            //
            // _isSpawning = true;
            // StartCoroutine(StartSpawnCoroutine());

            // areasTotal[0].GetSpawnZones()[0].GetSpawnPoints();
            // throw new System.NotImplementedException();
        }

        // private IEnumerator StartSpawnCoroutine()
        // {
        //     while (_isSpawning && _zombiesSpawnedCount <= _currentRoundData.ZombieCount)
        //     {
        //         if (_zombiesAliveCount >= ZombiesAliveMax)
        //         {
        //             yield return null; // Wait one frame
        //             // TODO: listen to ZombieDeathEvent so we can decrease the counter
        //         }
        //         else
        //         {
        //             yield return new WaitForSeconds(_currentRoundData.SecondsToSpawn);
        //             SpawnZombie();
        //         }
        //     }
        //
        //     _isSpawning = false;
        // }

        public override void StopSpawn()
        {
            _canSpawn = false;

            // _isSpawning = false;
        }

        private Transform GetNextSpawn()
        {
            return _spawnLocationDeciderStrategy.GetNextLocation();
        }

        private float GetSpawnInterval() => _currentRoundData.SecondsToSpawn;
        private int GetMaxSpawnCount() => _currentRoundData.ZombieCount;

        private bool CanSpawnNextZombie()
        {
            return (_canSpawn)
                   && (_zombiesSpawnedCount < GetMaxSpawnCount())
                   && (_zombiesAliveCount < ZombiesAliveMax)
                   && (Time.time - _lastSpawnTime > GetSpawnInterval());
        }

        private void SpawnZombie()
        {
            _lastSpawnTime = Time.time;
            
            // Spawn Zombie
            var spawn = GetNextSpawn();
            var position = spawn.position;
            var rotation = spawn.rotation;
                    
            _zombieBuilder
                .WithPosition(position)
                .WithRotation(rotation)
                .Build();
        }

        private void ResetRoundProgress()
        {
            _zombiesSpawnedCount = 0;
            _zombiesAliveCount = 0;
            _lastSpawnTime = 0.0f;
        }

        public void OnEventRaised(ZombieSpawnedEvent data)
        {
            _zombiesSpawnedCount++;
            _zombiesAliveCount++;

            // if (_zombiesSpawnedCount == _currentRoundData.ZombieCount)
            // {
            //     // TODO: Raise All zombies spawned Event
            // }
        }

        public void OnEventRaised(ZombieDiedEvent data)
        {
            _zombiesAliveCount--;
            
            // Checking end of round here, though it should be done in the game facade
            if (_zombiesAliveCount == 0 && _zombiesSpawnedCount == _currentRoundData.ZombieCount)
            {
                var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
                eventQueue.Enqueue(new RoundFinishedEvent{RoundNumber = _currentRoundNumber});
            }
        }

        public void OnEventRaised(RoundStartedEvent data)
        {
            ResetRoundProgress();
            
            // Create new round data
            _currentRoundData = roundDataFactory.CreateRoundData(++_currentRoundNumber);

            // Assign the builder with the new health for the round
            _zombieBuilder.WithHealth(_currentRoundData.ZombieHealth);
        }
    }
}