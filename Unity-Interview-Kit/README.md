# Unity Interview Kit — Yusuf Kaan Usta

> **Amaç:** 45 dakikalık canlı Unity teknik testinde kaybedeceğin saniyeyi sıfıra indirmek için hazırlanmış AI-asistanlı geliştirme paketi.
> Test öncesi gözden geçir, test sırasında Claude / ChatGPT / Cursor'a **referans dosya** olarak ver.

---

## İçindekiler

| # | Dosya | Ne işe yarar |
|---|---|---|
| 1 | `CLAUDE.md` | AI asistanına kurallar (bağlam, kod stili, MCP tercih sırası, dil). Boş Unity projesinin root'una kopyala. |
| 2 | `project-bootstrap.md` | Boş proje kurulum adımları — paket listesi, ayarlar, .gitignore, ilk commit. Test günü öncesi tek seferlik. |
| 3 | `unity-cookbook.md` | En sık istenen 12 task'in tam pattern'i — Character/Combat, Health Bar, Object Pool, Animator FSM, EditMode test, OnValidate. Copy-paste hazır. |
| 4 | `gameplay-patterns.md` | Unity mimarisi: ScriptableObject Architecture, Static Service Locator, Event Channels, Composition. |
| 5 | `performance-cheatsheet.md` | Update'te yasaklar (Unity 6 modern API), allocation traps, profiler tips, Job System. |
| 6 | `unity-mcp-guide.md` | Unity MCP (Coplay) tool'larının ne işe yaradığı + örnek kullanım. AI'a hangi tool ne için kullanılır. |
| 7 | `interview-strategy.md` | 45 dk içinde nasıl ilerlersin — ilk 5 dk, sahne kurulumu, debug, sunum. |
| 8 | `quick-prompts.md` | Test sırasında copy-paste hazır 11 AI prompt şablonu. Yan ekranda açık tut. |
| 9 | `tips.md` | **Mülakat saatinde yan ekran cheat sheet** — zaman bütçesi, sözlü cümle kalıpları (TR/EN), AI manuel-vs-prompt karar tablosu, acil durum yedekleri. |

---

## Kullanım akışı

### Test öncesi (1-2 saat)

1. `project-bootstrap.md`'yi izle — boş Unity 6 LTS URP projesi hazırla
2. Bu klasörün TÜMÜNÜ projenin root'una kopyala (örn. `<MyGame>/Docs/`)
3. `CLAUDE.md`'yi projenin root'una koy (Claude Code otomatik okur)
4. `claude mcp list` → `UnityMCP ✓ Connected`
5. Test run: Claude'a "Boş sahneye küp ekle, kırmızı yap" → MCP üzerinden çalıştığını doğrula

### Test sırasında

1. Görev gelince **30 saniye düşün** — soruyu ne istediğini netleştir
2. **Plan yap** — ana adımları kafanda say (1 cümle)
3. Claude terminal'e git → "X yapacağız, Y kullan" şeklinde net brief ver
4. Claude'a `cookbook.md`'deki ilgili pattern'i referans göster gerekirse: "@unity-cookbook.md → Object Pool"
5. AI çıktısını **körü körüne kabul etme** — 5 saniye oku, açık olanı düzelt
6. Görüşmeciye **ne yaptığını açıkla**: "AI'a şunu yaptırdım, çünkü..."

### Test sonrası

- `git log` → commitleri tek tek anlat (eğer git init yaptıysan)
- "Daha fazla sürse şunu eklerdim..." şeklinde extension story

---

## Felsefe

> **AI = junior developer'ın hızı, senior'ın aklı.**

AI'a güveniyorsun ama körü körüne değil. Her output'u 5 saniye gözden geçir. Görüşmeci AI kullandığını görüyor; **nasıl kullandığın** asıl test edilen şey.
