using System;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    static class TaskExtensions
    {
        internal static async Task<T> WithTimeout<T>(this Task<T> task, int millisecondsDelay)
        {
            if (await Task.WhenAny(task, Task.Delay(millisecondsDelay)) == task)
            {
                return task.Result;
            }
            else throw new TimeoutException("There was a timeout while awaiting the device response.");
        }
    }
}
