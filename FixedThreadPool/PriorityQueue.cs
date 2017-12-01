using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FixedThreadPool.Contract;
using FixedThreadPool.Expeptions;

namespace FixedThreadPool
{
    /// <summary>
    /// Класс очереди сообщений для исполнения
    /// </summary>
    public class PriorityQueue :
        IPriorityQueue<ITask>
    {
        private readonly int _highPriorityBarriers;
        private readonly Dictionary<Priority, ConcurrentQueue<ITask>> _queues;

        private int _currentBarriers;

        public PriorityQueue(
            int highPriorityBarriers)
        {
            _highPriorityBarriers = highPriorityBarriers;
            _queues = new Dictionary<Priority, ConcurrentQueue<ITask>>
            {
                {Priority.Low, new ConcurrentQueue<ITask>()},
                {Priority.Normal, new ConcurrentQueue<ITask>()},
                {Priority.High, new ConcurrentQueue<ITask>()}
            };
        }

        public bool TryAdd(KeyValuePair<Priority, ITask> item)
        {
            if (!_queues.ContainsKey(item.Key))
                throw new UnsupportedPriorityException("bla bla bla");

            _queues[item.Key].Enqueue(item.Value);

            return true;
        }

        public bool TryTake(out KeyValuePair<Priority, ITask> item)
        {
            var result = TryTake(out ITask task);
            item = new KeyValuePair<Priority, ITask>(result.Priority, task);
            return result.Result;
        }

        #region NotImplemented
        public int Count { get; }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }

        public IEnumerator<KeyValuePair<Priority, ITask>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Priority, ITask>[] array, int index)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<Priority, ITask>[] ToArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        private (bool Result, Priority Priority) TryTake(out ITask result)
        {
            lock (_queues)
            {
                if (_currentBarriers >= _highPriorityBarriers && _queues[Priority.Normal].TryDequeue(out result))
                {
                    _currentBarriers = 0;
                    return (true, Priority.Normal);
                }
                if (_queues[Priority.High].TryDequeue(out result))
                {
                    _currentBarriers++;
                    return (true, Priority.High);
                }
                if (_queues[Priority.Normal].TryDequeue(out result))
                {
                    _currentBarriers = 0;
                    return (true, Priority.Normal);
                }
                return (_queues[Priority.Low].TryDequeue(out result), Priority.Low);
            }
        }
    }
}