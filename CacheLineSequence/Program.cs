// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Consume;

using var clseq = new CacheLineSequence<long>(
    100, 100, 100, 100, 100, 101, 100, 100,
    100, 100, 100, 100, 100, 100, 100, 100
    );
var cl = clseq[0];


foreach (var item in cl)
{
    Console.WriteLine(item);
}

for (int i = 0; i < cl.Length; i++)
{
    cl[i] = 10L;
}

foreach (var item in cl)
{
    Console.WriteLine(item);
}

//var summary = BenchmarkRunner.Run<Benchmark>();