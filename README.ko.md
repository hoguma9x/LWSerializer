[English](README.md) | [한국어](README.ko.md) 
# LWSerializer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-orange)
![.NET](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)


### LWSerializer는 .NET 환경을 위한 **경량 고성능 바이너리 직렬화 라이브러리**입니다.
메모리 직접복사 방식을 통해 최소한의 오버헤드로 데이터를 처리하며, 직렬화 순서나 Padding값을 직접 설정할 수 있습니다 
게임 개발 및 고성능 서버 환경에 최적화되어 있습니다.



## Key Features
* **Performance**: 이미 빠른 직렬화 API `MemoryPack`과 비슷한 빠른 직렬화/역직렬화 속도를 제공합니다.
* **Simple Use**: 복잡한 설정 없이 `LwBinaryWriter/Reader`를 통해 즉시 사용 가능합니다.
* **Manual Ordering**: 데이터의 직렬화 순서를 사용자가 직접 제어할 수 있어 유연한 프로토콜 설계가 가능합니다.
* **Padding**: 구조체 변경에 대비한 Padding 기능을 지원하여, 데이터 구조 확장 시에도 하위 호환성을 유지할 수 있습니다.
* **Memory Efficiency**:
    * 모든 `unmanaged` 구조체 직렬화 지원
    * Memory Aliasing 대응을 통한 안전하고 효율적인 메모리 접근
    * 최대 **2GB** 크기의 대용량 데이터 직렬화 가능
* **Collection Support**: `List<T>`, `Dictionary<TKey, TValue>` 등 주요 제네릭 컬렉션을 기본 지원합니다.
* **Compatibility**: .NET Standard 2.1 지원으로 Unity 및 최신 .NET 환경 모두에서 사용 가능합니다.



## Performance Comparison
LWSerializer는 직접적인 메모리 복사 메커니즘을 사용하여 빠르고 GC가 발생하지않습니다
| Library | Performance | Methodology |
| :--- | :---: | :--- |
| BinaryWriter | Very High | Direct Memory Copy + Manual |
| MemoryPack | Very High | Code Gen / Direct Memory Copy |
| **LWSerializer** | Very High | **Direct Memory Copy** |
| Protobuf-net | High | Contract Based |
| Json.NET | Medium | Text Based |

벤치마킹은 추후 제공예정입니다



## Usage
0. 'LwUtility.cs' 를 사용하여 간단하게 직렬화를 하는 예제입니다
```csharp
var bytes = LwUtility.To("hello world");
var data = LwUtility.From(bytes);
var hash = LwUtility.ToXxHash64(bytes);


```

2. `unmanaged` 구조체를 바이너리로 변환하고 다시 복구하는 예제입니다.
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
2. `managed` 구조체를 바이너리로 변환하고 다시 복구하는 예제입니다

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

