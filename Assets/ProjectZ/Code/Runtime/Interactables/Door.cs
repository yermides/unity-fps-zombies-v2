using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Map;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Interactables
{
    public sealed class Door : InteractableBehaviour
    {
        [SerializeField, Range(750, 1500)] public int cost = 750;
        [SerializeField] private AreaID areaToOpen;
        private bool _hasBeenBought;
        
        public override void DoInteract(CharacterBehaviour character)
        {
            // Check if the player can buy door
            if (character.GetPoints() >= cost && !_hasBeenBought)
            {
                // If he does, trigger an event that the door was open passing the area id
                // and react to it inside the Area class, enabling the zombie spawn

                _hasBeenBought = true;
                character.RemovePoints(cost);
                Destroy(gameObject, 1.0f);
            }
        }
    }
}