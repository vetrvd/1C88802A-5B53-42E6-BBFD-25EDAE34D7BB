using System;
using System.Security.Authentication.ExtendedProtection;
using Xunit;

namespace FixedThreadPool.Tests
{
    public class FixedThreadPoolTest
    {
        [Fact]
        public void FixedThreadPool_Execute_TestRun()
        {
            var pool = new FixedThreadPool(2);
            for (int i = 0; i < 100; i++)
            {
                pool.Execute(new Task(), (Priority)new Random().Next(3));
            }
        }
    }
}