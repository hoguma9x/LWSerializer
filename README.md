[English](README.md) | [한국어](README.ko.md) 
# LWSerializer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-orange)
![.NET](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)

### LWSerializer is a **lightweight, high-performance binary serialization library** for the .NET environment.
It processes data with minimal overhead through direct memory copying and allows users to manually configure serialization order and padding. It is highly optimized for game development and high-performance server environments.

## Key Features
* **Performance**: Provides faster serialization/deserialization speeds same `MemoryPack`, which is already known for its performance.
* **Simple Usage**: Immediate integration via `LwBinaryWriter` and `LwBinaryReader` without complex configurations.
* **Manual Ordering**: Users can directly control the serialization order, allowing for flexible protocol design.
* **Padding**: Supports padding to prepare for future struct changes, maintaining backward compatibility even when data structures are expanded.
* **Memory Efficiency**:
    * Supports serialization for all `unmanaged` structs.
    * Safe and efficient memory access through memory aliasing handling.
    * Capable of serializing large datasets up to **2GB**.
* **Collection Support**: Native support for major generic collections such as `List<T>` and `Dictionary<TKey, TValue>`.
* **Compatibility**: Supports .NET Standard 2.1, making it compatible with both Unity and modern .NET environments.

## Performance Comparison
LWSerializer uses a direct memory copy mechanism, making it faster than existing libraries and ensuring zero GC (Garbage Collection).

| Library | Performance | Methodology |
| :--- | :---: | :--- |
| BinaryWriter | Very High | Direct Memory Copy + Manual |
| MemoryPack | Very High | Code Generation / Direct Memory Copy |
| **LWSerializer** | **Very High** | **Direct Memory Copy** |
| Protobuf-net | High | Contract-Based |
| Json.NET | Medium | Text-Based |

*Benchmarking results will be provided soon.*



## Usage
0. Simple Serialization
Example of basic serialization using `LwUtility.cs`
```csharp
var bytes = LwUtility.To("hello world");
var data = LwUtility.From(bytes);
var hash = LwUtility.ToXxHash64(bytes);


```
1. Unmanaged Structs
Example of converting an unmanaged struct to binary and restoring it:
```csharp
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


```

2. Managed Objects (Classe)
Example of converting a managed object to binary and restoring it by implementing ILwSerializable:
```csharp
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

