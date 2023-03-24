using NaughtyAttributes;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    /// <summary>
    /// Establishes a relation between an AudioClipID and an AudioClip.
    /// This makes it so we don't cry if the AudioClip gets deleted
    /// or we want to replace it across the board we just change the reference this object holds
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipConfiguration", menuName = "ProjectZ/Audio/AudioClipConfiguration",
        order = 0)]
    public class AudioClipConfiguration : ScriptableObject
    {
        [SerializeField] private AudioClipID audioClipID;
        [SerializeField] private AudioClip audioClip;

        public AudioClipID GetClipID() => audioClipID;
        public AudioClip GetAudioClip() => audioClip;

#if UNITY_EDITOR
        public void SetClipID(AudioClipID id) => audioClipID = id;
        public void SetAudioClip(AudioClip clip) => audioClip = clip;
#endif
    }
}