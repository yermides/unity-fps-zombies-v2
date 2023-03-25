using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "WeaponFactoryConfiguration", menuName = "ProjectZ/Weapon/Factory Configuration", order = 0)]
    public class WeaponFactoryConfiguration : ScriptableObject
    {
        [SerializeField] private List<WeaponBehaviour> weapons;
        private Dictionary<WeaponID, WeaponBehaviour> _idToWeapons;

        private void Awake()
        {
            _idToWeapons = new Dictionary<WeaponID, WeaponBehaviour>(weapons.Count);

            foreach (var weaponBehaviour in weapons)
            {
                _idToWeapons.TryAdd(weaponBehaviour.GetWeaponID(), weaponBehaviour);
            }
        }

        public WeaponBehaviour GetWeaponByID(WeaponID id)
        {
            return _idToWeapons[id];
        }
    }
}