using System;
using NaughtyAttributes;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public class CharacterInstaller : MonoBehaviour
    {
        [Header("Target"), HorizontalLine]
        [SerializeField] private Character character;
        
        [Header("Dependencies"), HorizontalLine]
        [SerializeField] private CharacterAnimatorEventReceiver animatorEventReceiver;
        [SerializeField] private CharacterInputEventReceiver inputEventReceiver;
        
        private void Reset()
        {
            character = GetComponent<Character>();
            animatorEventReceiver = GetComponentInChildren<CharacterAnimatorEventReceiver>();
            inputEventReceiver = GetComponent<CharacterInputEventReceiver>();
        }

        private void Awake()
        {
            var args = new CharacterArgs
            {
                characterAnimatorEvents = GetAnimatorEventReceiver(),
                characterInputEvents = GetInputReader(),
            };
            
            character.Configure(args);
        }

        private ICharacterInputEvents GetInputReader() => inputEventReceiver;
        private ICharacterAnimatorEvents GetAnimatorEventReceiver() => animatorEventReceiver;
    }
}