using UnityEngine;

namespace ProjectZ.Code.Runtime.Weapons
{
    /// <summary>
    /// Weapon Factory <br/>
    /// Given a configuration, it will search for the id and Instantiate a Weapon prefab
    /// </summary>
    public class WeaponFactory
    {
        private readonly WeaponFactoryConfiguration _configuration;

        public WeaponFactory(WeaponFactoryConfiguration configuration)
        {
            _configuration = configuration;
        }

        public WeaponBehaviour Create(WeaponID id)
        {
            var weaponToCreate = _configuration.GetWeaponById(id);
            return Object.Instantiate(weaponToCreate);
        }
    }
}