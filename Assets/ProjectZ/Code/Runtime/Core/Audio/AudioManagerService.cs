using System.Collections;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    /// <summary>
    /// Using Infima Games Audio Manager Service
    /// Manages the spawning and playing of sounds
    /// </summary>
    public class AudioManagerService : MonoBehaviour, IAudioManagerService
    {
        /// <summary>
        /// Contains data related to playing a OneShot audio
        /// </summary>
        private readonly struct OneShotCoroutine
        {
            public AudioClip Clip { get; }
            public AudioSettings Settings { get; }
            public float Delay { get; }
            
            public OneShotCoroutine(AudioClip clip, AudioSettings settings, float delay)
            {
                Clip = clip;
                Settings = settings;
                Delay = delay;
            }
        }

        /// <summary>
        /// Destroys the audio source once it has finished playing
        /// </summary>
        private IEnumerator DestroySourceWhenFinished(AudioSource source)
        {
            // Wait for the audio source to complete playing the clip
            yield return new WaitWhile(() => source.isPlaying);
            
            // Destroy the audio game object, since we're not using it anymore
            // This isn't really too great for performance, but it works, for now
            DestroyImmediate(source.gameObject);
        }

        /// <summary>
        /// Waits for a certain amount of time before starting to play a one shot sound
        /// </summary>
        private IEnumerator PlayOneShotAfterDelay(OneShotCoroutine value)
        {
            // Wait for the delay
            yield return new WaitForSeconds(value.Delay);
            // Play
            PlayOneShot_Internal(value.Clip, value.Settings);
        }
        
        /// <summary>
        /// Internal PlayOneShot. Basically does the whole function's name!
        /// </summary>
        private void PlayOneShot_Internal(AudioClip clip, AudioSettings settings)
        {
            // No need to do absolutely anything if the clip is null.
            if (clip == null) return;
            
            // Spawn a game object for the audio source
            var newSourceObject = new GameObject($"Audio Source -> {clip.name}");
            // Add an audio source component to that object
            var newAudioSource = newSourceObject.AddComponent<AudioSource>();

            // Set volume
            newAudioSource.volume = settings.Volume;
            // Set spatial blend
            newAudioSource.spatialBlend = settings.SpatialBlend;
            
            // Play the clip!
            newAudioSource.PlayOneShot(clip);
            
            //Start a coroutine that will destroy the whole object once it is done!
            if (settings.AutomaticCleanup)
            {
                StartCoroutine(nameof(DestroySourceWhenFinished), newAudioSource);
            }
        }

        #region Audio Manager Service Interface

        public void PlayOneShot(AudioClip clip, AudioSettings settings = default)
        {
            // Play
            PlayOneShot_Internal(clip, settings);
        }

        public void PlayOneShotDelayed(AudioClip clip, AudioSettings settings = default, float delay = 1.0f)
        {
            // Play
            StartCoroutine(nameof(PlayOneShotAfterDelay), new OneShotCoroutine(clip, settings, delay));
        }

        #endregion
    }
}