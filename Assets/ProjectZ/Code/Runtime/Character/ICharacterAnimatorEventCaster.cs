using System;

namespace ProjectZ.Code.Runtime.Character
{
    public interface ICharacterAnimatorEventCaster
    {
        public event Action EjectCasingEvent;
        public event Action<int> AmmunitionFillEvent; // int = amount
        public event Action<int> SetActiveKnifeEvent; // int = active knife
        public event Action GrenadeEvent;
        public event Action<int> SetActiveMagazineEvent; // int = active
        public event Action AnimationEndedBoltEvent;
        public event Action AnimationEndedReloadEvent;
        public event Action AnimationEndedGrenadeThrowEvent;
        public event Action AnimationEndedMeleeEvent;
        public event Action AnimationEndedInspectEvent;
        public event Action AnimationEndedHolsterEvent;
        public event Action<int> SlideBackEvent; // int = back
    }
    
    // public interface ICharacterAnimatorEventReceiver
    // {
    // 	void OnEjectCasing();
    // 	void OnAmmunitionFill(int amount = 0);
    // 	void OnSetActiveKnife(int active);
    // 	void OnGrenade();
    // 	void OnSetActiveMagazine(int active);
    // 	void OnAnimationEndedBolt();
    // 	void OnAnimationEndedReload();
    // 	void OnAnimationEndedGrenadeThrow();
    // 	void OnAnimationEndedMelee();
    // 	void OnAnimationEndedInspect();
    // 	void OnAnimationEndedHolster();
    // 	void OnSlideBack(int back);
    // }
}