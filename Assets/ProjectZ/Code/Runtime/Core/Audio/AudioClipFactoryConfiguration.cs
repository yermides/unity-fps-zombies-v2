using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    [CreateAssetMenu(fileName = "AudioClipFactoryConfiguration", menuName = "ProjectZ/Audio/AudioClipFactoryConfiguration", order = 0)]
    public class AudioClipFactoryConfiguration : ScriptableObject
    {
        [SerializeField] private List<AudioClipConfiguration> audioClipToIDConfigurations;
        private Dictionary<AudioClipID, AudioClip> _idToClip;
        
        private void Awake()
        {
            if (audioClipToIDConfigurations == null) return; // Prevent Unity Editor Behaviour
            
            _idToClip = new Dictionary<AudioClipID, AudioClip>(audioClipToIDConfigurations.Count);

            foreach (var clipProxy in audioClipToIDConfigurations)
            {
                _idToClip.TryAdd(clipProxy.GetClipID(), clipProxy.GetAudioClip());
            }
        }

        public AudioClip GetClipByID(AudioClipID audioClipID) => _idToClip[audioClipID];

#if UNITY_EDITOR
        [Button]
        public void GetAllClipConfigurations()
        {
            // Search for all AudioClipConfiguration so we don't have to manually assign them
            audioClipToIDConfigurations = AssetDatabase.FindAssets($"t: AudioClipConfiguration").ToList()
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AudioClipConfiguration>)
                .ToList();
        }
#endif
    }
}