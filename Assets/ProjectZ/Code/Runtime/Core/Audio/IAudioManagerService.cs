using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    /// <summary>
    /// Using Infima Games audio service interface
    /// </summary>
    public interface IAudioManagerService
    {
        void PlayOneShot(AudioClip clip, AudioSettings settings = default);
        void PlayOneShotDelayed(AudioClip clip, AudioSettings settings = default, float delay = 1.0f);
    }
}