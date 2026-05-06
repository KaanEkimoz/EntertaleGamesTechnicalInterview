# CLAUDE.md

Bu dosya proje root'unda olduğunda Claude Code otomatik okur. Aşağıdaki bağlam ve tercihlere uy.

---

## Bağlam

- **Geliştirici:** Yusuf Kaan Usta (Türkçe konuşuyor)
- **Ortam:** Canlı 45 dakikalık Unity teknik görüşmesi, ekran paylaşımı açık. Görüşmeci hem kodu hem AI etkileşimini izliyor.
- **Engine:** Unity 6 LTS (6000.x), Universal Render Pipeline (URP), 3D
- **AI kullanımı serbest** — beklenti AI'ı verimli kullanıp profesyonel kod çıkarmak
- **Hedef:** Verilen görevin MVP'sini 45 dk'da temiz + açıklanabilir biçimde teslim et

## Dil

- **Sohbet, açıklama, plan:** Türkçe
- **Kod, isimler, kod-içi yorum, commit mesajı, console log:** İngilizce
- **Görüşmeciye ne yaptığını anlatırken** Kaan kanal — sen Türkçe özet ver, o aktarır

## En iyi performans modu

Test bağlamı seni daha katı kılmamalı, sadece daha **net** kılmalı:
- Adım atmadan önce **plan kur** (1-2 cümle), gerekirse görüşmeciye sunulabilecek format
- Tool çağrıları öncesi **ne yaptığını söyle**, sonrasında **ne oldu** özet
- Hata olursa **kök sebep** + **fix** + **kanıt** üçlüsünü tek mesajda ver
- Belirsizlik varsa **soru sor**, varsayım yapma — yanlış varsayım 5 dk telef eder
- Cevap verirken **uzunluk minimal**, gereken her şey, fazlası değil

## Genel kod kalite çubuğu

Bu kit hiçbir kişisel/şirket convention'ı dayatmaz. Aşağıdakiler **Microsoft .NET / Unity ekosistemi yaygın varsayılanı** — görüşmeci kendi convention'ını söylediği anda **anında ona geç**, kitap-uyma yapma.

- Inspector'dan görünmesi gereken field → `[SerializeField] private` (encapsulation)
- `public` field yerine property — read-only ise `public T Foo => _foo;`
- Naming default'ları (Microsoft style guide):
  - Public API: `PascalCase`
  - Private field: `_camelCase` (yaygın varyant; bazı şirket `m_camelCase` veya plain `camelCase` ister — sor)
  - Local / parametre: `camelCase`
- `Update`/`FixedUpdate`/`LateUpdate`'te `FindFirstObjectByType` / `FindAnyObjectByType` (Unity 6 modern, eski `FindObjectOfType` deprecated), `GetComponent`, `Camera.main`, `Resources.Load` **yapma** — `Awake`/`Start`'ta cache et
- `var` sadece sağdan tip apaçık belliyse (`new Foo()`, `GetComponent<T>()`)
- Magic number'ları `const` veya `[SerializeField]` field'a çıkar
- Namespace kullan — kök ad şirket/proje varsa onu, yoksa görüşmeciye sor
- Bir public class = bir dosya
- `Debug.Log` geliştirme amaçlı; commit öncesi temizlenir. `Debug.LogError`/`LogWarning` gerçek anomali için

> **Convention çatışması anı:** Görüşmeci "biz `m_field` kullanıyoruz" derse, anında o satırdan itibaren `m_field` kullan. Önceki kodu da hemen rename et — geri dönmek çok pahalı değil. Tutarlılık > kişisel tercih.

## Mimari tercihler (yumuşak öneriler)

Görev küçükse over-engineering yapma. Görüşmeci ısrar etmediği sürece **en basit çalışan çözümü** seç. Aşağıdakiler scope büyürse dene-edilecek pattern'ler — dayatma değil:

- **Composition** > deep inheritance — bir `MonoBehaviour` + alt davranışlar **plain C# class** (Awake'te new). MonoBehaviour sadece Unity callback'i gerekiyorsa (OnTriggerEnter, animation event proxy vs.)
- **ScriptableObject** designer-tweakable veri için (stats, config, event channel)
- **Static service locator** veya DI > klasik singleton
- **Object pool** sık spawn/despawn olan obje için (projectile, enemy)
- **Event** sistemler arası gevşek bağlama (C# event lokal, SO channel cross-system)
- **Interface** test edilebilirlik için (`IDamageable`, `IPickup`, ...)

> Görüşmeci "MonoBehaviour'la basit yap" derse — composition'ı bırak, MonoBehaviour'a yaz. "Singleton yap" derse — singleton yaz. Görüşmecinin direktifi > kit önerisi.

## Tool tercih sırası

1. **Unity MCP** (`mcp__UnityMCP__*`) — sahne, GameObject, component, asset, prefab, ScriptableObject operasyonları için **birinci tercih**
2. **Native Read/Write/Edit** — script (.cs) dosyaları için (MCP recompile tetiklediğinden Edit tool çoğu zaman daha hızlı)
3. **Bash** — git, mkdir, mv, ls, klasör navigasyonu
4. **WebSearch / WebFetch** — Unity API docs, hata mesajı araması

Bağımsız iş varsa **paralel tool call** yap (tek mesajda birden fazla tool çağrısı).

## Plan-uygula döngüsü

- Plan tartışıldıysa → **uygula, küçük kararlarda izin sorma**, sonunda 2-3 madde özet
- Scope dışı / destructive işlem (file delete, force push, dependency uninstall, sahne reset) → **dur, sor**
- Görev sırasında yan iş gördüysen ("şurada warning var") **bahset ama dokunma** — ayrı commit yap istenirse

## Git

- Conventional Commits: `feat:`, `fix:`, `docs:`, `refactor:`, `chore:`, `test:`, `perf:`, `style:`
- Her mantıksal iş bitti → anında commit, biriktirme
- **Amend yok** — küçük tweak bile yeni commit
- Push sadece açık talimatla (görüşme demosunda commit history önemli olabilir)

## Edge case davranışları

- **Soru anlaşılmadıysa** — küçük netleştirme sor, "şuyu mu yoksa şunu mu" netleştir
- **Bir tool fail ederse** — alternatif yol dene veya görüşmeciye bilgi vermek için Kaan'a haber ver
- **Compile error** — `read_console` sonrası kök sebebi 1 cümlede özetle, fix tek atımda
- **Süre azalıyor (35+ dk)** — toparlama moduna geç: bug fix > yeni feature, MVP path çalışsın
- **Görüşmeci sözlü soru sorarsa** — Kaan'ın aktarımına göre cevap, doğrudan yorum yapma
