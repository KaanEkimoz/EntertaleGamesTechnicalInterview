# Performance Cheatsheet

Görüşmeci "performans için ne yaptın?" diye sorabilir. Hızlı cevaplar.

---

## Update / FixedUpdate / LateUpdate'te yasaklar

❌ Her frame:
```csharp
void Update() {
    var enemy = FindFirstObjectByType<Enemy>();         // tüm sahne tarama
    var rb = GetComponent<Rigidbody>();            // ucuz değil
    var cam = Camera.main;                         // tag-search
    var prefab = Resources.Load<GameObject>("X");  // disk I/O
    text.text = "Score: " + score;                 // string alloc her frame
}
```

✅ Awake/Start'ta cache:
```csharp
private Camera _cam;
private Rigidbody _rb;
private string _scoreFormat = "Score: {0}";
private int _lastScore = -1;

void Awake() {
    _cam = Camera.main;
    _rb = GetComponent<Rigidbody>();
}
void Update() {
    if (score != _lastScore) {
        text.text = string.Format(_scoreFormat, score);
        _lastScore = score;
    }
}
```

---

## Allocation traps

| Yapma | Yap |
|---|---|
| `new List<T>()` her frame | `_buffer.Clear()` reusable list |
| `string1 + string2 + score` | `StringBuilder` veya cache |
| `foreach` over `IEnumerable<T>` (LINQ result, dictionary, custom enumerable) → boxing alloc | `for` veya `foreach` over concrete `List<T>` / dizisi (struct enumerator, alloc-free) |
| `LINQ` (`.Where`, `.Select`) hot path | Manuel for döngü |
| `new Vector3()` her frame | Reuse `_tempPos` field |
| `Instantiate` / `Destroy` | Object Pool |

---

## Frame timing

Hedef 60 FPS = 16.6 ms / frame. Profiler'da ana metrikler:
- **CPU:** PlayerLoop her frame
- **GPU:** Render
- **GC.Alloc:** 0 olmalı hot path'te

```csharp
// Profiler markeri ekle:
using Unity.Profiling;
private static readonly ProfilerMarker s_marker = new ProfilerMarker("MyHotMethod");
void Tick() {
    using (s_marker.Auto()) {
        // ...
    }
}
```

Profiler Window → Editor mode'da bile ipucu, ama gerçek metric için **build + Development Build + Autoconnect Profiler**.

---

## Sahne / asset

- **Static GameObject'leri Static işaretle** → batching, occlusion culling, lightmap baking
- **Lightmap** static objeler için (gameplay'de değil pre-baked)
- **Texture compression:** import settings → maxTextureSize 2048 mobile, 4096 PC
- **Audio:** mobile → mono + Vorbis, music → streaming
- **Prefab variant** kullan, duplicate yerine

---

## Top 5 fix when "frame drops var"

1. Profiler aç → **GC.Alloc spike'ı bul** → cache et
2. **Camera.main** cache et — yüzlerce çağrı
3. **GetComponent / Find** Awake'e taşı
4. **String concat / Debug.Log** her frame → kaldır
5. **Particle system** çok partikülü → emission rate düşür, max particles azalt

---

## Build size

- **Player Settings → Stripping:** Strip Engine Code (IL2CPP)
- **Quality Settings:** Texture Streaming
- **Audio:** mp3/Vorbis, mono mobile
- **Mesh Compression:** Off → Low (model import)

---

## Job System / Burst (büyük hesap → ana thread'den çıkar)

Sahnede 1000+ unit transform, AI tick, mesh deform vs. ana thread'i şişiriyorsa:

```csharp
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct MoveJob : IJobParallelFor
{
    public NativeArray<float3> Positions;
    [ReadOnly] public NativeArray<float3> Velocities;
    public float DeltaTime;

    public void Execute(int i) => Positions[i] += Velocities[i] * DeltaTime;
}

// Schedule
var handle = new MoveJob {
    Positions = _positions, Velocities = _velocities, DeltaTime = Time.deltaTime
}.Schedule(_positions.Length, 64);
handle.Complete();
```

> **Soru: "Frame drop var, nasıl çözersin?"** → Profiler → GC.Alloc bul → cache → Update'te yasaklara bak → hot loop'u Job + Burst'a al.
