using NickStrupat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CacheLineSequence
{
    public sealed class CacheLineSequence<T> : IDisposable
        where T : unmanaged
    {
        private readonly IntPtr _dataPtr;
        private ref readonly T[] _data
        {
            get { unsafe { return ref Unsafe.AsRef<T[]>(_dataPtr.ToPointer()); } }
        }
        private readonly int _dataSize;
        private readonly int _cacheLineSize;
        private readonly int _cacheLineCapacity;
        private bool _disposed = false;

        /// <summary>
        /// Number of cache lines for T
        /// </summary>
        public readonly int Length;

        public Span<T> Data { get { return _data.AsSpan(); } }

        public Span<T> this[int ind]
        {
            get
            {
                if (ind < 0 || ind >= _dataSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(ind));
                }
                return Data.Slice(ind == 0 ? 0 : ind * _cacheLineCapacity, _cacheLineCapacity);
            }
        }

        public CacheLineSequence(params T[] items)
        {
            _dataSize = items.Length;
            _cacheLineSize = CacheLine.Size;
            _cacheLineCapacity = _cacheLineSize / Unsafe.SizeOf<T>();

            var dataByteCount = _dataSize * Unsafe.SizeOf<T>();

            Length = (int)Math.Ceiling(dataByteCount / (double)_cacheLineSize);

            unsafe
            {
                void* pData = NativeMemory.AlignedAlloc((nuint)dataByteCount, (nuint)_cacheLineSize);
                Unsafe.Copy(pData, ref items); //to be improved :)
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
}
