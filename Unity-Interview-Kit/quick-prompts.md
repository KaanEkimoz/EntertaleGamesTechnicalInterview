# Quick Prompts — Test Sırasında Hazır AI Komutları

> Test'te zaman kazandıran "ready-made" prompt şablonları. Doldur ve gönder.

---

## 🎬 Açılış prompt'u (Claude Code'a)

```
Bu boş bir Unity 6 LTS URP test projesi. CLAUDE.md kuralları okudum.

Şu task'i yapacağız: [TASK BURAYA]

Plan:
1. [adım 1]
2. [adım 2]
3. [adım 3]

Önce planı confirm et, sonra adım adım uygula. Her adımda:
- Hangi tool kullandığını söyle
- Yaptığın işi tek satır özetle
- Bittikten sonra git commit (conventional)
```

---

## 🏗️ Character / Unit yarat

```
Composition tabanlı bir Character class yarat:
- namespace MyGame.Units
- [SerializeField] private CharacterStats _stats; (SO referansı)
- Health, Mover, Combat alt component'leri (her biri ayrı C# class, MonoBehaviour DEĞİL)
- Awake'te component'leri new ile yarat
- Update'te Mover.Tick(direction) ve Combat.Tick(target) çağır
- Cookbook §1'i baz al
```

---

## 📦 ScriptableObject yarat

```
CharacterStats ScriptableObject yarat:
- [CreateAssetMenu(menuName = "MyGame/Unit Stats")]
- _maxHealth, _moveSpeed, _damage, _attackInterval (hepsi [SerializeField] private float)
- Public read-only property'ler (PascalCase)
- Cookbook §6'yı baz al
- Test asset olarak Assets/_Project/SO/SwordsmanStats.asset yarat (manage_scriptable_object)
- _maxHealth=100, _moveSpeed=5, _damage=15, _attackInterval=1
```

---

## 🎨 Sahne kurulumu

```
Boş sahneye şunları ekle (Unity MCP):
- ===CAMERA=== grup, altında Main Camera (zaten var, gruplara taşı)
- ===LIGHTING=== grup, Directional Light (zaten var, taşı)
- ===GAMEPLAY=== grup
- ===GAMEPLAY=== altına 'Player' GameObject (Capsule primitive, position 0,1,0)
- Player'a [Character, Rigidbody, CapsuleCollider] ekle
- Sahneyi kaydet
```

---

## 🎯 Click-to-move

```
ClickToMove.cs yarat (Cookbook §9 baz, Input System tercihli):
- NavMeshAgent gerekli, Camera.main Awake'te cache
- Mouse.current.leftButton.wasPressedThisFrame ile click yakala
- Mouse.current.position.ReadValue() ile screen pos
- ScreenPointToRay → Physics.Raycast → NavMesh.SamplePosition → SetDestination
- Player Settings → Active Input Handling = Both olduğunu doğrula
- Plane (10,1,10) zemine, Static işaretle, AI → Navigation → Bake
- Player'a NavMeshAgent + ClickToMove ekle
```

> Görüşmeci legacy Input ister derse `Input.GetMouseButtonDown(0)` + `Input.mousePosition` ile yeniden yaz — 30 sn iş.

---

## 🎬 Animator FSM

```
Animator Controller yarat 'CharacterAnimator' adında, Assets/_Project/Animators/:
- Parameters: Speed (float), Attack (trigger), IsDead (bool)
- States: Idle, Walk, Attack, Death (Empty motion'lı, dummy)
- Transitions:
  - Idle → Walk: Speed > 0.1
  - Walk → Idle: Speed < 0.1
  - Idle/Walk → Attack: trigger Attack
  - Attack → Idle: Has Exit Time
  - Any State → Death: IsDead == true
- CharacterAnimatorController.cs scripti yarat (Cookbook §4) — public Set methods
```

---

## ♻️ Object Pool

```
Generic ObjectPool<T> yarat (Cookbook §2, namespace MyGame.Core):
- Constructor: prefab, initialSize, parent
- Get(pos, rot) ve Return(inst) metotları
- Sahnede ProjectilePool MonoBehaviour ekle, _projectilePrefab field'ı ile
- Awake'te new ObjectPool<Projectile>(_projectilePrefab, 32, transform)
- GameServices.Register<IProjectilePool>(this)
```

---

## 🔫 Damage on collision

```
Projectile.cs:
- speed, damage, lifetime fields
- Rigidbody collider (trigger)
- OnTriggerEnter: IDamageable check + TakeDamage + Return to pool
- IDamageable interface: TakeDamage(float) + IsDead

DamageReceiver.cs (IDamageable implement):
- _maxHealth, _current
- OnDied event
- Pool'a return ya da Destroy
```

---

## 🐛 Debug / fix

```
Console'da şu hata var: [HATA YAPIŞTIR]

Kök sebebi bul:
1. Stack trace neresi gösteriyor?
2. Hangi obje/component crash oluyor?
3. Reproduce adımları nedir?
4. En az invasive fix nedir?

Fix uygula, neden çalıştığını 1 cümle açıkla.
```

---

## 🎨 UI / Health Bar

```
World-space Health Bar prefab yarat (Cookbook §3):
- Canvas (World Space), Image (Background), Image (Fill, Filled type)
- HealthBar.cs script: Bind(target, health), LateUpdate'te follow + camera-facing
- Player'ın üstünde 2 unit yukarıda görünsün
- Health TakeDamage çağrılınca fill azalır
```

---

## 📝 Commit

```
git add -A
git commit -m 'feat(units): character composition with health, mover, combat sub-components

- Character MonoBehaviour holds CharacterStats SO + delegates to Health/Mover/Combat
- Health: max-bounded, OnDied event
- Mover: NavMesh-driven, speed from stats
- Combat: trigger-based, applies stats.Damage to IDamageable
'
```

> **Co-Authored-By: Claude** satırı opsiyonel. Şirketin AI policy'si net değilse veya görüşmeci AI iz bırakmasını sevmiyor görünüyorsa **çıkar**. Pozitif görenler için: `Co-Authored-By: Claude <noreply@anthropic.com>` ekle.

---

## 🚀 "Mevcut kodu refactor et"

```
Şu kodu refactor et:
[kodu yapıştır]

Hedefler:
- Composition (inheritance varsa kır)
- SerializeField private (public field varsa)
- Update'te cache (FindObjectOfType / GetComponent varsa Awake'e al)
- Magic number'ları const'a çıkar
- Açıklayıcı naming

Sadece kod değişikliği, mimari yeniden yazma yok.
```

---

## 💬 "Bunu görüşmeciye nasıl açıklarım?"

```
Şu kodu yazdım, görüşmeci "neden böyle?" diye sorabilir:
[kod]

Bana 3 cümlede açıklamamı sağlayacak özet ver:
1. Ne yaptın (1 cümle, teknik)
2. Neden (1 cümle, trade-off)
3. Alternatif (1 cümle, "olsa şunu yapardım")
```

---

## 🔚 Test sonu özet

```
Şu ana kadar yapılanlar:
- git log --oneline
- Sahnedeki ana GameObject'leri listele (Unity MCP)
- Yazılan script dosyalarını listele

Bunları 60 saniyede sunabileceğim formatta özetle:
- Mimari kararlar (3 madde)
- Çalışan akış (1 cümle)
- Eksikler / improvements (3 madde)
```
