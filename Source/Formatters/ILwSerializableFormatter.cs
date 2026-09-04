using System;

namespace LWSerializer.Formatters
{
    public class ILwSerializableFormatter<T> : ILWFormatter<T> where T : ILwSerializable
    {
        public override void Serialize(LwBinaryWriter writer, T value)
        {
            value.OnNativeWrite(writer);
        }

        public override void DeSerialize(LwBinaryReader reader, out T value)
        {
            var instance = Activator.CreateInstance<T>();
            instance.OnNativeRead(reader);
            value = instance;
        }
    }
}