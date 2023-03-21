using NaughtyAttributes;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Common
{
    public sealed class Combat : CombatBehaviour
    {
        #region FIELDS SERIALIZED

        [Foldout("References")] 
        [SerializeField] private Transform attackSocket;
        
        [Foldout("Values")] 
        [SerializeField] private float attackRadius = 0.5f;
        
        [Foldout("Values")] 
        [SerializeField] private float attackDamage = 20.0f;

        [Foldout("Values")] 
        [SerializeField] private LayerMask attackMask;

        #endregion

        #region FIELDS

        private Team _team;
        private readonly Collider[] _colliderCache = new Collider[1];

        #endregion

        #region FUNCTIONS

        public override void Configure(CombatArgs args)
        {
            _team = args.Team;
        }

        public override void Attack()
        {
            var contacts = Physics.OverlapSphereNonAlloc(attackSocket.position, attackRadius, _colliderCache, attackMask.value);

            if (contacts < 1) return;

            var col = _colliderCache[0];

            if (col.TryGetComponent(out Health otherHealth) && otherHealth.GetTeam() != _team)
            {
                otherHealth.Damage(attackDamage);
            }
        }
        
        #endregion

        #region UNITY EDITOR

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!attackSocket) return;
            
            var position = attackSocket.position;
            
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the attack
            Gizmos.DrawSphere(position, attackRadius);
        }
#endif

        #endregion
    }
}