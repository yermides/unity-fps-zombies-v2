using UnityEngine;
using ProjectZ.Code.Runtime.Character;
using ProjectZ.Code.Runtime.Common.Commands;
using ProjectZ.Code.Runtime.Core.Audio;
using ProjectZ.Code.Runtime.Weapons;
using AudioSettings = ProjectZ.Code.Runtime.Core.Audio.AudioSettings;

namespace ProjectZ.Code.Runtime.Animation
{
    /// <summary>
    /// This class helps us to get runtime clips that can change depending on the weapon
    /// </summary>
    public class PlaySoundCharacterBehaviour : StateMachineBehaviour
    {
        private enum SoundType { Holster, Unholster, Reload, ReloadEmpty, Fire, FireEmpty, }
        
        private CharacterBehaviour _characterBehaviour;
        [SerializeField] private SoundType soundType;
        [SerializeField] private AudioSettings audioSettings;
        [SerializeField] private float delay;

        public void Configure(CharacterBehaviour characterBehaviour) =>_characterBehaviour = characterBehaviour;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            new PlayOneShotClipCommand(GetClip(), audioSettings, delay).Execute();
        }

        private AudioClipID GetClip()
        {
            WeaponBehaviour weaponBehaviour = _characterBehaviour.GetEquippedWeapon();
            
            AudioClipID clip = soundType switch
            {
                SoundType.Holster => weaponBehaviour.GetAudioClipHolster(),
                SoundType.Unholster => weaponBehaviour.GetAudioClipUnholster(),
                SoundType.Reload => weaponBehaviour.GetAudioClipReload(),
                SoundType.ReloadEmpty => weaponBehaviour.GetAudioClipReloadEmpty(),
                SoundType.Fire => weaponBehaviour.GetAudioClipFire(),
                SoundType.FireEmpty => weaponBehaviour.GetAudioClipFireEmpty(),
                _ => default
            };

            return clip;
        }
    }
}