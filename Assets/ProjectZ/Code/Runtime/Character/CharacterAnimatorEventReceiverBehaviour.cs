using UnityEngine;

namespace ProjectZ.Code.Runtime.Character
{
    public abstract class CharacterAnimatorEventReceiverBehaviour : MonoBehaviour
    {
        protected abstract void OnEjectCasing();
        protected abstract void OnAmmunitionFill(int amount = 0);
        protected abstract void OnSetActiveKnife(int active);
        protected abstract void OnGrenade();
        protected abstract void OnSetActiveMagazine(int active);
        protected abstract void OnAnimationEndedBolt();
        protected abstract void OnAnimationEndedReload();
        protected abstract void OnAnimationEndedGrenadeThrow();
        protected abstract void OnAnimationEndedMelee();
        protected abstract void OnAnimationEndedInspect();
        protected abstract void OnAnimationEndedHolster();
        protected abstract void OnSlideBack(int back);
    }
}