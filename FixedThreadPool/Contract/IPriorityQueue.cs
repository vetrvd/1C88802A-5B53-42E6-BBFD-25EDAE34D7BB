using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FixedThreadPool.Contract
{
    public interface IPriorityQueue<T> : IProducerConsumerCollection<KeyValuePair<Priority, T>>
    {
    }
}