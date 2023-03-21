using System;
using ProjectZ.Code.Runtime.Patterns;
using ProjectZ.Code.Runtime.Patterns.Events;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core
{
    /// <summary>
    /// Installers add dependencies to the ServiceLocator
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class GameSceneInstaller : MonoBehaviour
    {
        [SerializeField] private EventQueueImpl eventQueueImpl;
        
        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<IEventQueue>(eventQueueImpl);
        }

        private void Start()
        {
            bool cursorLocked = true;
            //Update cursor visibility.
            Cursor.visible = !cursorLocked;
            //Update cursor lock state.
            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        // private void OnDestroy()
        // {
        //     ServiceLocator.Instance.DeregisterService<IEventQueue>();
        // }
    }
}