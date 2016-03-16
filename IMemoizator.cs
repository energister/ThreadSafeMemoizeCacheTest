using System;

namespace ThreadSafeMemoizeCacheTest
{
    interface IMemoizator<TArgument, TResult>
    {
        TResult GetOrAdd(TArgument key, Func<TArgument, TResult> valueFactory);
    }
}