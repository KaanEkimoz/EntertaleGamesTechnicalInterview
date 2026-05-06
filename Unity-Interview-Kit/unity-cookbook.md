# Unity Cookbook — En Sık 10 Görev

> 45 dk testte istenmesi muhtemel pattern'ler. Hepsi tam çalışır, copy-paste hazır.
> AI'a referans verirken: "@unity-cookbook.md → Object Pool" gibi kullan.

---

## 1. Composition tabanlı Character

> **Önemli:** `Health` ve `Mover` sub-behavior'ları **plain C# class** (MonoBehaviour değil). Awake'te `new` ile yaratılır. Unity callback'ine ihtiyaç olan davranışlar (OnTriggerEnter, OnAnimatorMove vs.) ayrı bir MonoBehaviour'a alınır. Tek sorumluluk + test edilebilir.

```csharp
using UnityEngine;

namespace MyGame.Units
{
    public sealed class Character : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private Vector3 _moveInput = Vector3.forward; // dummy direction

        private Health _health;
        private Mover _mover;

        public float MoveSpeed => _moveSpeed;
        public Health Health => _health;

        private void Awake()
        {
            _health = new Health(_maxHealth);
            _mover  = new Mover(transform, _moveSpeed);
            _health.OnDied += HandleDied;
        }

        private void OnDestroy() { if (_health != null) _health.OnDied -= HandleDied; }

        private void Update()
        {
            if (_health.IsDead) return;
            _mover.Tick(_moveInput);
        }

        private void HandleDied()
        {
            // override / replace with VFX, ragdoll, pool return, etc.
            gameObject.SetActive(false);
        }
    }

    public sealed class Health
    {
        private float _current;
        public float Current => _current;
        public float Max { get; }
        public bool IsDead => _current <= 0f;
        public event System.Action OnDied;

        public Health(float max) { Max = max; _current = max; }
        public void TakeDamage(float dmg)
        {
            _current = Mathf.Max(0f, _current - dmg);
            if (IsDead) OnDied?.Invoke();
        }
    }

    public sealed class Mover
    {
        private readonly Transform _t;
        private readonly float _speed;
        public Mover(Transform t, float speed) { _t = t; _speed = speed; }
        public void Tick(Vector3 dir) { _t.position += dir.normalized * _speed * Time.deltaTime; }
    }
}
```

---

## 2. Object Pool (generic)

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Core
{
    public sealed class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool = new Queue<T>();

        public ObjectPool(T prefab, int initialSize = 16, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < initialSize; i++) Return(Create());
        }

        private T Create()
        {
            T inst = Object.Instantiate(_prefab, _parent);
            inst.gameObject.SetActive(false);
            return inst;
        }

        public T Get(Vector3 pos, Quaternion rot)
        {
            T inst = _pool.Count > 0 ? _pool.Dequeue() : Create();
            inst.transform.SetPositionAndRotation(pos, rot);
            inst.gameObject.SetActive(true);
            return inst;
        }

        public void Return(T inst)
        {
            inst.gameObject.SetActive(false);
            _pool.Enqueue(inst);
        }
    }
}
```

Kullanım: `_projectilePool = new ObjectPool<Projectile>(_projectilePrefab, 32, transform);`

---

## 3. Health Bar UI (world space, follow target)

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);
        private Transform _target;
        private Health _health;
        private Camera _cam;

        public void Bind(Transform target, Health health)
        {
            _target = target;
            _health = health;
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (_target == null || _health == null) return;
            transform.position = _target.position + _offset;
            transform.forward = _cam.transform.forward;
            _fill.fillAmount = _health.Current / _health.Max;
        }
    }
}
```

---

## 4. Animator FSM — Idle/Walk/Attack/Death

Animator Controller'da 4 state. Parameters: `Speed (float)`, `Attack (trigger)`, `IsDead (bool)`.

Transitions:
- `Idle → Walk`: `Speed > 0.1`
- `Walk → Idle`: `Speed < 0.1`
- `Idle / Walk → Attack`: trigger `Attack`
- `Attack → Idle`: Has Exit Time (1.0)
- `Any State → Death`: bool `IsDead == true`

Script:

```csharp
private static readonly int SpeedHash  = Animator.StringToHash("Speed");
private static readonly int AttackHash = Animator.StringToHash("Attack");
private static readonly int DeadHash   = Animator.StringToHash("IsDead");

private Animator _animator;
private void Awake() => _animator = GetComponent<Animator>();

public void SetSpeed(float v)   => _animator.SetFloat(SpeedHash, v);
public void TriggerAttack()     => _animator.SetTrigger(AttackHash);
public void SetDead(bool dead)  => _animator.SetBool(DeadHash, dead);
```

---

## 5. Damage Animation Event

