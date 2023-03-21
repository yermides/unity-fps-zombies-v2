using UnityEngine;

namespace ProjectZ.Code.Runtime.Common
{
    public struct CombatArgs
    {
        public Team Team;
    }

    public abstract class CombatBehaviour : MonoBehaviour
    {
        public abstract void Configure(CombatArgs args);
        public abstract void Attack();
    }
}