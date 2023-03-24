using NaughtyAttributes;
using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Common;
using ProjectZ.Code.Runtime.Core.Audio;
using ProjectZ.Code.Runtime.Utils;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    public class Weapon : WeaponBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Header("References"), HorizontalLine]

        [SerializeField] private WeaponID weaponID;
        [SerializeField] private Transform tipPoint;
        [SerializeField] private Animator weaponAnimator;
        [Tooltip("The AnimatorController the Character will use with this weapon")]
        [SerializeField] private RuntimeAnimatorController characterAnimatorController;

        [Header("Values"), HorizontalLine]

        [Tooltip("Rate of fire, in rounds per minute")]
        [SerializeField] private float rateOfFire = 700.0f;
        [SerializeField] private float damage = 30.0f;
        [SerializeField] private bool isAutomatic;
        [SerializeField] private int ammunitionMagazineTotal = 30; // Max bullets in magazine
        [SerializeField] private int ammunitionInventoryTotal = 150; // Max bullets in inventory

        [Header("Audio Clips"), HorizontalLine] 
        [SerializeField] private AudioClipID audioClipHolster;
        [SerializeField] private AudioClipID audioClipUnholster;
        [SerializeField] private AudioClipID audioClipReload;
        [SerializeField] private AudioClipID audioClipReloadEmpty;
        [SerializeField] private AudioClipID audioClipFire;
        [SerializeField] private AudioClipID audioClipFireEmpty;
        
        [Header("Additional Shooting Data"), HorizontalLine] 
        [SerializeField] private bool usesProjectiles; // Hitscan or projectile-based
        
        [SerializeField, HideIf(nameof(usesProjectiles))] private LayerMask raycastLayerMask;
        // Projectiles' layer should be configured inside the prefab in reality
        [ShowIf(nameof(usesProjectiles)), Layer]
        [SerializeField] private int projectileLayerIndex;
        [ShowIf(nameof(usesProjectiles))]
        [SerializeField] private ProjectileBehaviour projectilePrefab;
        [ShowIf(nameof(usesProjectiles))]
        [SerializeField] private float projectileImpulse;
        
        #endregion

        #region FIELDS

        private CharacterBehaviour _characterBehaviour;
        [ShowNonSerializedField] private int _ammunitionMagazineCurrent; // Bullets in magazine
        [ShowNonSerializedField] private int _ammunitionInventoryCurrent; // Current bullets in inventory

        #endregion

        #region UNITY

        protected override void Awake()
        {
            _characterBehaviour = GetComponentInParent<CharacterBehaviour>();
            // weaponAnimator = GetComponentInChildren<Animator>();
            
            // Fully reloaded on Awake
            FillAmmunition();
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
        public override AudioClipID GetAudioClipHolster()
        {
            throw new System.NotImplementedException();
        }

        public override AudioClipID GetAudioClipUnholster()
        {
            throw new System.NotImplementedException();
        }

        public override AudioClipID GetAudioClipReload()
        {
            throw new System.NotImplementedException();
        }

        public override AudioClipID GetAudioClipReloadEmpty()
        {
            throw new System.NotImplementedException();
        }

        public override AudioClipID GetAudioClipFire()
        {
            throw new System.NotImplementedException();
        }

        public override AudioClipID GetAudioClipFireEmpty()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region FUNCTIONS

        public override void Fire()
        {
            if (!HasAmmunition()) return;
            
            weaponAnimator.Play(AnimatorHelper.StateNameFire, 0, 0.0f);
            
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
            var stateName = HasAmmunition() ? AnimatorHelper.StateNameReload : AnimatorHelper.StateNameReloadEmpty;
            weaponAnimator.Play(stateName, 0, 0.0f);
        }

        // Refills current magazine by taking bullets from inventory
        public override void FillMagazine()
        {
            var firedRounds = GetAmmunitionTotal() - GetAmmunitionCurrent();
            var roundsToReload = Mathf.Min(GetAmmunitionInventoryCurrent(), firedRounds);
            
            _ammunitionMagazineCurrent += roundsToReload;
            _ammunitionInventoryCurrent -= roundsToReload;
        }

        // Refills all ammunition without animations
        public override void FillAmmunition()
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