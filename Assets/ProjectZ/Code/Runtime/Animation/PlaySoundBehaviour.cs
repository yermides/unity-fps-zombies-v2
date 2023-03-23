using ProjectZ.Code.Runtime.Common.Commands;
using ProjectZ.Code.Runtime.Core.Audio;
using UnityEngine;
using AudioSettings = ProjectZ.Code.Runtime.Core.Audio.AudioSettings;

namespace ProjectZ.Code.Runtime.Animation
{
    /// <summary>
    /// Plays an AudioClip when enters a state
    /// </summary>
    public class PlaySoundBehaviour : StateMachineBehaviour
    {
        [SerializeField] private AudioClipID clipToPlayID;
        [SerializeField] private AudioSettings audioSettings;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            new PlayOneShotClipCommand(clipToPlayID, audioSettings).Execute();
        }
    }
}