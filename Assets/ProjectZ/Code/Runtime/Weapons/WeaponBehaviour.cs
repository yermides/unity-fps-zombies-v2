using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        #region UNITY

        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}

        #endregion
        
        #region GETTERS

        public abstract WeaponID GetWeaponId();
        public abstract int GetAmmunitionCurrent();
        public abstract int GetAmmunitionTotal();
        public abstract int GetAmmunitionInventoryCurrent();
        public abstract int GetAmmunitionInventoryTotal();
        public abstract bool IsAutomatic();
        public abstract bool HasAmmunition();
        public abstract bool IsFull();
        public abstract float GetRateOfFire();

        public float GetFireInterval() => 60.0f / GetRateOfFire();

        #endregion

        #region FUNCTIONS
        
        public abstract void Fire();
        public abstract void Reload();

        public abstract void Refill();

        #endregion
    }
}