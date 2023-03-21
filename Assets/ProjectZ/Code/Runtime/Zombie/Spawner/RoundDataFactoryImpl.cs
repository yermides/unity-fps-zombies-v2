using NaughtyAttributes;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    [CreateAssetMenu(menuName = "Create RoundDataFactoryImpl", fileName = "RoundDataFactoryImpl", order = 0)]
    public sealed class RoundDataFactoryImpl : RoundDataFactoryBase
    {
        [CurveRange(0,0,1,10)]
        [SerializeField] private AnimationCurve zombieSpawnDelaySeconds;
        
        // [SerializeField] private ParticleSystem.MinMaxCurve intervalCurve;
        private int GetZombieCount(int round, int players)
        {
            int zombiesToSpawn = 24;
            
            if (round is >= 1 and <= 4)
            {
                return Mathf.CeilToInt(zombiesToSpawn * (round * 0.2f));
            }

            if (round is >= 5 and <= 9)
            {
                return zombiesToSpawn;
            }

            const float multiplyFactor = 0.15f;
            float multiplier = round * multiplyFactor;

            return Mathf.CeilToInt(zombiesToSpawn * multiplier);
        }

        private float GetZombieHealth(int round, int players)
        {
            float healthToSpawnWith = (round * 100.0f) + 50.0f;
            
            if (round is >= 1 and <= 9)
            {
                return (int)healthToSpawnWith;
            }

            for (int i = 10; i <= round; i++)
            {
                healthToSpawnWith *= 1.1f;
            }

            return healthToSpawnWith;
        }

        private float GetSecondsToSpawn(int round)
        {
            var curveValueNormalized = ((float)round) / 100.0f;
            return zombieSpawnDelaySeconds.Evaluate(curveValueNormalized);
        }

        private Vector2 GetSecondsToSpawnRange(int round, int players)
        {
            return Vector2.zero;
            
            // var curveValueNormalized = ((float)round) / 100.0f;
            // var evaluatedValue = intervalCurve.Evaluate(curveValueNormalized);
            // return new Vector2(0.8f, 1.2f) * evaluatedValue;
        }

        public override RoundData CreateRoundData(int round, int players = 1)
        {
            return new RoundData 
            { 
                ZombieCount = GetZombieCount(round, players), 
                ZombieHealth = GetZombieHealth(round, players),
                SecondsToSpawn = GetSecondsToSpawn(round),
                // SecondsToSpawnRange = GetSecondsToSpawnRange(round, players),
            };
        }
    }
}