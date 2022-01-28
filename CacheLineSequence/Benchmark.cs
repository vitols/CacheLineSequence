using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Consume
{
    [ShortRunJob]
    [NativeMemoryProfiler]
    [MemoryDiagnoser]
    public class Benchmark
    {
        public IEnumerable<long[]> Data()
        {
            return new long[][]
            {
                Enumerable.Range(0, 64).Select(l => (long)l).ToArray(),
                Enumerable.Range(0, 1024).Select(l => (long)l).ToArray(),
                Enumerable.Range(0, 4096).Select(l => (long)l).ToArray(),
                Enumerable.Range(0, 4096*4).Select(l => (long)l).ToArray()
            };
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void Allocation(long[] array)
        {
            using (var clSeq = new CacheLineSequence<long>(array))
            {
                //Console.WriteLine(clSeq.Length);
            }
        }
    }
}
