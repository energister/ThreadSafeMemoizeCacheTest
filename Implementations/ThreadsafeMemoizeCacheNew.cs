using System;
using System.Collections.Generic;

namespace ThreadSafeMemoizeCacheTest.Implementations
{
    /// <summary>
    /// Dictionary without copying (global lock)
    /// </summary>
    public class ThreadsafeMemoizeCacheNew<TArgument, TResult> : IMemoizator<TArgument, TResult>
    {
        private readonly Dictionary<TArgument, TResult> cache = new Dictionary<TArgument, TResult>();

        public TResult GetOrAdd(TArgument key, Func<TArgument, TResult> valueFactory)
        {
            TResult result;

            lock (cache)
            {
                if (cache.TryGetValue(key, out result))
                {
                    return result;
                }

                result = valueFactory(key);
                cache.Add(key, result);
            }
            return result;
        }
    }
}