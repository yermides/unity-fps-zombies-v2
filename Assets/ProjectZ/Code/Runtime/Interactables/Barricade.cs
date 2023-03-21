using System;
using ProjectZ.Code.Runtime.Character;
using Unity.AI.Navigation;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Interactables
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Barricade : InteractableBehaviour
    {
        [SerializeField] private int woodenPlankInitialCount = 6;
        [SerializeField] private NavMeshLink navMeshLink;
        [SerializeField] private Transform zombieStandingLocation;
        private int _woodenPlankCurrentCount;

        #region UNITY
        
        private void Reset()
        {
            navMeshLink = GetComponentInChildren<NavMeshLink>();
        }

        private void Awake()
        {
            _woodenPlankCurrentCount = woodenPlankInitialCount;
        }

        private void Start()
        {
            navMeshLink.enabled = !HasPlanks();
        }

        private void OnDrawGizmosSelected()
        {
            if (!zombieStandingLocation) return;
            
            var position = zombieStandingLocation.position;
            
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the attack
            Gizmos.DrawSphere(position, 0.3f);
        }
        
        #endregion

        public bool HasPlanks() => _woodenPlankCurrentCount > 0;

        public override void DoInteract(CharacterBehaviour character)
        {
            // TODO: add points and activate
            throw new System.NotImplementedException();
            AddPlank();
        }

        private void AddPlank()
        {
        }

        private void RemovePlank()
        {
        }
    }
}