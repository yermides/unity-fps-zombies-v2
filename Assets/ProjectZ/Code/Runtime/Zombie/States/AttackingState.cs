using System;
using System.Threading.Tasks;
using ProjectZ.Code.Runtime.Patterns.States;
using ProjectZ.Code.Runtime.Utils.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectZ.Code.Runtime.Zombie.States
{
    public class AttackingState : IState
    {
        private Zombie _zombie;
        private readonly Action _onCompleted;
        private readonly NavMeshAgent _agent;
        
        public AttackingState(Zombie zombie, Action onCompleted)
        {
            _zombie = zombie;
            _onCompleted = onCompleted;
            _agent = _zombie.GetNavMeshAgent();
        }
        
        public void OnEnter()
        {
            _agent.isStopped = true;
            AttackInDelay().WrapErrors();
        }

        public void OnUpdate()
        {
        }

        public void OnExit()
        {
            Debug.Log("Exited attack");
        }

        private async Task AttackInDelay()
        {
            await Task.Delay(1000);
            _onCompleted?.Invoke();
        }
    }
}