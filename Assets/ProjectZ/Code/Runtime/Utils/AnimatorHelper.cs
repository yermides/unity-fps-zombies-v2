using UnityEngine;

namespace ProjectZ.Code.Runtime.Utils
{
    public static class AnimatorHelper
    {
        // Character Animator
        
        // Hashes
        public static readonly int HashAimingAlpha = Animator.StringToHash("Aiming"); // Aim progress, float
        public static readonly int HashMovement = Animator.StringToHash("Movement");
        public static readonly int HashRunning = Animator.StringToHash(BoolNameRun);
        public static readonly int HashAiming = Animator.StringToHash(BoolNameAim);
        
        // Layer names
        public const string LayerNameHolster = "Layer Holster";
        public const string LayerNameActions = "Layer Actions";
        public const string LayerNameOverlay = "Layer Overlay";
        
        // Parameter names
        public const string BoolNameAim = "Aim"; // Aim request, bool
        public const string BoolNameRun = "Running";
        
        // State names
        public const string StateNameInspect = "Inspect";
        public const string StateNameFire = "Fire";
        public const string StateNameReload = "Reload";
        public const string StateNameReloadEmpty = "Reload Empty";
        public const string StateNameUnholster = "Unholster";
        public const string StateNameFireEmpty = "Fire Empty";
        public const string StateNameHolstered = "Holstered";
        
        // Weapon Animator
        
    }
}