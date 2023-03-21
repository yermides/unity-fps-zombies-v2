using System;

namespace ProjectZ.Code.Runtime.Character
{
	/// <summary>
	/// Receives the notification of events directly from Animation Events inside Animation Clips
	/// and propagates them through events of its own, much like what was done with the CharacterInputReceiver.
	/// It acts as a mediator between Animation Events triggered by the animator and the Character
	/// </summary>
	public class CharacterAnimatorEventReceiver
		: CharacterAnimatorEventReceiverBehaviour
		, ICharacterAnimatorEventCaster
    {
	    #region EVENTS
	    
	    public event Action EjectCasingEvent;
	    public event Action<int> AmmunitionFillEvent;
	    public event Action<int> SetActiveKnifeEvent;
	    public event Action GrenadeEvent;
	    public event Action<int> SetActiveMagazineEvent;
	    public event Action AnimationEndedBoltEvent;
	    public event Action AnimationEndedReloadEvent;
	    public event Action AnimationEndedGrenadeThrowEvent;
	    public event Action AnimationEndedMeleeEvent;
	    public event Action AnimationEndedInspectEvent;
	    public event Action AnimationEndedHolsterEvent;
	    public event Action<int> SlideBackEvent;
	    
	    #endregion
	    
        #region ANIMATION

        protected override void OnEjectCasing() => EjectCasingEvent?.Invoke();
        protected override void OnAmmunitionFill(int amount = 0) => AmmunitionFillEvent?.Invoke(amount);
        protected override void OnSetActiveKnife(int active) => SetActiveKnifeEvent?.Invoke(active);
        protected override void OnGrenade() => GrenadeEvent?.Invoke();
        protected override void OnSetActiveMagazine(int active) => SetActiveMagazineEvent?.Invoke(active);
        protected override void OnAnimationEndedBolt() => AnimationEndedBoltEvent?.Invoke();
        protected override void OnAnimationEndedReload() => AnimationEndedReloadEvent?.Invoke();
        protected override void OnAnimationEndedGrenadeThrow() => AnimationEndedGrenadeThrowEvent?.Invoke();
        protected override void OnAnimationEndedMelee() => AnimationEndedMeleeEvent?.Invoke();
        protected override void OnAnimationEndedInspect() => AnimationEndedInspectEvent?.Invoke();
        protected override void OnAnimationEndedHolster() => AnimationEndedHolsterEvent?.Invoke();
        protected override void OnSlideBack(int back) => SlideBackEvent?.Invoke(back);
	        
		#endregion
    }
}