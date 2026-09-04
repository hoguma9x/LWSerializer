namespace LWSerializer
{
    public interface ILwSerializable
    {
        void OnNativeWrite(LwBinaryWriter writer);
        void OnNativeRead(LwBinaryReader reader);
    }
}