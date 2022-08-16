# CacheLineSequence
A wrapper onto arrays of inlined (value-type) data to be represented as lines of cache.

## Description
The idea is to allocate a chunk of data aligned on the cache line size boundary of the CPU, providing an abstraction over the lines.

The class allocates on the unmanaged memory to ensure a few things:
1. Allocation happens on the cache line size boundary.
2. There is no additional object header, method table ptr and array size overhead.
3. The array is not being moved during the execution of the program by the GC.

The indexer returns a Span<T> over each cache line.

## Usage
```csharp
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
```

Output:
```sh
101 102 103 104 105 106 107 108
201 202 203 204 205 206 207 208
```

## Allocation benchmark
```md
BenchmarkDotNet=v0.13.1
Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

|     Method |        array |     Mean |     Error |  StdDev |  Gen 0 | Allocated native memory | Native memory leak | Allocated |
|----------- |------------- |---------:|----------:|--------:|-------:|------------------------:|-------------------:|----------:|
| Allocation |  Int64[1024] | 190.0 ns |   7.44 ns | 0.41 ns | 0.0076 |                 8,263 B |                  - |      48 B |
| Allocation | Int64[16384] | 335.0 ns |  73.93 ns | 4.05 ns | 0.0076 |               131,143 B |                  - |      48 B |
| Allocation |  Int64[4096] | 331.6 ns | 126.87 ns | 6.95 ns | 0.0076 |                32,839 B |                  - |      48 B |
| Allocation |    Int64[64] | 185.0 ns |  11.03 ns | 0.60 ns | 0.0076 |                   583 B |                  - |      48 B |
```

## References
The program utilizes [CacheLineSize.NET](https://github.com/NickStrupat/CacheLineSize.NET "CacheLineSize.NET library") package to retrieve the platform's cache line size.
