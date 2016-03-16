using System;
using System.Collections.Immutable;

namespace ThreadSafeMemoizeCacheTest.Implementations
{
    /// <summary>
    /// Based on ImmutableDictionary
    /// </summary>
    public class ImmutableDictionaryMemoizator<TArgument, TResult> : IMemoizator<TArgument, TResult>
    {
        private ImmutableDictionary<TArgument, TResult> cache = ImmutableDictionary.CreateBuilder<TArgument, TResult>().ToImmutable();

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
                    var builder = ImmutableDictionary.CreateBuilder<TArgument, TResult>();
                    builder.AddRange(cache);
                    builder.Add(key, result);
                    cache = builder.ToImmutable();
                }
            }
            return result;
        }
    }
}