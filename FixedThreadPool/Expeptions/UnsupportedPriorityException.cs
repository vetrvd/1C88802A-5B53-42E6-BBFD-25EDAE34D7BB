using System;

namespace FixedThreadPool.Expeptions
{
    public class UnsupportedPriorityException : Exception
    {
        public UnsupportedPriorityException(string message, Exception ex = null) : base(message, ex)
        {
        }
    }
}