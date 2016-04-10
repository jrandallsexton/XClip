
using System;
using System.Diagnostics;

namespace XClip.Tests
{
    internal class TimeKeeper
    {
        public static TimeSpan Measure(Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            action();
            return watch.Elapsed;
        }
    }
}