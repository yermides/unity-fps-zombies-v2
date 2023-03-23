using System.Threading.Tasks;

namespace ProjectZ.Code.Runtime.Patterns.Commands
{
    /// <summary>
    /// Commands are an encapsulation of an action into a class
    /// We use Tasks because we could have commands that go on for more than one frame
    /// </summary>
    public interface ICommand
    {
        Task Execute();
    }
}