using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    /// <summary>
    /// Given a configuration, it will search for the ID and Instantiate a Weapon prefab
    /// </summary>
    public class WeaponFactory
    {
        private readonly WeaponFactoryConfiguration _configuration;
        public WeaponFactory(WeaponFactoryConfiguration configuration) => _configuration = configuration;
        public WeaponBehaviour Create(WeaponID id) => Object.Instantiate(_configuration.GetWeaponByID(id));
    }
}