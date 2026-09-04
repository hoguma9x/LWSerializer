#if UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER
using Unity.Collections;

namespace LWSerializer.Formatters.Unity
{
    public class NativeReferenceFormatter<T> : ILWFormatter<NativeReference<T>> where T : unmanaged
    {
        public override void Serialize(LwBinaryWriter writer, NativeReference<T> value)
        {
            writer.Write(value.Value);
        }

        public override void DeSerialize(LwBinaryReader reader, out NativeReference<T> value)
        {
            value = new NativeReference<T>(Allocator.Persistent);
            reader.Read(out T data);
            value.Value = data;
        }
    }
}
#endif