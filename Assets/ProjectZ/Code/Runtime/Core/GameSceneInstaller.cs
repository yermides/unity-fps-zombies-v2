using System;
using ProjectZ.Code.Runtime.Common.Commands;
using ProjectZ.Code.Runtime.Core.Audio;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;
using AudioSettings = ProjectZ.Code.Runtime.Core.Audio.AudioSettings;

namespace ProjectZ.Code.Runtime.Core
{
    /// <summary>
    /// Installers add dependencies to the ServiceLocator
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class GameSceneInstaller : MonoBehaviour
    {
        [SerializeField] private EventQueueImpl eventQueueImpl;
        [SerializeField] private AudioManagerService audioManager;
        [SerializeField] private AudioClipFactoryConfiguration audioClipFactoryConfiguration;
        [SerializeField] private AudioClipID clipToPlayAtStart;
        
        private void Awake()
        {
            var locator = ServiceLocator.Instance;
            
            // Register Event Queue
            DontDestroyOnLoad(eventQueueImpl);
            locator.RegisterService<IEventQueue>(eventQueueImpl);
            
            // Register Audio Manager
            DontDestroyOnLoad(audioManager);
            locator.RegisterService<IAudioManagerService>(audioManager);
            
            // Register Audio Clip Factory
            var audioClipFactory = new AudioClipFactory(Instantiate(audioClipFactoryConfiguration));
            locator.RegisterService(audioClipFactory);
        }

        private void Start()
        {
            // Cursor 
            bool cursorLocked = true;
            // Update cursor visibility
            Cursor.visible = !cursorLocked;
            // Update cursor lock state
            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;

            new PlayOneShotClipCommand(clipToPlayAtStart, new AudioSettings(1.0f, 0.0f, true)).Execute();
        }

        private void OnDestroy()
        {            
            var locator = ServiceLocator.Instance;
            locator.DeregisterService<AudioClipFactory>();
        }
    }
}