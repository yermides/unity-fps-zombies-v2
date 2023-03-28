using System;
using NaughtyAttributes;
using ProjectZ.Code.Runtime.Character;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Zombie
{
    public class ZombieInstaller : MonoBehaviour
    {
        [Header("Target"), HorizontalLine]
        [SerializeField] private Zombie zombie;
        
        [Header("Dependencies"), HorizontalLine]
        [SerializeField] private ZombieAnimatorEventReceiver animatorEventReceiver;

        private void Awake()
        {
            ZombieArgs args = new ZombieArgs { zombieAnimatorEvents = animatorEventReceiver };
            zombie.Configure(args);
        }
    }
}