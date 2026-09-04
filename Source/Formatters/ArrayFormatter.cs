using System.Runtime.CompilerServices;

namespace LWSerializer.Formatters
{
    public unsafe class ArrayFormatter<T> : ILWFormatter<T[]>
        where T : class
    {
        public override void Serialize(LwBinaryWriter writer, T[] value)
        {
            writer.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i]);
        }

        public override void DeSerialize(LwBinaryReader reader, out T[] value)
        {
            reader.Read(out int len);
            value = new  T[len];
            for (int i = 0; i < len; i++)
            {
                reader.Read(out T t);
                value[i] = t;
            }
        }
    }
    
    public unsafe class ArrayFormatter_Unamanaged<T> : ILWFormatter<T[]>
        where T : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, T[] value)
        {
            int byteLen = GetSize<T>() * value.Length;
            writer.Write(value.Length);
            if (value.Length == 0)
                return;
            void* destPtr = writer.BeginWrite(byteLen);
            fixed (void* srcPtr = &value[0])
                Unsafe.CopyBlock(destPtr, srcPtr, (uint)byteLen);
        }

        public override void DeSerialize(LwBinaryReader reader, out T[] value)
        {
            reader.Read(out int length);
            int byteLen = length * GetSize<T>();
            value = new T[length];
            if (length == 0)
                return;
            void* source = reader.BeginRead(byteLen);
            fixed (void* destPtr = &value[0])
            {
                Unsafe.CopyBlock(destPtr, source, (uint)byteLen);
            }
        }
    }
}