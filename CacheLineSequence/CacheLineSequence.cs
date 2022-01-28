using NickStrupat;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Consume
{
    public sealed class CacheLineSequence<T> : IDisposable where T : unmanaged
    {
        private IntPtr _dataPtr;
        private ref readonly T[] getData()
        {
            unsafe
            {
                return ref Unsafe.As<byte, T[]>(ref *(byte*)_dataPtr.ToPointer());
            }
        }

        private ref readonly T[] _data => ref getData();
        private readonly int _dataSize;
        private readonly int _clSize;
        private readonly int _clCapacity;
        private bool _disposed = false;

        public readonly int Length;

        public Span<T> this[int ind]
        {
            get
            {
                if(ind < 0 || ind >= _dataSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(ind));
                }
                return _data.AsSpan().Slice(ind == 0 ? 0 : ind * _clCapacity, _clCapacity);
            }
        }

        public CacheLineSequence(params T[] items)
        {
            _dataSize = items.Length;
            _clSize = CacheLine.Size;
            _clCapacity = _clSize / Unsafe.SizeOf<T>();
            Length = (int)Math.Round(_dataSize * Unsafe.SizeOf<T>() / (float)_clSize);

            unsafe
            {
                void* pData = NativeMemory.AlignedAlloc((nuint)(_dataSize * sizeof(T)), (nuint)_clSize);
                Unsafe.Copy(pData, ref items);
                _dataPtr = new IntPtr(pData);
            }
        }

        ~CacheLineSequence() => Dispose(false);

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }   

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                unsafe
                {
                    NativeMemory.AlignedFree(_dataPtr.ToPointer());
                }
                _disposed = true;
            }
        }
    }


    //64b     64b     64b
    //1. 24b 40b 64b     64b
    //size: 42elements, read: 20
    //(size-read)*sizeof(T)
    //  cl1 (64bit alignment): 24b 1 2 3 4 5 6 7 8 9 10 //_value.Length > _cacheLineSize - objInfoLength / sizeof(T) then > 1 cl otherwise == 1 cl
    //  cl2 (64bit alignment): 11 12 13 14 15 16 ... 27
    //  cl3 (64bit alignment): 28 ... 42
    //  GetCacheLinesNumber(_value.Length) = 2

}

/*


    cl1 (64bit alignment): he he mt mt sz sz 1  2  3  4  5  6  7  8  9  10
    cl2 (64bit alignment): 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26
    cl3 (64bit alignment): 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42

    cl1 (64bit alignment): he he mt mt sz sz 1  2  3  4  5  6  7  8  9  10
    cl2 (64bit alignment): 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26
    cl3 (64bit alignment): 27 28 29 30 31 32 33 34 35 -- -- -- -- -- -- --
*/
