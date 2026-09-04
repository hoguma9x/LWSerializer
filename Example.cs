using System.Collections.Generic;
using LWSerializer;

namespace LWBinarySerializer
{
    public static class Example_Unmanaged
    {
        public struct ExampleStruct
        {
            public int m_int;
            public float m_float;
            public bool m_bool;
            public decimal m_decimal;
        }
        
        public static byte[] Write(ExampleStruct exampleStruct)
        {
            using (var writer = new LwBinaryWriter())
            {
                writer.Write(exampleStruct);
                return writer.ToArray(); //or writer.ToPtr()
            }
        }

        public static ExampleStruct Read(byte[] bytes)
        {
            ExampleStruct result;
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.Read(out result);
            }
            return result;
        }
    }
    
    public static class Example_Managed
    {
        public class ExampleClass : ILwSerializable
        {
            public int m_int;
            public float m_float;
            public string[] m_arr;
            
            void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
            {
                writer.Write(m_int, m_float, m_arr);
            }

            void ILwSerializable.OnNativeRead(LwBinaryReader reader)
            {
                reader.Read(out m_int, out m_float, out m_arr);
            }
        }
        
        public static byte[] Write(ExampleClass exampleStruct)
        {
            using (var writer = new LwBinaryWriter())
            {
                writer.Write(exampleStruct);
                return writer.ToArray(); //or writer.ToPtr()
            }
        }

        public static ExampleClass Read(byte[] bytes)
        {
            ExampleClass result;
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.Read(out result);
            }
            return result;
        }
    }

    public static class Example_Dictionary
    {
        public static byte[] Write(Dictionary<string, string> dictionary)
        {
            using (var writer = new LwBinaryWriter())
            {
                writer.Write(dictionary);
                return writer.ToArray(); //or writer.ToPtr()
            }
        }

        public static Dictionary<string, string> Read(byte[] bytes)
        {
            Dictionary<string, string> result;
            using (var reader = new LwBinaryReader(bytes))
            {
                reader.Read(out result);
            }
            return result;
        }
    }
}