using System;
using ProjectZ.Code.Runtime.Patterns.States;

namespace ProjectZ.Code.Runtime.Zombie.States
{
    public class SpawningState : IState
    {
        private readonly Zombie _zombie;
        private readonly Action _onCompleted;
        private readonly IZombieAnimatorEvents _animatorEvents;

        public SpawningState(Zombie zombie, Action onCompleted)
        {
            _zombie = zombie;
            _animatorEvents = _zombie.GetAnimatorEvents();
            _onCompleted = onCompleted;
        }

        public void OnEnter() => _animatorEvents.AnimationEndedSpawningEvent += TriggerCompletedCallback;
        public void OnUpdate() { }
        public void OnExit() => _animatorEvents.AnimationEndedSpawningEvent -= TriggerCompletedCallback;
        private void TriggerCompletedCallback() => _onCompleted();
    }
}