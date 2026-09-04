using System.Collections.Generic;

namespace LWSerializer.Formatters
{
    public class DictionaryFormatter_UnmanagedKV<K, V> : ILWFormatter<Dictionary<K, V>>
        where K : unmanaged
        where V : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, Dictionary<K, V> value)
        {
            writer.Write(value.Count);
            foreach (var itm in value)
            {
                writer.Write(itm.Key);
                writer.Write(itm.Value);
            }
        }

        public override void DeSerialize(LwBinaryReader reader, out Dictionary<K, V> value)
        {
            reader.Read(out int length);
            value = new Dictionary<K, V>(length);
            for (int i = 0; i < length; i++)
            {
                reader.Read(out K Key);
                reader.Read(out V Value);
                value.Add(Key , Value);
            }
        }
    }
    
    public class DictionaryFormatter_UnmanagedK<K, V> : ILWFormatter<Dictionary<K, V>>
        where K : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, Dictionary<K, V> value)
        {
            writer.Write(value.Count);
            foreach (var itm in value)
            {
                writer.Write(itm.Key);
                writer.Write(itm.Value);
            }
        }

        public override void DeSerialize(LwBinaryReader reader, out Dictionary<K, V> value)
        {
            reader.Read(out int length);
            value = new Dictionary<K, V>(length);
            for (int i = 0; i < length; i++)
            {
                reader.Read(out K Key);
                reader.Read(out V Value);
                value.Add(Key , Value);
            }
        }
    }
    
    public class DictionaryFormatter_UnmanagedV<K, V> : ILWFormatter<Dictionary<K, V>>
        where V : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, Dictionary<K, V> value)
        {
            writer.Write(value.Count);
            foreach (var itm in value)
            {
                writer.Write(itm.Key);
                writer.Write(itm.Value);
            }
        }

        public override void DeSerialize(LwBinaryReader reader, out Dictionary<K, V> value)
        {
            reader.Read(out int length);
            value = new Dictionary<K, V>(length);
            for (int i = 0; i < length; i++)
            {
                reader.Read(out K Key);
                reader.Read(out V Value);
                value.Add(Key , Value);
            }
        }
    }
    
    public class DictionaryFormatter<K, V> : ILWFormatter<Dictionary<K, V>>
        where K : class
        where V : class
    {
        public override void Serialize(LwBinaryWriter writer, Dictionary<K, V> value)
        {
            writer.Write(value.Count);
            foreach (var itm in value)
            {
                writer.Write(itm.Key);
                writer.Write(itm.Value);
            }
        }

        public override void DeSerialize(LwBinaryReader reader, out Dictionary<K, V> value)
        {
            reader.Read(out int length);
            value = new Dictionary<K, V>(length);
            for (int i = 0; i < length; i++)
            {
                reader.Read(out K Key);
                reader.Read(out V Value);
                value.Add(Key , Value);
            }
        }
    }
}