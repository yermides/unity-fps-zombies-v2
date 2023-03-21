using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Patterns.Events
{
    public class EventQueueImpl : MonoBehaviour, IEventQueue
    {
        private Dictionary<Type, dynamic> _eventsToDelegatesDictionary;
        private Queue<dynamic> _eventsQueued;

        #region INTERFACE
        
        public void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            
            if (!_eventsToDelegatesDictionary.ContainsKey(type)) _eventsToDelegatesDictionary.Add(type, null);

            _eventsToDelegatesDictionary[type] += callback;
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);

            if (_eventsToDelegatesDictionary.ContainsKey(type))
            {
                _eventsToDelegatesDictionary[type] -= callback;
            }
        }

        public void Enqueue<T>(T signal)
        {
            _eventsQueued.Enqueue(signal);
        }
        
        #endregion

        #region UNITY
        
        public void Awake()
        {
            _eventsToDelegatesDictionary = new Dictionary<Type, dynamic>();
            _eventsQueued = new Queue<dynamic>();
        }

        private void LateUpdate()
        {
            while (_eventsQueued.Count > 0)
            {
                ProcessOne();
            }
        }

        #endregion

        #region FUNCTIONS

        private void ProcessOne()
        {
            var queuedEvent = _eventsQueued.Dequeue();

            Type type = queuedEvent.GetType();
            
            if (!_eventsToDelegatesDictionary.ContainsKey(type)) return;

            var callback = _eventsToDelegatesDictionary[type];
            callback?.Invoke(queuedEvent);
        }

        #endregion
    }
}