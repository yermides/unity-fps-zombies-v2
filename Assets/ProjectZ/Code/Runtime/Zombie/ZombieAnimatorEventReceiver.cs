using System;

namespace ProjectZ.Code.Runtime.Zombie
{
    public interface IZombieAnimatorEvents
    {
        public event Action AnimationEndedSpawningEvent; // End Animation
        public event Action AnimationEndedMeleeEvent; // End Animation
        public event Action MeleeAttackPerformedEvent; // Mid animation
    }

    public sealed class ZombieAnimatorEventReceiver : ZombieAnimatorEventReceiverBehaviour, IZombieAnimatorEvents
    {
        #region ANIMATION
        
        public override void OnAnimationEndedSpawning() => AnimationEndedSpawningEvent?.Invoke();
        public override void OnMeleeAttackPerformed() => MeleeAttackPerformedEvent?.Invoke();
        public override void OnAnimationEndedMelee() => AnimationEndedMeleeEvent?.Invoke();
        
        #endregion

        #region EVENTS

        public event Action AnimationEndedSpawningEvent;
        public event Action AnimationEndedMeleeEvent;
        public event Action MeleeAttackPerformedEvent;

        #endregion
    }
}