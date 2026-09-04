using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LWSerializer.Formatters;

namespace LWSerializer
{
    public unsafe class LwBinaryReader : IDisposable
    {
        private IntPtr _array;
        private long _position;
        private GCHandle _handle;
        private StringFormatter _stringFormatter;
        
        #region constructor
        public LwBinaryReader(LwNativePointer<byte> span)
        {
            _array = span;
            _position = 0;
        }
        
        public LwBinaryReader(IntPtr binaryData)
        {
            _array = binaryData;
            _position = 0;
        }

        public LwBinaryReader(byte[] binaryData)
        {
            _handle = GCHandle.Alloc(binaryData, GCHandleType.Pinned);
            _array = _handle.AddrOfPinnedObject();
        }
        
        ~LwBinaryReader()
        {
            _dispose();
        }

        public void ManualInitialize(IntPtr binaryPtr)
        {
            _position = 0;
            _array = binaryPtr;
            if (_handle.IsAllocated)
                _handle.Free();
        }
        #endregion
        public Span<byte> ToSpan => new Span<byte>(_array.ToPointer(), (int)_position);
        
        public void SetPosition(long position)
        {
            _position = position;
        }
        
        /// <summary>
        /// Writer에 추가적인 쓰기작업이 발생할경우 반환한 주소값이 유효하지않게 될 수 있습니다.
        /// </summary>
        /// <returns></returns>
        public LwNativePointer<byte> ToPtr()
        {
            return new LwNativePointer<byte>(_array);
        }
        
        #region BeginRead
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(int byteLength) => BeginRead((uint)byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginRead(uint byteLength)
        {
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1;
            long currentPos = _position;
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            _position = alignedPos + byteLength;
            return (byte*)_array + alignedPos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* Peek(int byteLengthint)
        {
            uint byteLength = (uint)byteLengthint;
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1;
            long currentPos = _position;
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            return (byte*)_array + alignedPos;
        }
        #endregion

        public void ReadPadding(int byteLen)
        {
            BeginRead(byteLen);
        }

        public int PeekSpanLength<T>() where T : unmanaged
        {
            return Peek<int>();
        }

        public void Read<T>(out T data)
        {
            LWFormatterCache<T>.Formatter.DeSerialize(this, out data);
        }
        
        public T Peek<T>() where T : unmanaged
        {
            return *(T*)Peek(Unsafe.SizeOf<T>());
        }
        
        public ulong GetXxHash64(long seed)
        {
            return LwUtility.ToXxHash64(ToPtr(), (int)_position, seed);
        }
        
        public void Dispose()
        {
            _dispose();
            GC.SuppressFinalize(this);
        }

        private void _dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }

        #region T1....T7
        public void Read<T1, T2>(out T1 _1, out T2 _2)
        {
            Read(out _1);
            Read(out _2);
        }

        public unsafe void Read<T1, T2, T3>(out T1 _1, out T2 _2, out T3 _3)
        {
            Read(out _1);
            Read(out _2);
            Read(out _3);
        }

        public unsafe void Read<T1, T2, T3, T4>(out T1 _1, out T2 _2, out T3 _3, out T4 _4)
        {
            Read(out _1);
            Read(out _2);
            Read(out _3);
            Read(out _4);
        }

        public unsafe void Read<T1, T2, T3, T4, T5>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5)
        {
            Read(out _1);
            Read(out _2);
            Read(out _3);
            Read(out _4);
            Read(out _5);
        }

        public unsafe void Read<T1, T2, T3, T4, T5, T6>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5,
            out T6 _6)
        {
            Read(out _1);
            Read(out _2);
            Read(out _3);
            Read(out _4);
            Read(out _5);
            Read(out _6);
        }

        public unsafe void Read<T1, T2, T3, T4, T5, T6, T7>(out T1 _1, out T2 _2, out T3 _3, out T4 _4, out T5 _5,
            out T6 _6, out T7 _7)
        {
            Read(out _1);
            Read(out _2);
            Read(out _3);
            Read(out _4);
            Read(out _5);
            Read(out _6);
            Read(out _7);
        }
        #endregion
    }
}