using System;
using System.Threading.Tasks;
using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Patterns.States;
using ProjectZ.Code.Runtime.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectZ.Code.Runtime.Zombie.States
{
    public class ChasingState : IState
    {
        private readonly Zombie _zombie;
        private readonly Transform _zombieTransform;
        private Transform _target;
        private readonly NavMeshAgent _agent;
        private readonly Action _onCompleted;
        private bool _isActive;

        public ChasingState(Zombie zombie, Action onCompleted)
        {
            _zombie = zombie;
            _zombieTransform = _zombie.transform;
            _agent = _zombie.GetNavMeshAgent();
            _onCompleted = onCompleted;
        }

        public void OnEnter()
        {
            _isActive = true;
            _agent.isStopped = false;
            
            // Find Target
            _target = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        }

        public void OnUpdate()
        {
            _agent.destination = _target.position;

            if (Vector3.Distance(_target.position, _zombieTransform.position) < 1.5f)
            {
                _onCompleted?.Invoke();
            }
        }

        public void OnExit()
        {
            _isActive = false;
        }

        // TODO: Search for target periodically
        private async void SearchForTarget()
        {
            while (_isActive)
            {
                await Task.Yield();
            }
        }
    }
}