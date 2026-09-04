using System;
using System.IO;
using System.Text;

namespace LWSerializer.Formatters
{
    public unsafe class StringFormatter : ILWFormatter<string>
    {
        public override void Serialize(LwBinaryWriter writer, string value)
        {
            if(string.IsNullOrEmpty(value))
                writer.Write(0);
            else
            {
                fixed (char* charPtr = value)
                {
                    int byteCount = Encoding.UTF8.GetByteCount(charPtr, value.Length);
                    writer.Write(byteCount); 
                    byte* ptr = (byte*)writer.BeginWrite(byteCount);
                    Encoding.UTF8.GetBytes(charPtr, value.Length, ptr, byteCount);
                }
            }
        }

        public override void DeSerialize(LwBinaryReader reader, out string value)
        {
            reader.Read(out int byteCount);
            if (byteCount == 0)
            {
                value = "";
                return;
            }
            byte* srcPtr = (byte*)reader.BeginRead(byteCount);
            value = Encoding.UTF8.GetString(srcPtr, byteCount);
        }
    }
}