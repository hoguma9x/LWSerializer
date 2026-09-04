using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LWSerializer.Formatters;

namespace LWSerializer
{
    /// <summary>
    /// 변수를 바이너리 데이터로 변경하여 작성하는데 사용되는 구조체 입니다
    /// 버스트 기능을 지원합니다. / 작성완료시 Dispose 필요
    /// 구조체이지만 참조형식처럼 사용해도 됩니다
    /// MAX 2GB
    /// </summary>
    public unsafe class LwBinaryWriter : IDisposable
    {
        private const uint MAXCAPACITY = int.MaxValue-1;
        private const uint CacheLineSize = 64;
        
        private IntPtr _array;
        private long _length;
        private long _capacity;

        public int Length => (int)_length;
        public int Position => (int)_length;
        public Span<byte> ToSpan => new Span<byte>(_array.ToPointer(), (int)_length);
        
        public LwBinaryWriter()
        {
            _array = Marshal.AllocHGlobal(new IntPtr(CacheLineSize));
            _capacity = CacheLineSize;
            _length = 0;
        }
        
        public LwBinaryWriter(long capacity)
        {
            _array = Marshal.AllocHGlobal(new IntPtr(capacity));
            _capacity = (uint)capacity;
            _length = 0;
        }

        ~LwBinaryWriter()
        {
            _dispose();
        }

        /// <summary>
        /// Writer에 추가적인 쓰기작업이 발생할경우 반환한 주소값이 유효하지않게 될 수 있습니다.
        /// </summary>
        /// <returns></returns>
        public LwNativePointer<byte> ToPtr()
        {
            return new LwNativePointer<byte>(_array);
        }
        
        #region BeginWrite / EnsureCapacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* BeginWrite(int byteLength) => BeginWrite((uint)byteLength);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void* BeginWrite(uint byteLength)
        {
            if (byteLength == 0)
                throw new Exception($"{byteLength} bytes length cannot be 0");
            // 1. 정렬 단위(Alignment) 결정
            // 원칙: 데이터 크기가 곧 정렬 단위가 됨 (최대 8바이트)
            // 예: 4바이트 int -> 4의 배수 주소, 8바이트 long -> 8의 배수 주소
            // (참고: 3, 5, 6 같은 2의 거듭제곱이 아닌 크기는 1바이트 정렬로 취급하여 안전하게 처리)
            uint alignment = byteLength >= 8 ? 8 : byteLength;
            if ((alignment & (alignment - 1)) != 0) alignment = 1; // 2의 거듭제곱 검사
            
            long currentPos = _length;
            // 2. 정렬된 위치(Aligned Position) 계산 (비트 연산 최적화)
            // 공식: (현재위치 + (정렬단위 - 1)) & ~(정렬단위 - 1)
            long alignedPos = (currentPos + (alignment - 1)) & ~(alignment - 1);
            long padding = alignedPos - currentPos;
            long totalRequired = padding + byteLength;
            CheckAndEnsureCapacity(_length + totalRequired);
            _length += totalRequired;
            return (byte*)_array + alignedPos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private void CheckAndEnsureCapacity(long required)
        {
            if (_capacity < required)
                EnsureCapacity(required);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)] 
        private void EnsureCapacity(long required)
        {
            if (required > MAXCAPACITY)
                throw new OutOfMemoryException();
            long t_capacity = _capacity + (_capacity >> 1); // 1.5배
            if (t_capacity < required)
                t_capacity = required;
            // Align Up to CacheLineSize (64)
            t_capacity = (t_capacity + CacheLineSize - 1) & ~(CacheLineSize - 1);
            if (t_capacity >= MAXCAPACITY)
                t_capacity = MAXCAPACITY;
            _array = Marshal.ReAllocHGlobal(_array, new IntPtr(t_capacity));
            _capacity = t_capacity;
        }
        #endregion
        
        public void Write<T>(T data)
        {
            LWFormatterCache<T>.Formatter.Serialize(this, data);
        }

        public void Write(object obj)
        {
            throw new Exception("object type not supported");
        }
        
        public void WritePadding(int bytelen)
        {
            BeginWrite(bytelen);
        }
        
        public ulong GetXxHash64(long seed)
        {
            return LwUtility.ToXxHash64(ToPtr(), (int)_length, seed);
        }
        
        public byte[] ToArray()
        {
            return ToPtr().AsSpan((int)_length).ToArray();
        }
        
        public void Dispose()
        {
            _dispose();
            GC.SuppressFinalize(this);
        }

        private void _dispose()
        {
            if (_array != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_array);
                _array = IntPtr.Zero;
                _length = _capacity = 0;
            }
        }

        public void SetLength(int length)
        {
            _length = length;
        }
        
        #region T1....T7
        public void Write<T1, T2>(T1 _1, T2 _2)
        {
            Write(_1);
            Write(_2);
        }

        public void Write<T1, T2, T3>(T1 _1, T2 _2, T3 _3) 
        {
            Write(_1);
            Write(_2);
            Write(_3);
        }
        
        public void Write<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4) 
        {
            Write(_1);
            Write(_2);
            Write(_3);
            Write(_4);
        }

        public void Write<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
        {
            Write(_1);
            Write(_2);
            Write(_3);
            Write(_4);
            Write(_5);
        }
        public void Write<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
            where T6 : unmanaged
        {
            Write(_1);
            Write(_2);
            Write(_3);
            Write(_4);
            Write(_5);
            Write(_6);
        }
        public void Write<T1, T2, T3, T4, T5, T6, T7>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7)
        {
            Write(_1);
            Write(_2);
            Write(_3);
            Write(_4);
            Write(_5);
            Write(_6);
            Write(_7);
        }
        #endregion
    }
}