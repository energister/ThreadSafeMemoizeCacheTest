using System;
using System.Collections.Generic;

namespace ThreadSafeMemoizeCacheTest.Implementations
{
    /// <summary>
    /// Old one (with Dictionary copying)
    /// </summary>
    public class ThreadsafeMemoizeCache<TArgument, TResult> : IMemoizator<TArgument, TResult>
    {
        private Dictionary<TArgument, TResult> cache = new Dictionary<TArgument, TResult>();

        public TResult GetOrAdd(TArgument key, Func<TArgument, TResult> valueFactory)
        {
            TResult result;

            if (cache.TryGetValue(key, out result))
            {
                return result;
            }
            lock (this)
            {
                if (!cache.TryGetValue(key, out result))
                {
                    result = valueFactory(key);
                    Dictionary<TArgument, TResult> newCache = new Dictionary<TArgument, TResult>(cache);
                    newCache.Add(key, result);
                    cache = newCache;
                }
            }
            return result;
        }
    }
}