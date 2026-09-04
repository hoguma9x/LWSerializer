#if UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER
using System.Runtime.CompilerServices;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LWSerializer.Formatters.Unity
{
    public unsafe class NativeArrayFormatter<T> : ILWFormatter<NativeArray<T>> where T : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, NativeArray<T> value)
        {
            writer.Write(value.Length);
            if (value.Length == 0)
                return;
            int byteLen = GetSize<T>() * value.Length;
            var ptr = writer.BeginWrite(byteLen);
            void* srcPtr = value.GetUnsafePtr();
            Unsafe.CopyBlock(ptr, srcPtr, (uint)byteLen);
        }

        public override void DeSerialize(LwBinaryReader reader, out NativeArray<T> value)
        {
            reader.Read(out int length);
            if (length == 0)
            {
                value = new NativeArray<T>(0, Allocator.Persistent);
                return;
            }
            int byteLen = GetSize<T>() * length;
            value = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            void* destPtr = value.GetUnsafePtr();
            void* source = reader.BeginRead(byteLen);
            Unsafe.CopyBlock(destPtr, source, (uint)byteLen);
        }
    }
}
#endif