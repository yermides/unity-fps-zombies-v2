using System;
using ProjectZ.Code.Runtime.Common;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    public sealed class Projectile : ProjectileBehaviour
    {
        #region FIELDS SERIALIZED

        [Tooltip("Self-destructs in X seconds")]
        [SerializeField] private float lifetimeSeconds = 3.0f;
        
        #endregion

        #region FIELDS

        private Team _team;
        private float _damage;
        private int _layer;

        #endregion

        #region EVENT

        private event Action OnImpact;
        private event Action OnKill;

        #endregion

        public override void Configure(ProjectileArgs args)
        {
            _team = args.Team;
            _damage = args.Damage;
            _layer = args.Layer;
            OnImpact = args.OnImpact;
            OnKill = args.OnKill;

            gameObject.layer = _layer;
        }

        protected override void Start()
        {
            Destroy(gameObject, lifetimeSeconds);
        }

        protected override void OnDestroy()
        {
            OnImpact = null;
            OnKill = null;
        }

        protected override void DoCollision(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out Health health)) return;
            if (_team == health.GetTeam()) return;
            
            OnImpact?.Invoke();
            
            health.Damage(_damage);

            if (health.IsDead())
            {
                OnKill?.Invoke();
                OnKill = null;
            }
            
            Destroy(gameObject);
        }
    }
}