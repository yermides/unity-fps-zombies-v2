using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public abstract class MovementBehaviour : MonoBehaviour
    {
        #region UNITY
        
        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}
        
        #endregion

        #region GETTERS

        public abstract bool IsGrounded();

        #endregion

        #region FUNCTIONS

        public abstract void Jump();

        #endregion
    }
}