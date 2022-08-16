using System;
using CacheLineSequence;

using var cacheLineSequence = new CacheLineSequence<long>(
    101, 102, 103, 104, 105, 106, 107, 108,
    201, 202, 203, 204, 205, 206, 207, 208);

Console.WriteLine(cacheLineSequence.Length);

for (int i = 0; i < cacheLineSequence.Length; i++)
{
    foreach (var item in cacheLineSequence[i])
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine();
}

//var summary = BenchmarkRunner.Run<AllocationBenchmark>();