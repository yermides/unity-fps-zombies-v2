using System.Collections.Generic;
using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Common.Events;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Map
{
    /// <summary>
    /// Areas of the map, used to detect the player and enable all the objects it holds
    /// </summary>
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class Area : MonoBehaviour
    {
        [SerializeField] private AreaID id;
        [SerializeField] private List<SpawnZone> spawnZones;
        private readonly List<CharacterBehaviour> _characters = new List<CharacterBehaviour>(4); // 4 == maxPlayers
        private BoxCollider[] _boxColliders;
        private bool _isActive;

        private void Reset()
        {
            var rigidbodyComponent = GetComponent<Rigidbody>();
            rigidbodyComponent.useGravity = false;
            
            _boxColliders = GetComponents<BoxCollider>();
            
            foreach (var colliderComponent in _boxColliders)
            {
                colliderComponent.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out CharacterBehaviour character)) return;

            _isActive = true;
            _characters.Add(character);

            // Triggers global event PlayerEnteredArea
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new PlayerEnteredAreaEvent{ AreaID = GetID(), CharacterCount = _characters.Count });
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out CharacterBehaviour character)) return;
            
            _characters.Remove(character);
            
            // Triggers global event PlayerLeftArea
            var eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            eventQueue.Enqueue(new PlayerLeftAreaEvent{ AreaId = GetID(), CharacterCount = _characters.Count });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, .2f);
            
            _boxColliders = GetComponents<BoxCollider>();
            
            foreach (var boxCollider in _boxColliders)
            {
                Gizmos.DrawCube(transform.position + boxCollider.center, boxCollider.size);
            }
            
            Gizmos.color = Color.magenta;
            
            foreach (var spawnZone in spawnZones)
            {
                Gizmos.DrawSphere(spawnZone.transform.position, 1.0f);
            }
        }

        public void Activate() => _isActive = true;

        public bool IsActive() => _isActive;
        public bool IsPlayerInside() => _characters.Count > 0;
        public int GetPlayerCountInside() => _characters.Count;
        public List<SpawnZone> GetSpawnZones() => spawnZones;
        public AreaID GetID() => id;
    }
}