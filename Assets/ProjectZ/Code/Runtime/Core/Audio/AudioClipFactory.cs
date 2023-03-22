using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    /// <summary>
    /// More of a Dictionary searcher than a Factory
    /// </summary>
    public class AudioClipFactory
    {
        private readonly AudioClipFactoryConfiguration _configuration;
        public AudioClipFactory(AudioClipFactoryConfiguration configuration) => _configuration = configuration;
        public AudioClip Get(AudioClipID id) => _configuration.GetClipByID(id);
    }
}