[English](README.md) | [한국어](README.ko.md)

# LWSerializer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-orange)

LWSerializer is a binary serializer for C#.

**An unmanaged struct can be serialized as-is.** It does not need attributes, generated code, or `ILwSerializable`. Managed objects can define how their fields are written by implementing `ILwSerializable`.

## Quick start: unmanaged struct

```csharp
public struct PlayerState
{
    public int Level;
    public float Health;
    public bool IsAlive;
}

var original = new PlayerState
{
    Level = 10,
    Health = 75.5f,
    IsAlive = true
};

byte[] bytes = LwUtility.To(original);
PlayerState restored = LwUtility.From<PlayerState>(bytes);
```

The whole struct is copied in one operation. Keep its type and memory layout compatible between the code that writes it and the code that reads it.

### Without creating an output `byte[]`

```csharp
ReadOnlySpan<byte> bytes = LwUtility.To_Immediate(original);
ConsumeImmediately(bytes);
```

`To_Immediate` returns a view of a reused buffer. Consume the span before another `LwUtility` write, or copy it when it must be kept.

## Managed objects

Implement `ILwSerializable` when the value contains managed fields such as `string` or arrays.

```csharp
public sealed class PlayerProfile : ILwSerializable
{
    public int Id;
    public string Name;

    public void OnNativeWrite(LwBinaryWriter writer)
    {
        writer.Write(Id, Name);
    }

    public void OnNativeRead(LwBinaryReader reader)
    {
        reader.Read(out Id, out Name);
    }
}

byte[] bytes = LwUtility.To(new PlayerProfile { Id = 7, Name = "Rui" });
PlayerProfile restored = LwUtility.From<PlayerProfile>(bytes);
```

LWSerializer also includes formatters for strings, compatible arrays, `List<T>`, `Dictionary<TKey, TValue>`, and optional Unity Collections types.

## Before using it

- Write and read fields in the same order and with compatible types.
- Unmanaged structs use their raw memory layout. Define a layout and version policy when data must remain compatible across builds or platforms.
- Deserialize only trusted, structurally valid data. The reader does not validate payload bounds.
- `LwUtility` reuses static writer and reader instances, so do not call it concurrently or nest its calls.
- `null` and empty strings both read back as an empty string. Padding reserves space but does not create automatic version compatibility.

## Benchmark

One Unity 6000.3.14f1 Windows Editor debug run on a Ryzen 7 8845HS. The same value contained scalar fields, `int[100]`, and `Sample[100]`. Each result is the median of seven round averages after 13 warmups, with 337 operations per round. Serialization reused output buffers.

| Serializer | Payload | Serialize median | Serialize min / max | Deserialize median | Deserialize min / max |
| :-- | --: | --: | --: | --: | --: |
| LWSerializer (`To_Immediate`) | 6,072 B | 0.201 µs/op | 0.200 / 0.219 µs/op | 1.401 µs/op | 1.371 / 16.129 µs/op |
| MemoryPack 1.21.4 | 6,068 B | 0.328 µs/op | 0.310 / 0.366 µs/op | 1.431 µs/op | 1.258 / 2.753 µs/op |
| protobuf-net 3.2.56 | 6,572 B | 33.216 µs/op | 32.306 / 36.930 µs/op | 31.587 µs/op | 31.173 / 32.017 µs/op |
| Unity JsonUtility + UTF-8 | 16,956 B | 128.052 µs/op | 125.201 / 254.309 µs/op | 127.505 µs/op | 124.999 / 137.551 µs/op |
| BinaryFormatter (legacy) | 5,966 B | 437.315 µs/op | 420.774 / 797.089 µs/op | 355.845 µs/op | 349.899 / 1,130.578 µs/op |


## License

MIT. See [LICENSE](LICENSE).
