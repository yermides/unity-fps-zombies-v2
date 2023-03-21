using ProjectZ.Code.Runtime.Weapons;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public abstract class InventoryBehaviour : MonoBehaviour
    {
        public abstract void Init(int weaponToEquipIndex = 0);
        public abstract int GetPreviousIndex();
        public abstract int GetNextIndex();
        public abstract int GetEquippedIndex();
        public abstract WeaponBehaviour Equip(int index);
        public abstract WeaponBehaviour GetWeaponEquipped();
        public abstract void Add(WeaponBehaviour weaponBehaviour);
    }
}