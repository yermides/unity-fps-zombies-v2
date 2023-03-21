using NaughtyAttributes;
using ProjectZ.Code.Runtime.Character;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Camera
{
    public class CameraLook : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Foldout("References"), Tooltip("Player Character")] 
        [SerializeField] private CharacterBehaviour playerCharacter;
        
        [Foldout("References"), Tooltip("Character Body to rotate in X (Pitch)")] 
        [SerializeField] private Transform characterBody;

        [Foldout("Settings"), Tooltip("Sensitivity when looking around.")] 
        [SerializeField] private Vector2 sensitivity = new Vector2(1, 1);

        [Foldout("Settings"), Tooltip("Minimum and maximum up/down rotation angle the camera can have.")]
        [SerializeField] private Vector2 yClamp = new Vector2(-60, 60);

        [Foldout("Interpolation"), Tooltip("Should the look rotation be interpolated?")] 
        [SerializeField] private bool smooth;

        [Foldout("Interpolation"), Tooltip("The speed at which the look rotation is interpolated.")] 
        [SerializeField] private float interpolationSpeed = 25.0f;

        #endregion

        #region FIELDS

        /// <summary>
        /// The player character's rotation.
        /// </summary>
        private Quaternion _rotationCharacter;

        /// <summary>
        /// The camera's rotation.
        /// </summary>
        private Quaternion _rotationCamera;

        #endregion

        #region UNITY

        private void Reset()
        {
            playerCharacter = GetComponentInParent<CharacterBehaviour>();
            
            // Transform to be affected by the camera-like rotation, must hold the camera inside it's hierarchy 
            characterBody = playerCharacter.transform.GetChild(0);
        }

        private void Start()
        {
            //Cache the character's initial rotation.
            _rotationCharacter = playerCharacter.transform.localRotation;
            //Cache the camera's initial rotation.
            _rotationCamera = characterBody.localRotation;
        }

        private void LateUpdate()
        {
            // Frame Input. The Input to add this frame!
            Vector2 frameInput = playerCharacter.IsCursorLocked() ? playerCharacter.GetLookInput() : default;
            // Sensitivity.
            frameInput *= sensitivity;

            // Yaw.
            Quaternion rotationYaw = Quaternion.Euler(0.0f, frameInput.x, 0.0f);
            // Pitch.
            Quaternion rotationPitch = Quaternion.Euler(-frameInput.y, 0.0f, 0.0f);

            //Save rotation. We use this for smooth rotation.
            _rotationCamera *= rotationPitch;
            _rotationCamera = Clamp(_rotationCamera);

            _rotationCharacter *= rotationYaw;

            // Local Rotation
            var localRotation = characterBody.localRotation;

            // Smooth
            if (smooth)
            {
                // Interpolate local rotation
                localRotation = Quaternion.Slerp(localRotation, _rotationCamera, Time.deltaTime * interpolationSpeed);
                
                // Clamp
                localRotation = Clamp(localRotation);
                
                // Interpolate character rotation
                playerCharacter.transform.rotation = Quaternion.Slerp(
                    playerCharacter.transform.rotation,
                    _rotationCharacter,
                    Time.deltaTime * interpolationSpeed
                );
            }
            else
            {
                // Rotate local
                localRotation *= rotationPitch;
                
                // Clamp
                localRotation = Clamp(localRotation);

                // Rotate character
                playerCharacter.transform.rotation *= rotationYaw;
            }

            // Set
            characterBody.localRotation = localRotation;
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Clamps the pitch of a quaternion according to our clamps.
        /// </summary>
        private Quaternion Clamp(Quaternion rotation)
        {
            rotation.x /= rotation.w;
            rotation.y /= rotation.w;
            rotation.z /= rotation.w;
            rotation.w = 1.0f;

            // Pitch
            float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);

            // Clamp
            pitch = Mathf.Clamp(pitch, yClamp.x, yClamp.y);
            rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

            // Return
            return rotation;
        }

        #endregion
    }
}