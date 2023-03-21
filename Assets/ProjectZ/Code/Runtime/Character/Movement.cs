using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProjectZ.Code.Runtime.Character
{
    public sealed class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED

        [Foldout("References")]
        [SerializeField] private CharacterBehaviour character;
        [Foldout("References")]
        [SerializeField] private CharacterController characterController;

        [Foldout("Acceleration")]
        [SerializeField] private float acceleration = 9.0f;
        [Foldout("Acceleration")]
        [SerializeField] private float deceleration = 11.0f;
        [Foldout("Speed")]
        [SerializeField] private float speedWalking = 4.0f;
        [Foldout("Speed")]
        [SerializeField] private float speedRunning = 7.0f;

        [Foldout("Jump")] 
        [SerializeField] private float accelerationAir = 3.0f;
        [Foldout("Jump")] 
        [SerializeField] private float gravityDescending = 25.0f;
        [Foldout("Jump")]
        [SerializeField] private float gravityAscending = 18.0f;
        [Foldout("Jump")] 
        [SerializeField] private float jumpHeight = 1.0f;
        [Foldout("Jump")] 
        [SerializeField] private float groundStickyForce = 0.05f;
        
        #endregion

        #region FIELDS

        private Transform _transform;
        private bool _isRunning;
        private bool _isJumping; // Only true when we call Jump() and until we reach ground
        private bool _isGrounded;
        private bool _wasGrounded;
        private Vector3 _velocity;

        #endregion
        
        #region UNITY

        private void Reset()
        {
            character = GetComponent<CharacterBehaviour>();
            characterController = GetComponent<CharacterController>();
        }
        
        protected override void Awake()
        {
            _transform = transform;
        }

        protected override void Update()
        {
            _isGrounded = IsGrounded();

            // If we touched ground this frame, we are no longer jumping (even if we weren't)
            if (_isGrounded && !_wasGrounded)
            {
                _isJumping = false;
            }
            
            Move();

            _wasGrounded = _isGrounded;
        }

        #endregion

        #region GETTERS

        public override bool IsGrounded() => characterController.isGrounded;

        #endregion

        #region FUNCTIONS

        private void Move()
        {
            var input = Vector3.ClampMagnitude(character.GetMovementInput(), 1.0f);
            var targetDirection = new Vector3(input.x, 0, input.y);

            // Get speed
            var speed = character.IsRunning() ? speedRunning : speedWalking;
            targetDirection *= speed;
            
            // Convert to world space
            targetDirection = _transform.TransformDirection(targetDirection);
            
            // Apply gravity and jump forces
            if (!_isGrounded)
            {
                // If we fell (mid-air with no jump request), negate velocity upwards so we start descending
                if (_wasGrounded && !_isJumping)
                {
                    _velocity.y = 0.0f;
                }
                
                // Apply mid-air movement
                _velocity += targetDirection * (accelerationAir * Time.deltaTime);

                // Clamp velocity XZ mid air to normal velocity
                var velocityXZ = new Vector3(_velocity.x, 0, _velocity.z);
                velocityXZ = Vector3.ClampMagnitude(velocityXZ, speed);
                
                _velocity = new Vector3(velocityXZ.x, _velocity.y, velocityXZ.z);

                // Apply gravity force
                bool isAscending = _velocity.y >= 0;
                _velocity.y -= (isAscending ? gravityAscending : gravityDescending) * Time.deltaTime;
            } 
            // Moving totally on ground
            else if (!_isJumping)
            {
                // If we received no input, magnitude is zero and therefore we are stopping
                bool isStopping = targetDirection.sqrMagnitude <= 0.0f;
                
                // Velocity change
                _velocity = Vector3.Lerp(
                    _velocity, 
                    new Vector3(targetDirection.x, _velocity.y, targetDirection.z),
                    Time.deltaTime * (isStopping ? deceleration : acceleration)
                    );
            }
            
            Vector3 applied = _velocity * Time.deltaTime;

            if (characterController.isGrounded && !_isJumping)
            {
                // Prevent air sliding when descending slopes
                applied.y -= groundStickyForce;
            }

            characterController.Move(applied);
        }

        public override void Jump()
        {
            // Avoid jumping again mid-air
            if (!_isGrounded) return;
            
            // Cache the jumping state for the Move()
            _isJumping = true;
            
            // Apply jump velocity
            _velocity = new Vector3(_velocity.x, Mathf.Sqrt(2.0f * jumpHeight * gravityAscending), _velocity.z);
        }

        #endregion
    }
}