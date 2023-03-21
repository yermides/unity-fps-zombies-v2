namespace ProjectZ.Code.Runtime.Patterns.Events
{
    /// <summary>
    /// Simple interface to remind a class it needs to react to a certain event, can be used multiple times
    /// </summary>
    /// <typeparam name="T">The type of the event to react to</typeparam>
    public interface IEventListener<in T>
    {
        void OnEventRaised(T data);
    }
}