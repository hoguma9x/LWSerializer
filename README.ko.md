[English](README.md) | [한국어](README.ko.md)

# LWSerializer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-orange)

LWSerializer는 C#용 바이너리 직렬화 라이브러리입니다.

**unmanaged 구조체는 정의한 모습 그대로 직렬화할 수 있습니다.** Attribute, 코드 생성, `ILwSerializable` 구현이 필요하지 않습니다. Managed 객체는 `ILwSerializable`을 구현해 기록할 필드와 순서를 정할 수 있습니다.

## 빠른 시작: unmanaged 구조체

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

구조체 전체를 한 번에 복사합니다. 데이터를 쓰는 쪽과 읽는 쪽에서 타입과 메모리 배치를 호환되게 유지해야 합니다.

### 출력 `byte[]`를 만들지 않는 방법

```csharp
ReadOnlySpan<byte> bytes = LwUtility.To_Immediate(original);
ConsumeImmediately(bytes);
```

`To_Immediate`는 재사용 버퍼를 바라보는 span을 반환합니다. 다음 `LwUtility` 쓰기 전에 사용을 끝내고, 오래 보관해야 한다면 복사하세요.

## Managed 객체

`string`이나 배열 같은 managed 필드가 있다면 `ILwSerializable`을 구현합니다.

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

문자열, 호환되는 배열, `List<T>`, `Dictionary<TKey, TValue>`, 선택 사항인 Unity Collections 타입도 지원합니다.

## 사용 전 확인

- 필드를 쓴 순서와 같은 순서로 읽고, 서로 호환되는 타입을 사용해야 합니다.
- unmanaged 구조체는 원시 메모리 배치를 사용합니다. 빌드나 플랫폼이 달라도 데이터를 유지해야 한다면 레이아웃과 버전 규칙을 정해야 합니다.
- 구조가 올바른 신뢰 가능한 데이터만 역직렬화하세요. Reader는 payload 경계를 검사하지 않습니다.
- `LwUtility`는 정적 Writer와 Reader를 재사용합니다. 동시에 호출하거나 호출 안에서 다시 호출하면 안 됩니다.
- `null` 문자열과 빈 문자열은 모두 빈 문자열로 복원됩니다. Padding은 공간만 확보하며 자동으로 버전 호환성을 만들지는 않습니다.

## 벤치마크

Ryzen 7 8845HS의 Unity 6000.3.14f1 Windows Editor debug 환경에서 한 번 실행한 결과입니다. 모든 라이브러리는 scalar 필드, `int[100]`, `Sample[100]`을 포함한 같은 값을 사용했습니다. 13회 warmup 뒤 round당 337회씩 7 round를 측정했으며, 표에는 round 평균의 중앙값과 최솟값 및 최댓값을 표시했습니다. 직렬화 출력 버퍼는 재사용했습니다.

| Serializer | Payload | 직렬화 중앙값 | 직렬화 min / max | 역직렬화 중앙값 | 역직렬화 min / max |
| :-- | --: | --: | --: | --: | --: |
| LWSerializer (`To_Immediate`) | 6,072 B | 0.201 µs/op | 0.200 / 0.219 µs/op | 1.401 µs/op | 1.371 / 16.129 µs/op |
| MemoryPack 1.21.4 | 6,068 B | 0.328 µs/op | 0.310 / 0.366 µs/op | 1.431 µs/op | 1.258 / 2.753 µs/op |
| protobuf-net 3.2.56 | 6,572 B | 33.216 µs/op | 32.306 / 36.930 µs/op | 31.587 µs/op | 31.173 / 32.017 µs/op |
| Unity JsonUtility + UTF-8 | 16,956 B | 128.052 µs/op | 125.201 / 254.309 µs/op | 127.505 µs/op | 124.999 / 137.551 µs/op |
| BinaryFormatter (지원 종료) | 5,966 B | 437.315 µs/op | 420.774 / 797.089 µs/op | 355.845 µs/op | 349.899 / 1,130.578 µs/op |


## 라이선스

MIT. [LICENSE](LICENSE)를 확인하세요.
