# Gameplay Patterns

Test'te "neden böyle yaptın?" sorusu gelirse net cevap verecek mimari pattern'ler.

---

## 1. Composition over inheritance

❌ **Yapma:** `class Swordsman : Character`, `class Archer : Character`, `class Mage : Character`
✅ **Yap:** Tek `Character` MonoBehaviour + `[SerializeField] CharacterStats` + sub-behavior'lar.

**Sub-behavior MonoBehaviour mu, plain C# mi?** — Tek varsayılan kuralı:
- **Plain C# class** (Awake'te `new` ile yaratılan) → `Health`, `Mover`, `TargetSelector` gibi pure logic. Test edilir, alloc-free, sahne dışı yaşar.
- **MonoBehaviour** → Unity callback'lerine ihtiyacı olanlar (`OnTriggerEnter`, `OnAnimatorMove`, animation event receiver, çocuk visual prefab'a attach edilenler).

> Cookbook §1 Character örneği bu kuralı uyguluyor: `Health` ve `Mover` plain class, çünkü Unity callback gerekmiyor.

**Sebep (genel):** Diamond inheritance, derin hiyerarşilerde override hell. Composition: her component bağımsız test edilir, swap edilir, prefab variant cehennemine girmez.

---

## 2. ScriptableObject Architecture

| Tip | Kullanım |
|---|---|
| `CharacterStats` SO | Düşman/oyuncu unit istatistikleri (HP, damage, hız). Designer Inspector'dan tweak. |
| `LevelConfig` SO | Sahne başına dalgalar, AI profili, gold target. |
| `EventChannel` SO | Sistemler arası event broadcast, prefab/sahne arası bağ kopuk. |
| `WeaponData` SO | Silah istatistikleri, prefab, ses. |

**Sebep:** Designer-friendly (kod değişmeden tweak), hot-reload, tek kaynaktan referans.

```csharp
[CreateAssetMenu(menuName = "MyGame/Unit Stats")]
public sealed class CharacterStats : ScriptableObject { ... }
```

---

## 3. Static Service Locator (singleton'a alternatif)

❌ **Singleton:** `PlayerInstance.Instance.X()` — global state, test edilemez, sahne lifetime karışık.
✅ **Locator:** `GameServices.Get<IAudio>().Play(clip)` — interface'e bind, fake test edilir.

```csharp
public static class GameServices {
    public static void Register<T>(T svc) where T : class { ... }
    public static T Get<T>() where T : class { ... }
}
```

Bootstrap GameObject:
```csharp
private void Awake() {
    GameServices.Register<IAudio>(GetComponent<AudioService>());
    GameServices.Register<IPool>(new ProjectilePoolService());
}
```

---

## 4. Event sistemi 3 katmanlı

| Bağlam | Kullan | Neden |
|---|---|---|
| Sistemler arası (Combat → UI) | **SO Event Channel** | Gevşek bağ, Inspector-friendly |
| Aynı GameObject içi component'ler | **C# `event` / `Action`** | Lokal, hızlı |
| UI Button/Slider | **Unity `UnityEvent`** | Standart, designer Inspector'dan bağlar |

```csharp
// SO channel
[CreateAssetMenu] public sealed class IntEvent : ScriptableObject {
    public event Action<int> OnRaised;
    public void Raise(int v) => OnRaised?.Invoke(v);
}

// C# event
public sealed class Health : MonoBehaviour {
    public event Action OnDied;
    public void Kill() => OnDied?.Invoke();
}
```

---

## 5. Object Pool (allocation-free)

`Instantiate` / `Destroy` GC alloc + frame spike. Pool ile reuse:
- 60 FPS'te 200 projectile spawn → pool ile 0 GC
- Hot path'lerde (combat, particle, projectile) **şart**

`unity-cookbook.md` → Object Pool

---

## 6. Composition'ı interface ile gevşet

❌ Character doğrudan `_target.GetComponent<EnemyHealth>()` çağırıyor → tightly coupled
✅ `IDamageable` interface, kim implement ederse hedef olabilir

```csharp
public interface IDamageable {
    bool IsDead { get; }
    Transform Transform { get; }
    void TakeDamage(float dmg, Side attacker);
}
```

Building, Character, BreakableProp hepsi `IDamageable` implement → `CharacterCombat` herkesi vurur.

---

## 7. Data ↔ Behavior ayrımı

| Veri | Behavior |
|---|---|
| `CharacterStats` SO | `CharacterCombat` MonoBehaviour |
| `LevelConfig` SO | `LevelManager` MonoBehaviour |
| `AIProfile` SO | `EnemySpawnAI` MonoBehaviour |

Veri = Inspector-tweakable, immutable runtime'da. Behavior = component, runtime'da çalışır.

---

## 8. Ne zaman bunlardan **vazgeçilir**?

Test 45 dk, "minimum viable" istiyorlar. Scope küçükse:
- Bir scene, 2-3 prefab → ScriptableObject Architecture'a gerek yok, MonoBehaviour fields yeter
- Tek ses çalacak → `AudioSource.PlayClipAtPoint()` enough, audio service overkill
- 10 düşman spawn → pool gereksiz, direct `Instantiate` OK

**Mimari pattern'leri overengineering tuzağına düşmeden kullan.** Görüşmeci "scope küçük olsa da senior pattern uygulamış" görüyor → puanı yüksek. Ama herşeyi pattern'le sarmalamak yavaş çıkar.

---

## 9. Sample karar tablosu

Test sırasında "şunu nasıl yapayım?" sorusu için hızlı karar:

| İhtiyaç | İlk dene |
|---|---|
| 5+ aynı obje spawn | Object Pool |
| Birden fazla prefab'a aynı stats | ScriptableObject |
| Sahnedeki 2 component konuşacak | C# event |
| Sahne A → Sahne B veri taşı | Static service / SO |
| Asset bağımlılığı esnek | SO event channel |
| Designer tweak edilecek | SerializeField + SO |
| Editor-only bir tool | Editor/ klasörü + MenuItem |
