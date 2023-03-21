using System;

namespace ProjectZ.Code.Runtime.Patterns.Events
{
    public interface IEventQueue
    {
        void Subscribe<T>(Action<T> callback);
        void Unsubscribe<T>(Action<T> callback);
        void Enqueue<T>(T signal);
    }
}