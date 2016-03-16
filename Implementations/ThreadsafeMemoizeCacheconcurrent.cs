using System;
using System.Collections.Concurrent;

namespace ThreadSafeMemoizeCacheTest.Implementations
{
    /// <summary>
    /// new one (ConcurrentDictionary)
    /// </summary>
    public class ThreadsafeMemoizeCacheconcurrent<TArgument, TResult> : IMemoizator<TArgument, TResult>
    {
        private readonly ConcurrentDictionary<TArgument, TResult> cache = new ConcurrentDictionary<TArgument, TResult>();

        public TResult GetOrAdd(TArgument key, Func<TArgument, TResult> valueFactory)
        {
            return cache.GetOrAdd(key, valueFactory(key));
        }
    }
}