Attack clip içinde **OnAttackHit** adında Animation Event ekle (hit frame'inde). Receiver script visual GameObject'inde:

```csharp
public sealed class AnimationEventForwarder : MonoBehaviour
{
    private CharacterCombat _combat;
    private void Awake() => _combat = GetComponentInParent<CharacterCombat>();
    public void OnAttackHit() => _combat?.DealDamage();
}
```

CharacterCombat'ta:

```csharp
public void DealDamage()
{
    if (_currentTarget == null || _currentTarget.IsDead) return;
    _currentTarget.TakeDamage(_damage);
}
```

---

## 6. Scriptable Object Stats

```csharp
using UnityEngine;

namespace MyGame.Units
{
    [CreateAssetMenu(menuName = "MyGame/Unit Stats")]
    public sealed class CharacterStats : ScriptableObject
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _damage    = 10f;
        [SerializeField] private float _attackInterval = 1f;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float Damage    => _damage;
        public float AttackInterval => _attackInterval;
    }
}
```

Character'da `[SerializeField] private CharacterStats _stats;` ile bind.

---

## 7. Static Service Locator (singleton yerine)

```csharp
using System;
using System.Collections.Generic;

namespace MyGame.Core
{
    public static class GameServices
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T svc) where T : class => _services[typeof(T)] = svc;
        public static T Get<T>() where T : class
            => _services.TryGetValue(typeof(T), out var s) ? (T)s : null;
        public static bool TryGet<T>(out T svc) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s)) { svc = (T)s; return true; }
            svc = null; return false;
        }
        public static void Clear() => _services.Clear();
    }
}
```

Bootstrap'te: `GameServices.Register<IAudio>(new AudioService());`. Kullanım: `GameServices.Get<IAudio>().Play(clip);`.

---

## 8. ScriptableObject Event Channel

```csharp
using System;
using UnityEngine;

namespace MyGame.Events
{
    [CreateAssetMenu(menuName = "MyGame/Events/Character Died Event")]
    public sealed class CharacterDiedEvent : ScriptableObject
    {
        public event Action<Character> OnRaised;
        public void Raise(Character p) => OnRaised?.Invoke(p);
    }
}
```

Listener:
```csharp
[SerializeField] private CharacterDiedEvent _channel;
private void OnEnable()  => _channel.OnRaised += HandleCharacterDied;
private void OnDisable() => _channel.OnRaised -= HandleCharacterDied;
```

---

## 9. Click-to-move (NavMesh) — Input System (Unity 6 modern)

```csharp
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class ClickToMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Camera _cam;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 1f, NavMesh.AllAreas))
        {
            _agent.SetDestination(nav.position);
        }
    }
}
```

> **Setup:** Player Settings → Active Input Handling = **Both**. Window → Package Manager → Input System (yüklü olduğunu doğrula). NavMesh için zemini Static işaretle → Window → AI → Navigation → Bake.
>
> **Legacy fallback** (Input System paketi yoksa): `Input.GetMouseButtonDown(0)` + `Input.mousePosition`. Görüşmeci hangisini istediğini sormazsa Input System tercih et — modern + senior göstergesi.

---

## 10. Custom Editor / MenuItem tool

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyGame.EditorTools
{
    public static class MyGameMenu
    {
        [MenuItem("MyGame/Spawn 10 Cubes")]
        private static void SpawnCubes()
        {
            for (int i = 0; i < 10; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i * 1.5f, 0f, 0f);
                Undo.RegisterCreatedObjectUndo(cube, "Spawn Cube");
            }
        }

        [MenuItem("MyGame/Snap Selected To Grid")]
        private static void SnapToGrid()
        {
            foreach (var t in Selection.transforms)
            {
                Undo.RecordObject(t, "Snap To Grid");
                Vector3 p = t.position;
                t.position = new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));
            }
        }
    }
}
#endif
```

`Editor/` klasörüne koy — runtime build'a girmez.

---

## 11. EditMode Test (Unity Test Framework)

`Tests/EditMode/HealthTests.cs` (assembly definition gerekirse `Tests.EditMode.asmdef` ile `nunit.framework` referansı):

```csharp
using NUnit.Framework;
using MyGame.Units;

namespace MyGame.Tests
{
    public sealed class HealthTests
    {
        [Test]
        public void TakeDamage_ReducesCurrent()
        {
            var h = new Health(100f);
            h.TakeDamage(30f);
            Assert.AreEqual(70f, h.Current);
        }

        [Test]
        public void TakeDamage_ClampsAtZero_AndFiresOnDied()
        {
            var h = new Health(50f);
            bool died = false;
            h.OnDied += () => died = true;

            h.TakeDamage(80f);

            Assert.AreEqual(0f, h.Current);
            Assert.IsTrue(h.IsDead);
            Assert.IsTrue(died);
        }
    }
}
```

> Window → Test Runner → EditMode → Run All. Plain C# Health class olduğu için Unity'ye gerek yok, testler hızlı.
> "Test yazdın mı?" sorusuna **evet, EditMode'da Health logic'ini test ettim** diyebilmek senior göstergesi.

---

## 12. OnValidate — Inspector sanity-check

```csharp
public sealed class Character : MonoBehaviour
{
    [SerializeField] private CharacterStats _stats;
    [SerializeField] private Transform _projectileSpawn;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null)
            Debug.LogWarning($"[{name}] CharacterStats reference is missing.", this);
        if (_projectileSpawn == null)
            Debug.LogWarning($"[{name}] Projectile spawn anchor is missing.", this);
    }
#endif
}
```

> Inspector'dan field unutulmuş bir referans varsa anında Console'a uyarı düşer. Senior dev'in tipik göstergesi — production'da en sık yaşanan "boş ref crash" buradan kapanır.
