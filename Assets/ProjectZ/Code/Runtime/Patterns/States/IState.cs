namespace ProjectZ.Code.Runtime.Patterns.States
{
    public interface IState
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}