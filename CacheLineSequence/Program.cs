using BenchmarkDotNet.Running;
using CacheLineSequence;

using var clseq = new CacheLineSequence<long>(
    101, 102, 103, 104, 105, 106, 107, 108,
    201, 202, 203, 204, 205, 206, 207, 208
    );

Console.ReadKey();
for (int i = 0; i < clseq.Length; i++)
{
    foreach (var item in clseq[i])
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine();
}

//var summary = BenchmarkRunner.Run<AllocationBenchmark>();