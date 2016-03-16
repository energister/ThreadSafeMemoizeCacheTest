namespace ThreadSafeMemoizeCacheTest.TestFramework
{
    /// <summary>
    /// Instance which <see cref="ExecuteTimeCriticalCode"/> method will be executed in dedicated thread during test
    /// </summary>
    public interface IThreadAction
    {
        /// <summary>
        /// Shold executed code which performance will be tested
        /// and nothing more
        /// </summary>
        void ExecuteTimeCriticalCode();
    }
}
