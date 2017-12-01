using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using FixedThreadPool.Contract;

namespace FixedThreadPool
{
    public class FixedThreadPool
    {
        private readonly object _lockSyncCancelling = new object();
        private readonly BlockingCollection<KeyValuePair<Priority, ITask>> _priorityQueue;
        private readonly Thread[] _threads;
        private volatile bool _isCancelling;

        public FixedThreadPool(
            int maxThreads)
        {
            _priorityQueue =
                new BlockingCollection<KeyValuePair<Priority, ITask>>(new PriorityQueue(3));
            _threads = new Thread[maxThreads];

            for (var i = 0; i < maxThreads; i++)
            {
                _threads[i] = new Thread(Process);
                _threads[i].Start();
            }
        }

        public bool Execute(ITask task, Priority priority)
        {
            if (_isCancelling)
                return false;

            lock (_lockSyncCancelling)
            {
                if (!_isCancelling)
                {
                    _priorityQueue.TryAdd(new KeyValuePair<Priority, ITask>(priority, task));
                    return true;
                }
                return false;
            }
        }

        private void Stop()
        {
            if (!_isCancelling)
                lock (_lockSyncCancelling)
                {
                    if (!_isCancelling)
                    {
                        _isCancelling = true;
                        _priorityQueue.CompleteAdding();

                        foreach (var thread in _threads)
                            thread.Join();
                    }
                }
        }

        private void Process()
        {
            while (!_priorityQueue.IsCompleted)
                try
                {
                    var result = _priorityQueue.Take();
                    result.Value.Execute();
                }
                catch (Exception)
                {
                    //....
                }
        }
    }
}