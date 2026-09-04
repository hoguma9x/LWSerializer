using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LWSerializer.Formatters
{
    public class ListFormatter<V> : ILWFormatter<List<V>>
        where V : class
    {
        public override void Serialize(LwBinaryWriter writer, List<V> value)
        {
            writer.Write(value.Count);
            foreach (var v in value)
                writer.Write(v);
        }

        public override void DeSerialize(LwBinaryReader reader, out List<V> value)
        {
            reader.Read(out int count);
            value = new List<V>(count);
            for (int i = 0; i < count; i++)
            {
                reader.Read(out V v);
                value.Add(v);   
            }
        }
    }
    
    public unsafe class ListFormatter_Unmanaged<V> : ILWFormatter<List<V>>
        where V : unmanaged
    {
        private readonly Func<List<V>, V[]> _getInternalArray = null;
        private readonly FieldInfo _sizeField = typeof(List<V>).GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
       
        public ListFormatter_Unmanaged()
        {
            var field = typeof(List<V>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                _getInternalArray = (List<V> list) => (V[])field.GetValue(list);
        }
        
        public override void Serialize(LwBinaryWriter writer, List<V> value)
        {
            int byteLen = GetSize<V>() * value.Count;
            writer.Write(value.Count);
            if (value.Count == 0)
                return;
            var internalArray = _getInternalArray.Invoke(value);
            void* destPtr = writer.BeginWrite(byteLen);
            fixed (void* srcPtr = &internalArray[0])
                Unsafe.CopyBlock(destPtr, srcPtr, (uint)byteLen);
        }

        public override void DeSerialize(LwBinaryReader reader, out List<V> value)
        {
            reader.Read(out int count);
            value = new List<V>(count);
            if (count == 0)
                return;
            int byteLen = count * GetSize<V>();
            _sizeField.SetValue(value, count);
            void* source = reader.BeginRead(byteLen);
            V[] internalArray = _getInternalArray.Invoke(value);
            fixed (V* ptr = internalArray)
            {
                Unsafe.CopyBlock(ptr, source, (uint)byteLen);
            }
        }
    }
}