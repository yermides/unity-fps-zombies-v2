using System;
using ProjectZ.Code.Runtime.Common;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    public struct ProjectileArgs
    {
        public Team Team;
        public float Damage;
        public LayerMask LayerMask;
        public int Layer;
        public Action OnImpact;
        public Action OnKill;
    }

    public abstract class ProjectileBehaviour : MonoBehaviour
    {
        public abstract void Configure(ProjectileArgs args);
        protected virtual void Start() { }
        protected virtual void OnDestroy() { }
        private void OnCollisionEnter(Collision collision)
        {
            DoCollision(collision);
        }
        protected abstract void DoCollision(Collision collision);
    }
}