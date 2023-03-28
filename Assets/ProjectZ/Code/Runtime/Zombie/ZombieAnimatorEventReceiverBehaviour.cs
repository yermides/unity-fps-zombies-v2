using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie
{
    public abstract class ZombieAnimatorEventReceiverBehaviour : MonoBehaviour
    {
        public abstract void OnAnimationEndedSpawning();
        public abstract void OnMeleeAttackPerformed();
        public abstract void OnAnimationEndedMelee();
    }
}