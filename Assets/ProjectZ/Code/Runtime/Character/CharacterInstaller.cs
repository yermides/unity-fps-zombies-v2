using System;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public class CharacterInstaller : MonoBehaviour
    {
        [SerializeField] private Character character;
        [SerializeField] private CharacterAnimatorEventReceiver animatorEventReceiver;
        
        private void Reset()
        {
            character = GetComponent<Character>();
            animatorEventReceiver = GetComponentInChildren<CharacterAnimatorEventReceiver>();
        }

        private void Awake()
        {
            character.Configure(animatorEventReceiver);
        }
    }
}