using System.Threading.Tasks;

namespace ProjectZ.Code.Runtime.Utils.Extensions
{
    public static class TaskExtension
    {
        // Unity specific, we need to await the task to know which errors it invoked
        public static async void WrapErrors(this Task task) => await task;
    }
}