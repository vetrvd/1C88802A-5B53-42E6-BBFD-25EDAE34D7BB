using System.Threading;
using FixedThreadPool.Contract;

namespace FixedThreadPool
{
    public class Task : ITask
    {
        public void Execute()
        {
            Thread.Sleep(5000);
        }
    }
}