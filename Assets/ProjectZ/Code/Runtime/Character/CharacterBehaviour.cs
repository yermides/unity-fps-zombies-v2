using ProjectZ.Code.Runtime.Common;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        #region UNITY
        
        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}
        protected virtual void OnDestroy(){}

        #endregion
        
        #region GETTERS
        
        public abstract UnityEngine.Camera GetWorldCamera();
        public abstract bool IsRunning();
        public abstract bool IsAiming();
        public abstract Vector2 GetMovementInput();
        public abstract Vector2 GetLookInput();
        public abstract bool IsCursorLocked();
        public abstract Team GetTeam();

        #endregion

        #region FUNCTIONS

        public abstract int GetPoints();
        public abstract void AddPoints(int pts);
        public abstract void RemovePoints(int pts);

        #endregion

    }
}