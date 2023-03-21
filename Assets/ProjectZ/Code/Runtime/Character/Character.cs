using System.Threading.Tasks;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Common;
using ProjectZ.Code.Runtime.Utils.Extensions;
using ProjectZ.Code.Runtime.Weapons;
using ProjectZ.Code.Runtime.Zombie;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace ProjectZ.Code.Runtime.Character
{
    [SelectionBase]
    public sealed class Character : CharacterBehaviour
    {
        #region FIELDS SERIALIZED
        
        [FormerlySerializedAs("inputReader")]
        [Foldout("References"), Tooltip("Character Input Reader")]
        [SerializeField] private CharacterInputReader characterInputReader;

        [Foldout("References"), Tooltip("Character Health")]
        [SerializeField] private Health health;
        
        [Foldout("References"), Tooltip("Character Movement")]
        [SerializeField] private MovementBehaviour movement;
        
        [Foldout("References"), Tooltip("Character Combat")]
        [SerializeField] private CombatBehaviour combat;
        
        [Foldout("References"), Tooltip("Character Interactions")]
        [SerializeField] private InteractorBehaviour interactor;

        [Foldout("References"), Tooltip("Character IK")]
        [SerializeField] private CharacterKinematics characterKinematics;
        
        [Foldout("References"), Tooltip("Weapon Inventory")]
        [SerializeField] private InventoryBehaviour inventory;
        
        [Foldout("References"), Tooltip("World Camera")]
        [SerializeField] private new UnityEngine.Camera camera;
        
        [Foldout("References"), Tooltip("Character Animator")]
        [SerializeField] private Animator animator;
        
        [Foldout("Values"), Tooltip("Player points")]
        [SerializeField] private int points;
        
        [Foldout("Values"), Tooltip("Zombie Attraction Weight"), Range(0, 1)]
        [SerializeField] private float attractionWeight = 1.0f ;

        #endregion

        #region FIELDS

        private Vector2 _movementInput;
        private Vector2 _lookInput;
        private bool _isPressingButtonRun;
        private bool _isHoldingButtonFire;
        private WeaponBehaviour _equippedWeapon;
        private bool _isAttacking;
        private float _lastShotTime;
        private ICharacterAnimatorEventCaster _characterAnimatorEventCaster;

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
            animator = GetComponent<Animator>();
        }

        protected override void Awake()
        {
            health.OnDeath += Character_OnDeath;
        }

        protected override void Start()
        {
            inventory.Init(0);
            _equippedWeapon = inventory.GetWeaponEquipped();
            SubscribeInputEvents();
            SubscribeAnimationEvents();
        }

        // TODO:
        private void SubscribeAnimationEvents()
        {
            _characterAnimatorEventCaster.GrenadeEvent += delegate {  };
        }

        protected override void Update()
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

        protected override void LateUpdate()
        {
            characterKinematics.Compute();
        }

        protected override void OnDestroy()
        {
            UnsubscribeInputEvents();
            health.OnDeath -= Character_OnDeath;
        }

        private void Print(Vector2 moveDir) => print($"Character Dir: {moveDir}");

        #endregion

        #region GETTERS

        public override UnityEngine.Camera GetWorldCamera() => camera;

        public override bool IsRunning() => _isPressingButtonRun;

        public override bool IsAiming()
        {
            throw new System.NotImplementedException();
        }

        public override Vector2 GetMovementInput() => _movementInput;
        public override Vector2 GetLookInput() => _lookInput;
        public override bool IsCursorLocked() => true;
        // Cursor.lockState == CursorLockMode.Locked;
        public override Team GetTeam() => health.GetTeam();

        public override int GetPoints() => points;

        public override void AddPoints(int pts) => points += pts;
        
        public override void RemovePoints(int pts) => points -= pts;

        #endregion

        #region INPUT

        private void OnMove(Vector2 value)
        {
            _movementInput = value;
        }
        
        private void OnLook(Vector2 value)
        {
            _lookInput = value;
        }

        private void OnFireStarted()
        {
            _isHoldingButtonFire = true;
        }

        private void OnFirePerformed()
        {
            // Fire for semi-auto weapons
            if (Time.time - _lastShotTime > _equippedWeapon.GetFireInterval())
            {
                Fire();
            }
        }
        
        private void OnFireCanceled()
        {
            _isHoldingButtonFire = false;
        }

        private void OnRunStarted()
        {
            _isPressingButtonRun = true;
        }

        private void OnRunCanceled()
        {
            _isPressingButtonRun = false;
        }

        private void OnJumpPerformed()
        {
            movement.Jump();
        }

        private void OnMeleePerformed()
        {
            DoAttack().WrapErrors();
        }

        private void OnInteractPerformed()
        {
            interactor.DoInteract();
        }
        
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

        #endregion

        #region FUNCTIONS

        public void Configure(ICharacterAnimatorEventCaster characterAnimatorEventCaster)
        {
            _characterAnimatorEventCaster = characterAnimatorEventCaster;
        }

        private void SubscribeInputEvents()
        {
            characterInputReader.MoveEvent += OnMove;
            characterInputReader.LookEvent += OnLook;
            characterInputReader.FireStartedEvent += OnFireStarted;
            characterInputReader.FirePerformedEvent += OnFirePerformed;
            characterInputReader.FireCanceledEvent += OnFireCanceled;
            characterInputReader.RunStartedEvent += OnRunStarted;
            characterInputReader.RunCanceledEvent += OnRunCanceled;
            characterInputReader.JumpPerformedEvent += OnJumpPerformed;
            characterInputReader.MeleePerformedEvent += OnMeleePerformed;
            characterInputReader.InteractPerformedEvent += OnInteractPerformed;
            characterInputReader.CyclePerformedEvent += OnCyclePerformed;
            characterInputReader.ReloadPerformedEvent += OnReloadPerformed;
        }

        private void OnReloadPerformed()
        {
            if (_equippedWeapon == null) return;
            
            _equippedWeapon.Reload();
        }

        private void UnsubscribeInputEvents()
        {
            characterInputReader.MoveEvent -= OnMove;
            characterInputReader.LookEvent -= OnLook;
            characterInputReader.FireStartedEvent -= OnFireStarted;
            characterInputReader.FirePerformedEvent -= OnFirePerformed;
            characterInputReader.FireCanceledEvent -= OnFireCanceled;
            characterInputReader.RunStartedEvent -= OnRunStarted;
            characterInputReader.RunCanceledEvent -= OnRunCanceled;
            characterInputReader.JumpPerformedEvent -= OnJumpPerformed;
            characterInputReader.MeleePerformedEvent -= OnMeleePerformed;
            characterInputReader.InteractPerformedEvent -= OnInteractPerformed;
            characterInputReader.CyclePerformedEvent -= OnCyclePerformed;
            characterInputReader.ReloadPerformedEvent -= OnReloadPerformed;
        }

        private void Fire()
        {
            _lastShotTime = Time.time;
            _equippedWeapon.Fire();
        }

        private void Character_OnDeath()
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