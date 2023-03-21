using System;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Common;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    public class Weapon : WeaponBehaviour
    {
        #region FIELDS SERIALIZED
        
        [SerializeField] private WeaponID weaponID;
        
        [Foldout("References")]
        [SerializeField] private Transform tipPoint;

        [Foldout("Values")] 
        [Tooltip("Rate of fire, in rounds per minute")]
        [SerializeField] private float rateOfFire = 700.0f;
        [Foldout("Values")] 
        [SerializeField] private float damage = 30.0f;
        [Foldout("Values")] 
        [SerializeField] private bool isAutomatic;
        [Foldout("Values")]
        [SerializeField] private bool usesProjectiles; // Hitscan or projectile-based
        [Foldout("Values")]
        [SerializeField] private int ammunitionMagazineTotal = 30; // Max bullets in magazine
        [Foldout("Values")]
        [SerializeField] private int ammunitionInventoryTotal = 150; // Max bullets in inventory
        
        [Foldout("Raycast")]
        [SerializeField, HideIf(nameof(usesProjectiles))] private LayerMask raycastLayerMask;
        
        // Projectiles' layer should be configured inside the prefab in reality
        [Foldout("Projectiles"), ShowIf(nameof(usesProjectiles)), Layer]
        [SerializeField] private int projectileLayerIndex;
        [Foldout("Projectiles"), ShowIf(nameof(usesProjectiles))]
        [SerializeField] private ProjectileBehaviour projectilePrefab;
        [Foldout("Projectiles"), ShowIf(nameof(usesProjectiles))]
        [SerializeField] private float projectileImpulse;
        
        #endregion

        #region FIELDS

        private CharacterBehaviour _characterBehaviour;
        private int _ammunitionMagazineCurrent; // Bullets in magazine
        private int _ammunitionInventoryCurrent; // Current bullets in inventory

        #endregion

        #region UNITY

        protected override void Awake()
        {
            _characterBehaviour = GetComponentInParent<CharacterBehaviour>();
            
            // Fully reloaded on Awake
            Refill();
        }

        #endregion

        #region GETTERS

        public override WeaponID GetWeaponId() => weaponID;
        public override int GetAmmunitionCurrent() => _ammunitionMagazineCurrent;
        public override int GetAmmunitionTotal() => ammunitionMagazineTotal;
        public override int GetAmmunitionInventoryCurrent() => _ammunitionInventoryCurrent;
        public override int GetAmmunitionInventoryTotal() => ammunitionInventoryTotal;
        public override bool IsAutomatic() => isAutomatic;
        public override bool HasAmmunition() => GetAmmunitionCurrent() > 0;

        public override bool IsFull() => 
            GetAmmunitionCurrent() == GetAmmunitionTotal() && 
            GetAmmunitionInventoryCurrent() == GetAmmunitionInventoryTotal();
        
        public override float GetRateOfFire() => rateOfFire;

        #endregion

        #region FUNCTIONS

        public override void Fire()
        {
            if (!HasAmmunition()) return;
            
            if (usesProjectiles)
            {
                ShootProjectile();
            }
            else
            {
                ShootRaycast();
            }

            _ammunitionMagazineCurrent--;
        }

        public override void Reload()
        {
            if (GetAmmunitionInventoryTotal() <= 0) return;
            
            var firedRounds = GetAmmunitionTotal() - GetAmmunitionCurrent();
            var roundsToReload = Mathf.Min(GetAmmunitionInventoryCurrent(), firedRounds);
            
            _ammunitionMagazineCurrent += roundsToReload;
            _ammunitionInventoryCurrent -= roundsToReload;
        }

        public override void Refill()
        {
            _ammunitionMagazineCurrent = ammunitionMagazineTotal;
            _ammunitionInventoryCurrent = ammunitionInventoryTotal;
        }

        private Transform GetTipPoint() => tipPoint;

        private void ShootProjectile()
        {
            var firingPoint = GetTipPoint();
            var targetForward = firingPoint.forward;
            var args = new ProjectileArgs
            {
                Team = _characterBehaviour.GetTeam(), 
                Damage = damage, 
                LayerMask = raycastLayerMask, 
                Layer = projectileLayerIndex,
                OnImpact = delegate { _characterBehaviour.AddPoints(10); },  
                OnKill = delegate { _characterBehaviour.AddPoints(50); },  
            };
            
            // Instantiate and configure projectile dependencies
            var bullet = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity);
            bullet.Configure(args);
            
            // Apply bullet force
            var bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bullet.transform.forward = targetForward;
            bulletRigidbody.velocity = targetForward * projectileImpulse;
        }

        // TODO: ignore collision of the CharacterController (use layers or something, I used to use TransparentFX on the character)
        private void ShootRaycast()
        {
            var playerCamera = _characterBehaviour.GetWorldCamera().transform;
            var ray = new Ray(playerCamera.position, playerCamera.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000, raycastLayerMask.value)) return;
            if (!hit.collider.TryGetComponent(out Health health)) return;

            health.Damage(damage);
        }

        #endregion
    }
}