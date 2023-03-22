using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    [CreateAssetMenu(fileName = "AudioClipFactoryConfiguration", menuName = "ProjectZ/Audio/AudioClipFactoryConfiguration", order = 0)]
    public class AudioClipFactoryConfiguration : ScriptableObject
    {
        [SerializeField] private List<AudioClipConfiguration> audioClipProxies;
        private Dictionary<AudioClipID, AudioClip> _idToClip;
        
        private void Awake()
        {
            if (audioClipProxies == null) return; // Prevent Unity Editor Behaviour
            
            _idToClip = new Dictionary<AudioClipID, AudioClip>(audioClipProxies.Count);

            foreach (var clipProxy in audioClipProxies)
            {
                _idToClip.TryAdd(clipProxy.GetClipID(), clipProxy.GetAudioClip());
            }
        }

        public AudioClip GetClipByID(AudioClipID audioClipID) => _idToClip[audioClipID];
    }
}