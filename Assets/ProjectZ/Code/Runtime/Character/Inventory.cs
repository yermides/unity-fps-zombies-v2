using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Weapons;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public class Inventory : InventoryBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField] private Transform weaponSocket;
        
        [MinValue(2), MaxValue(3)]
        [SerializeField] private int capacity = 2;
        
        #endregion
        
        #region FIELDS
        
        private List<WeaponBehaviour> _weapons;
        private WeaponBehaviour _equipped;
        private int _equippedIndex = -1;
        
        #endregion

        public override void Init(int index = 0)
        {
            var weaponsToAdd = weaponSocket.GetComponentsInChildren<WeaponBehaviour>(true);
            _weapons = new List<WeaponBehaviour>(weaponsToAdd);
            
            // Disable all weapons
            foreach (var weaponBehaviour in _weapons)
            {
                weaponBehaviour.gameObject.SetActive(false);
            }
            
            Equip(index);
        }

        public override int GetPreviousIndex()
        {
            // Get last index with wrap around
            int newIndex = _equippedIndex - 1;
            
            if (newIndex < 0)
            {
                newIndex = _weapons.Count - 1;
            }

            return newIndex;
        }

        public override int GetNextIndex()
        {
            // Get next index with wrap around
            int newIndex = _equippedIndex + 1;

            if (newIndex > _weapons.Count - 1)
            {
                newIndex = 0;
            }

            return newIndex;
        }

        public override int GetEquippedIndex() => _equippedIndex;

        public override WeaponBehaviour Equip(int index)
        {
            // If we have no weapons, we can't really equip anything
            if (_weapons == null)
            {
                return _equipped;
            }
            
            // The index needs to be within the array's bounds
            if (index > _weapons.Count - 1)
            {
                return _equipped;
            }

            // No point in allowing equipping the already-equipped weapon
            if (_equippedIndex == index)
            {
                return _equipped;
            }
            
            // Disable the currently equipped weapon, if we have one
            if (_equipped != null)
            {
                _equipped.gameObject.SetActive(false);
            }

            // Update index
            _equippedIndex = index;
            
            // Update equipped
            _equipped = _weapons[_equippedIndex];
            
            // Activate the newly-equipped weapon
            _equipped.gameObject.SetActive(true);

            return _equipped;
        }

        public override WeaponBehaviour GetWeaponEquipped() => _equipped;
        
        public override void Add(WeaponBehaviour weaponBehaviour)
        {
            // TODO: add if capacity < 2~3 and not in inventory with search based on ID
            // and if found, refill instead of adding
            throw new NotImplementedException();
        }
    }
}