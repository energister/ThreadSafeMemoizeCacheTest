using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSafeMemoizeCacheTest.TestFramework
{
    /// <summary>
    /// Runs some code simultaniously in multiple threads and precisely calculates execution time
    /// </summary>
    public static class MultipleThreadsTester
    {
        /// <summary>
        /// Should return <see cref="IThreadAction"/> wich should be prepared to be executed during the test.
        /// The code will be executed by the thread with the <paramref name="threadIndex"/> (starting from 0)
        /// </summary>
        public delegate IThreadAction ThreadActionFactory(int threadIndex);

        /// <summary>
        /// Returns <see cref="Stopwatch.ElapsedTicks"/> for the code under test
        /// </summary>
        public static double Run(int numberOfThreads, ThreadActionFactory testThreadActionFactory)
        {
            /* prepare */
            var start = new ManualResetEvent(false);
            var end = new ManualResetEvent(false);
            var stopwatch = new Stopwatch();

            var listTasks = new Task[numberOfThreads];
            int counter = 0;
            for (int i = 0; i < listTasks.Length; i++)
            {
                int i1 = i;
                listTasks[i] = new Task(() =>
                {
                    IThreadAction action = testThreadActionFactory(i1);
                    ThreadTestCode(start, action, () =>
                    {
                        // on every thread start
                        int started = Interlocked.Increment(ref counter);
                        if (started == numberOfThreads)
                        {
                            stopwatch.Start();
                            start.Set();
                        }
                    }, () =>
                    {
                        // on every thread finish
                        int value = Interlocked.Decrement(ref counter);
                        if (value == 0)
                        {
                            stopwatch.Stop();
                            end.Set();
                        }
                    });
                }, TaskCreationOptions.LongRunning);
                listTasks[i].Start();
            }

            end.WaitOne();

            return stopwatch.ElapsedTicks;
        }

        private static void ThreadTestCode(ManualResetEvent start, IThreadAction threadAction, Action onStart, Action onEnd)
        {
            onStart();

            start.WaitOne();

            threadAction.ExecuteTimeCriticalCode();

            onEnd();
        }
    }
}