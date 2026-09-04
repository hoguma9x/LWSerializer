using System.Runtime.CompilerServices;

namespace LWSerializer.Formatters
{
    public unsafe class UnmangedFormatter<T> : ILWFormatter<T> where T : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, T value)
        {
            *(T*)writer.BeginWrite(GetSize<T>()) = value;
        }
        
        public override void DeSerialize(LwBinaryReader reader, out T value)
        {
            value = *(T*)reader.BeginRead(GetSize<T>());
        }
    }
}