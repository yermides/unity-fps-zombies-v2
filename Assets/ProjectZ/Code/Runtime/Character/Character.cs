using System;
using System.Threading.Tasks;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Animation;
using ProjectZ.Code.Runtime.Common;
using ProjectZ.Code.Runtime.Utils;
using ProjectZ.Code.Runtime.Utils.Extensions;
using ProjectZ.Code.Runtime.Weapons;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public struct CharacterArgs
    {
        public ICharacterAnimatorEvents characterAnimatorEvents;
        public ICharacterInputEvents characterInputEvents;
    }

    [SelectionBase]
    public sealed class Character : CharacterBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Header("References"), HorizontalLine]

        [Tooltip("Character Health")]
        [SerializeField] private Health health;
        
        [Tooltip("Character Movement")]
        [SerializeField] private MovementBehaviour movement;
        
        [Tooltip("Character Combat")]
        [SerializeField] private CombatBehaviour combat;
        
        [Tooltip("Character Interactions")]
        [SerializeField] private InteractorBehaviour interactor;

        [Tooltip("Character IK")]
        [SerializeField] private CharacterKinematics characterKinematics;
        
        [Tooltip("Weapon Inventory")]
        [SerializeField] private InventoryBehaviour inventory;
        
        [Tooltip("World Camera")]
        [SerializeField] private new UnityEngine.Camera camera;
        
        [Tooltip("Character Animator")]
        [SerializeField] private Animator animator;
        
        [Header("Values"), HorizontalLine]
        
        [Tooltip("Determines how smooth the locomotion blend space is.")]
        [SerializeField] private float dampTimeLocomotion = 0.15f;

        [Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
        [SerializeField] private float dampTimeAiming = 0.3f;
        
        [Tooltip("Player points")]
        [SerializeField] private int points;
        
        [Tooltip("Zombie Attraction Weight"), Range(0, 1)]
        [SerializeField] private float attractionWeight = 1.0f ;

        #endregion

        #region FIELDS

        // Injected event receivers
        private ICharacterAnimatorEvents _characterAnimatorEvents;
        private ICharacterInputEvents _characterInputEvents;
        
        // Input related
        private Vector2 _movementInput;
        private Vector2 _lookInput;
        private bool _isHoldingButtonFire;
        private bool _isHoldingButtonAim;
        private bool _isHoldingButtonRun;
        
        // Weapon related
        private WeaponBehaviour _equippedWeapon;
        private float _lastShotTime;
        
        // States
        private bool _isAttacking; // Melee
        private bool _isAiming;
        private bool _isRunning;
        private bool _isReloading;
        private bool _isHolstering;
        
        // Animator Layers
        private int _layerOverlay;
        private int _layerHolster;
        private int _layerActions;

        #endregion
        
        #region UNITY

        private void Reset()
        {
            health = GetComponent<Health>();
            movement = GetComponent<MovementBehaviour>();
            combat = GetComponent<CombatBehaviour>();
            interactor = GetComponent<InteractorBehaviour>();
            camera = GetComponentInChildren<UnityEngine.Camera>();
            characterKinematics = GetComponent<CharacterKinematics>();
            animator = GetComponentInChildren<Animator>();
        }

        protected override void Awake()
        {
            // Store the indexes of the animation layers
            _layerOverlay = animator.GetLayerIndex(AnimatorHelper.LayerNameOverlay);
            _layerHolster = animator.GetLayerIndex(AnimatorHelper.LayerNameHolster);
            _layerActions = animator.GetLayerIndex(AnimatorHelper.LayerNameActions);

            // Configure the State Machine Behaviour dependencies
            var behaviours = animator.GetBehaviours<PlaySoundCharacterBehaviour>();
            
            foreach (var playSoundCharacterBehaviour in behaviours)
            {
                playSoundCharacterBehaviour.Configure(this);
            }
        }

        protected override void Start()
        {
            // Initialize Inventory
            inventory.Init(0);
            _equippedWeapon = inventory.GetWeaponEquipped();
            
            // Subscribe to events
            SubscribeInputEvents();
            SubscribeAnimationEvents();
            health.OnDeath += OnCharacterDeath;
        }

        protected override void Update()
        {
            // Match Aim
            _isAiming = _isHoldingButtonAim /* && CanAim() */;
            // Match Run
            _isRunning = _isHoldingButtonRun /* && CanRun() */;
            
            UpdateFire();
            UpdateAnimator();
        }

        protected override void LateUpdate()
        {
            characterKinematics.Compute();
        }

        protected override void OnDestroy()
        {
            // Unsubscribe from all events
            UnsubscribeAnimationEvents();
            UnsubscribeInputEvents();
            health.OnDeath -= OnCharacterDeath;
        }

        #endregion

        #region GETTERS

        public override UnityEngine.Camera GetWorldCamera() => camera;
        public override bool IsRunning() => _isHoldingButtonRun;
        public override bool IsAiming() => throw new System.NotImplementedException();
        public override Vector2 GetMovementInput() => _movementInput;
        public override Vector2 GetLookInput() => _lookInput;
        public override bool IsCursorLocked() => true;
        public override Team GetTeam() => health.GetTeam();
        public override WeaponBehaviour GetEquippedWeapon() => _equippedWeapon;
        public override int GetPoints() => points;
        public override void AddPoints(int pts) => points += pts;
        public override void RemovePoints(int pts) => points -= pts;

        #endregion

        #region INPUT

        private void OnMove(Vector2 value) => _movementInput = value;
        private void OnLook(Vector2 value) => _lookInput = value;
        private void OnFireStarted() => _isHoldingButtonFire = true;
        
        // Firing On-click for both auto and semi-auto weapons
        private void OnFirePerformed()
        {
            if (Time.time - _lastShotTime > _equippedWeapon.GetFireInterval())
            {
                Fire();
            }
        }
        
        private void OnFireCanceled() => _isHoldingButtonFire = false;
        private void OnRunStarted() => _isHoldingButtonRun = true;
        private void OnRunCanceled() => _isHoldingButtonRun = false;
        private void OnJumpPerformed() => movement.Jump();
        private void OnMeleePerformed() => DoAttack().WrapErrors();
        private void OnInteractPerformed() => interactor.DoInteract();

        private void OnCyclePerformed(float value)
        {
            int indexNext = value > 0 ? inventory.GetNextIndex() : inventory.GetPreviousIndex();
            
            // Get the current weapon's index
            int indexCurrent = inventory.GetEquippedIndex();

            // Equip next weapon if it's not the next
            if (indexCurrent != indexNext)
            {
                inventory.Equip(indexNext);
                _equippedWeapon = inventory.GetWeaponEquipped();
            }
        }

        private void OnReloadPerformed()
        {
            // Play reload animation
            var stateName = _equippedWeapon.HasAmmunition() ? AnimatorHelper.StateNameReload : AnimatorHelper.StateNameReloadEmpty;
            animator.Play(stateName, _layerActions, 0.0f);
            // _isReloading = true;
            
            // Reload the weapon
            _equippedWeapon.Reload();
        }

        private void OnAimStarted() => _isHoldingButtonAim = true;
        private void OnAimCanceled() => _isHoldingButtonAim = false;

        #endregion

        #region ANIMATION

        private void OnSlideBack(int obj) => throw new System.NotImplementedException();
        private void OnAnimationEndedHolster() => throw new System.NotImplementedException();
        private void OnAnimationEndedInspect() => throw new System.NotImplementedException();
        private void OnAnimationEndedMelee() => throw new System.NotImplementedException();
        private void OnAnimationEndedGrenadeThrow() => throw new System.NotImplementedException();
        private void OnAnimationEndedReload() => throw new System.NotImplementedException();
        private void OnAnimationEndedBolt() => throw new System.NotImplementedException();
        private void OnSetActiveMagazine(int active) => throw new System.NotImplementedException();
        private void OnGrenade() => throw new System.NotImplementedException();
        private void OnSetActiveKnife(int active) => throw new System.NotImplementedException();
        private void OnAmmunitionFill(int amount) => _equippedWeapon.FillMagazine();
        private void OnEjectCasing() => throw new System.NotImplementedException();

        #endregion

        #region FUNCTIONS

        public void Configure(CharacterArgs args)
        {
            _characterAnimatorEvents = args.characterAnimatorEvents;
            _characterInputEvents = args.characterInputEvents;
        }

        private void SubscribeInputEvents()
        {
            _characterInputEvents.MoveEvent += OnMove;
            _characterInputEvents.LookEvent += OnLook;
            _characterInputEvents.FireStartedEvent += OnFireStarted;
            _characterInputEvents.FirePerformedEvent += OnFirePerformed;
            _characterInputEvents.FireCanceledEvent += OnFireCanceled;
            _characterInputEvents.RunStartedEvent += OnRunStarted;
            _characterInputEvents.RunCanceledEvent += OnRunCanceled;
            _characterInputEvents.JumpPerformedEvent += OnJumpPerformed;
            _characterInputEvents.MeleePerformedEvent += OnMeleePerformed;
            _characterInputEvents.InteractPerformedEvent += OnInteractPerformed;
            _characterInputEvents.CyclePerformedEvent += OnCyclePerformed;
            _characterInputEvents.ReloadPerformedEvent += OnReloadPerformed;
            _characterInputEvents.AimStartedEvent += OnAimStarted;
            _characterInputEvents.AimCanceledEvent += OnAimCanceled;
        }

        private void UnsubscribeInputEvents()
        {
            _characterInputEvents.MoveEvent -= OnMove;
            _characterInputEvents.LookEvent -= OnLook;
            _characterInputEvents.FireStartedEvent -= OnFireStarted;
            _characterInputEvents.FirePerformedEvent -= OnFirePerformed;
            _characterInputEvents.FireCanceledEvent -= OnFireCanceled;
            _characterInputEvents.RunStartedEvent -= OnRunStarted;
            _characterInputEvents.RunCanceledEvent -= OnRunCanceled;
            _characterInputEvents.JumpPerformedEvent -= OnJumpPerformed;
            _characterInputEvents.MeleePerformedEvent -= OnMeleePerformed;
            _characterInputEvents.InteractPerformedEvent -= OnInteractPerformed;
            _characterInputEvents.CyclePerformedEvent -= OnCyclePerformed;
            _characterInputEvents.ReloadPerformedEvent -= OnReloadPerformed;
            _characterInputEvents.AimStartedEvent -= OnAimStarted;
            _characterInputEvents.AimCanceledEvent -= OnAimCanceled;
        }
        
        private void SubscribeAnimationEvents()
        {
            // TODO: implement all event definitions
            _characterAnimatorEvents.EjectCasingEvent += OnEjectCasing;
            _characterAnimatorEvents.AmmunitionFillEvent += OnAmmunitionFill;
            _characterAnimatorEvents.SetActiveKnifeEvent += OnSetActiveKnife;
            _characterAnimatorEvents.GrenadeEvent += OnGrenade;
            _characterAnimatorEvents.SetActiveMagazineEvent += OnSetActiveMagazine;
            _characterAnimatorEvents.AnimationEndedBoltEvent += OnAnimationEndedBolt;
            _characterAnimatorEvents.AnimationEndedReloadEvent += OnAnimationEndedReload;
            _characterAnimatorEvents.AnimationEndedGrenadeThrowEvent += OnAnimationEndedGrenadeThrow;
            _characterAnimatorEvents.AnimationEndedMeleeEvent += OnAnimationEndedMelee;
            _characterAnimatorEvents.AnimationEndedInspectEvent += OnAnimationEndedInspect;
            _characterAnimatorEvents.AnimationEndedHolsterEvent += OnAnimationEndedHolster;
            _characterAnimatorEvents.SlideBackEvent += OnSlideBack;
        }

        private void UnsubscribeAnimationEvents()
        {
            _characterAnimatorEvents.EjectCasingEvent -= OnEjectCasing;
            _characterAnimatorEvents.AmmunitionFillEvent -= OnAmmunitionFill;
            _characterAnimatorEvents.SetActiveKnifeEvent -= OnSetActiveKnife;
            _characterAnimatorEvents.GrenadeEvent -= OnGrenade;
            _characterAnimatorEvents.SetActiveMagazineEvent -= OnSetActiveMagazine;
            _characterAnimatorEvents.AnimationEndedBoltEvent -= OnAnimationEndedBolt;
            _characterAnimatorEvents.AnimationEndedReloadEvent -= OnAnimationEndedReload;
            _characterAnimatorEvents.AnimationEndedGrenadeThrowEvent -= OnAnimationEndedGrenadeThrow;
            _characterAnimatorEvents.AnimationEndedMeleeEvent -= OnAnimationEndedMelee;
            _characterAnimatorEvents.AnimationEndedInspectEvent -= OnAnimationEndedInspect;
            _characterAnimatorEvents.AnimationEndedHolsterEvent -= OnAnimationEndedHolster;
            _characterAnimatorEvents.SlideBackEvent -= OnSlideBack;
        }

        private void Fire()
        {
            _lastShotTime = Time.time;
            _equippedWeapon.Fire();
            animator.CrossFade(AnimatorHelper.StateNameFire, 0.05f, _layerOverlay, 0);
        }
        
        private void UpdateFire()
        {
            // Update Firing
            if (!_isHoldingButtonFire) return;

            // TODO: Check for rounds and if is automatic
            if (_equippedWeapon.IsAutomatic())
            {
                // Has fire rate passed.
                if (Time.time - _lastShotTime > _equippedWeapon.GetFireInterval())
                {
                    Fire();
                }
            }
        }
        
        private void UpdateAnimator()
        {
            animator.SetFloat(
                AnimatorHelper.HashMovement, 
                Mathf.Clamp01(Mathf.Abs(_movementInput.x) + Mathf.Abs(_movementInput.y)), 
                dampTimeLocomotion, 
                Time.deltaTime);
            
            animator.SetFloat(
                AnimatorHelper.HashAimingAlpha, 
                Convert.ToSingle(_isAiming), 
                0.25f / 1.0f * dampTimeAiming, 
                Time.deltaTime);

            animator.SetBool(AnimatorHelper.HashAiming, _isAiming);
            animator.SetBool(AnimatorHelper.HashRunning, _isRunning);
        }

        private void OnCharacterDeath()
        {
            // TODO: this should be managed inside Game class
#if UNITY_STANDALONE
            // Application.Quit();
#endif
 
#if UNITY_EDITOR
            // UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private async Task DoAttack()
        {
            if (_isAttacking) return;

            _isAttacking = true;
            await Task.Delay(1000);
            combat.Attack();
            _isAttacking = false;
        }

        #endregion
    }
}