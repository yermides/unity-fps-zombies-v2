using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie.Spawner
{
    public interface IRoundDataFactory
    {
        public RoundData CreateRoundData(int round, int players);
    }

    public abstract class RoundDataFactoryBase : ScriptableObject, IRoundDataFactory
    {
        public abstract RoundData CreateRoundData(int round, int players = 1);
    }
}