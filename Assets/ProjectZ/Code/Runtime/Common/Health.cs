using System;
using NaughtyAttributes;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Common
{
    public struct HealthArgs
    { 
        public float HealthPrevious, HealthNew;
    }

    public class Health : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField] private float healthMax = 150.0f;
        [SerializeField] private Team team;
        [SerializeField] private bool invulnerable;

        #endregion

        #region FIELDS

        [ShowNonSerializedField] private float _healthCurrent;

        #endregion

        #region EVENTS

        public event Action<HealthArgs> OnHeal;
        public event Action<HealthArgs> OnDamage;
        public event Action OnDeath;

        #endregion
        
        private void Start()
        {
            _healthCurrent = healthMax;
        }

        public void Heal(float amount)
        {
            var previous = _healthCurrent;
            _healthCurrent = Mathf.Clamp(_healthCurrent, 0, _healthCurrent + amount);
            OnHeal?.Invoke(new HealthArgs { HealthPrevious = previous, HealthNew = _healthCurrent });
        }

        public void Damage(float amount)
        {
            if (invulnerable) return;
            
            var previous = _healthCurrent;
            _healthCurrent = Mathf.Clamp(_healthCurrent, 0, _healthCurrent - amount);
            OnDamage?.Invoke(new HealthArgs { HealthPrevious = previous, HealthNew = _healthCurrent });

            if (_healthCurrent <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public bool IsDead() => _healthCurrent <= 0;
        public float GetHealth() => _healthCurrent;
        public void SetHealth(float pHealth) => _healthCurrent = pHealth;
        
        public float GetHealthMax() => healthMax;
        public void SetHealthMax(float pHealth) => healthMax = pHealth;
        public Team GetTeam() => team;
    }
}