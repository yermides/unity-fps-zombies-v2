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
        [SerializeField] private string weaponName = "Default";

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
        private Transform _characterCamera;
        [ShowNonSerializedField] private int _ammunitionMagazineCurrent; // Bullets in magazine
        [ShowNonSerializedField] private int _ammunitionInventoryCurrent; // Current bullets in inventory

        #endregion

        #region UNITY

        protected override void Awake()
        {
            _characterBehaviour = GetComponentInParent<CharacterBehaviour>();
            _characterCamera = _characterBehaviour.GetWorldCamera().transform;
            // weaponAnimator = GetComponentInChildren<Animator>();
            
            // Fully reloaded on Awake
            FillAmmunition();
        }

        #endregion

        #region GETTERS

        public override WeaponID GetWeaponID() => weaponID;
        public override string GetWeaponName() => weaponName;
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

        public override AudioClipID GetAudioClipHolster() => audioClipHolster;
        public override AudioClipID GetAudioClipUnholster()=> audioClipUnholster;
        public override AudioClipID GetAudioClipReload() => audioClipReload;
        public override AudioClipID GetAudioClipReloadEmpty() => audioClipReloadEmpty;
        public override AudioClipID GetAudioClipFire() => audioClipFire;
        public override AudioClipID GetAudioClipFireEmpty() => audioClipFireEmpty;
        public override RuntimeAnimatorController GetCharacterAnimatorController() => characterAnimatorController;

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
            var firingPointPosition = GetTipPoint().position;
            
            var args = new ProjectileArgs
            {
                Team = _characterBehaviour.GetTeam(), 
                Damage = damage, 
                LayerMask = raycastLayerMask, 
                Layer = projectileLayerIndex,
                OnImpact = delegate { _characterBehaviour.AddPoints(10); },  
                OnKill = delegate { _characterBehaviour.AddPoints(50); },  
            };
            
            Quaternion rotation = Quaternion.LookRotation(_characterCamera.forward * 1000.0f - firingPointPosition);
            
            // Instantiate and configure projectile dependencies
            var bullet = Instantiate(projectilePrefab, firingPointPosition, rotation);
            bullet.Configure(args);
            
            // Apply bullet force
            var bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = bullet.transform.forward * projectileImpulse;
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