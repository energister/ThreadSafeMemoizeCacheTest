using System;
using ThreadSafeMemoizeCacheTest.Implementations;
using ThreadSafeMemoizeCacheTest.TestFramework;

namespace ThreadSafeMemoizeCacheTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfKeys = 300;  // at this number of keys 1st and 3d implementations are around the equal

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: test.exe numberOfKeys");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }
            
            numberOfKeys = int.Parse(args[0]);

            double cumulative1 = 0;
            double cumulative2 = 0;
            double cumulative3 = 0;
            double cumulative4 = 0;

            for (int i = 0; i < 100; i++)
            {
                Console.Write(i+1 + ":\t");

                var test = new Test(numberOfKeys);

                // OldMemoizeTest
                double time = test.RunTest(new ThreadsafeMemoizeCache<int, string>());
                cumulative1 += time;
                Console.Write("100%");

                // NewMemoizeTest
                double time2 = test.RunTest(new ThreadsafeMemoizeCacheNew<int, string>());
                cumulative2 += time2;
                Console.Write(" {0:##}%", GetPercents(time, time2));

                // ConcurrentMemoizeTest
                double time3 = test.RunTest(new ThreadsafeMemoizeCacheconcurrent<int, string>());
                cumulative3 += time3;
                Console.Write(" {0:##}%", GetPercents(time, time3));
                
                double time4 = test.RunTest(new ImmutableDictionaryMemoizator<int, string>());
                cumulative4 += time4;
                Console.Write(" {0:##}%", GetPercents(time, time4));
                
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Cumulative (less value is better):");
            Console.WriteLine("\t100.0%\t{0:##.0}%\t{1:##.0}%\t{2:##.0}%", GetPercents(cumulative1, cumulative2), GetPercents(cumulative1, cumulative3), GetPercents(cumulative1, cumulative4));

            Console.ReadKey();
        }

        private static double GetPercents(double @base, double value)
        {
            return value * 100 / @base;
        }

        private class Test
        {
            private const int NumberOfThreads = 10;
            private const int NumberOfRepetitions = 100;
            
            private readonly int numberOfKeys = 300; 
            private readonly int[][] randomIndexes = new int[NumberOfThreads][];

            public Test(int numberOfKeys)
            {
                this.numberOfKeys = numberOfKeys;
                GenerateRandomKeys();
            }

            private void GenerateRandomKeys()
            {
                var rnd = new Random();

                for (int i = 0; i < NumberOfThreads; i++)
                {
                    var threadIndexes = new int[numberOfKeys];
                    for (int j = 0; j < numberOfKeys; j++)
                    {
                        threadIndexes[j] = rnd.Next(numberOfKeys + 1);
                    }

                    randomIndexes[i] = threadIndexes;
                }
            }

            public double RunTest(IMemoizator<int, string> memoize)
            {
                return MultipleThreadsTester.Run(NumberOfThreads, threadIndex => new ThreadAction(memoize, randomIndexes[threadIndex]));
            }

            private static void TestMethod(IMemoizator<int, string> memoize, int[] randomIndexes)
            {
                for (int i = 0; i < NumberOfRepetitions; i++)
                {
                    foreach (int key in randomIndexes)
                    {
                        memoize.GetOrAdd(key, ValueFactory());
                    }
                }
            }
            
            private static Func<int, string> ValueFactory()
            {
                return (arg) => string.Empty;
            }

            private class ThreadAction : IThreadAction
            {
                private readonly IMemoizator<int, string> memoize;
                private readonly int[] randomIndexes;

                public ThreadAction(IMemoizator<int, string> memoize, int[] randomIndexes)
                {
                    this.memoize = memoize;
                    this.randomIndexes = randomIndexes;
                }

                public void ExecuteTimeCriticalCode()
                {
                    TestMethod(memoize, randomIndexes);
                }
            }
        }
    }
}
