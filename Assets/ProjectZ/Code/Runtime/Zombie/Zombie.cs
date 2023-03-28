using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Common;
using ProjectZ.Code.Runtime.Patterns.States;
using ProjectZ.Code.Runtime.Utils.Extensions;
using ProjectZ.Code.Runtime.Zombie.States;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectZ.Code.Runtime.Zombie
{
    public struct ZombieArgs
    {
        public IZombieAnimatorEvents zombieAnimatorEvents;
    }

    [SelectionBase]
    public sealed class Zombie : ZombieBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Foldout("References")]
        [SerializeField] private NavMeshAgent agent;
        [Foldout("References")]
        [SerializeField] private Health health;
        [Foldout("References")] 
        [SerializeField] private CombatBehaviour combat;
        [Foldout("References")] 
        [SerializeField] private Animator animator;

        [Foldout("Values")] 
        [SerializeField] private float speed = 1.5f;

        #endregion

        #region FIELDS

        private Transform _target;
        private bool _isAttacking;
        private bool _canAttack;
        private IState _stateCurrent;
        private ZombieStateID _stateIDCurrent;
        private Dictionary<ZombieStateID, IState> _zombieStatesToIDs;
        private IZombieAnimatorEvents _zombieAnimatorEvents;

        #endregion

        #region UNITY
        
        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            combat = GetComponent<CombatBehaviour>();
        }

        protected override void Awake()
        {
            base.Awake();
            
            ConfigureStateMachine();
            
            // TODO: find target more efficiently or through interface
            // target = GameObject.FindGameObjectsWithTag(Tags.Player)[0].GetComponent<CharacterBehaviour>();

            // agent.isStopped = false;
            agent.speed = speed;
        }

        protected override void Start()
        {
            health.OnDeath += Zombie_OnDeath;
            combat.Configure(new CombatArgs { Team = GetTeam() });
            
            _stateCurrent.OnEnter();
        }

        protected override void Update()
        {
            _stateCurrent.OnUpdate();
            // agent.destination = _targetTransform.position;
            // Attack();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            health.OnDeath -= Zombie_OnDeath;
        }

        public override void Configure(float pHealth)
        {
            health.SetHealthMax(pHealth);
        }

        #endregion

        #region FUNCTIONS

        public void Configure(ZombieArgs args)
        {
            _zombieAnimatorEvents = args.zombieAnimatorEvents;
        }

        public override void Attack()
        {
            if (_canAttack)
            {
                DoAttack().WrapErrors();
            }
        }

        private async Task DoAttack()
        {
            if (_isAttacking) return;

            _isAttacking = true;
            await Task.Delay(1000);
            combat.Attack();
            _isAttacking = false;
        }

        private void Zombie_OnDeath()
        {
            Destroy(health);
            agent.isStopped = true;
            _canAttack = false;
            Destroy(gameObject, 1.5f);
        }
        
        #endregion

        #region GETTERS
        
        public Team GetTeam() => health.GetTeam();
        public Animator GetAnimator() => animator;
        public NavMeshAgent GetNavMeshAgent() => agent;
        
        #endregion

        #region STATES
        
        private void ConfigureStateMachine()
        {
            var spawningState = new SpawningState(this, SwitchToChasingState);
            var chasingState = new ChasingState(this, SwitchToAttackingState);
            var attackingState = new AttackingState(this, SwitchToChasingState);
            
            _zombieStatesToIDs = new Dictionary<ZombieStateID, IState>()
            {
                { ZombieStateID.Spawning , spawningState },
                { ZombieStateID.Chasing, chasingState },
                { ZombieStateID.Attacking, attackingState },
            };

            // Set initial state
            _stateCurrent = chasingState;
            _stateIDCurrent = ZombieStateID.Chasing;
        }

        private void SwitchToChasingState() => SwitchState(ZombieStateID.Chasing);
        private void SwitchToAttackingState() => SwitchState(ZombieStateID.Attacking);
        private IState GetStateByID(ZombieStateID zombieStateID) => _zombieStatesToIDs[zombieStateID];

        private async void SwitchState(ZombieStateID zombieStateID)
        {
            await Task.Yield();
            _stateCurrent?.OnExit();
            _stateCurrent = GetStateByID(zombieStateID);
            _stateIDCurrent = zombieStateID;
            _stateCurrent?.OnEnter();
        }

        public ZombieStateID GetCurrentStateID() => _stateIDCurrent;
        public IZombieAnimatorEvents GetAnimatorEvents() => _zombieAnimatorEvents; // states may subscribe to events

        #endregion
    }
}