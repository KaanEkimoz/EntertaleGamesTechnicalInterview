# 45 Dakika Test Stratejisi

> Görüşme **canlı + ekran paylaşımlı** + AI serbest. AI'ı "kullanan ama düşünen" geliştirici olarak görünmek = puan.

---

## İlk 5 dakika — yön belirleme

1. **Görevi tekrarla** — "Anladığım kadarıyla X yapıyorum, Y kısıtla, Z çıktıyla. Doğru mu?"
   - Yanlış anlamayı baştan düzelt; 30 dk yanlış yönde gidip telafi etmektense
2. **Scope çek** — "MVP olarak şunu hedefliyorum, kalan zamanda şunu eklerim"
   - Görüşmeci scope'a saygıyı görür
3. **Plan yaz** — terminal'de yorum/notepad'de 3-5 madde:
   ```
   1. Character class + composition (Health, Mover)
   2. ScriptableObject CharacterStats
   3. Sahnede 1 Player + 1 Enemy spawn
   4. Click-to-move
   5. Damage on collision
   ```

---

## 5-35 dk — uygulama

### AI ile etkili çalışma

| Yapma | Yap |
|---|---|
| "Bu oyunu yap" gibi vague prompt | "Character class'ı yarat: composition pattern, Health (max 100), Mover (speed 5). Namespace MyGame.Units. Cookbook §1'den ilhamla." |
| AI çıktısını sessizce kopyala-yapıştır | "AI şunu önerdi, _ile başlayan field doğru ama generic interface eksik, ekleyelim" — sesli yorum |
| Hata olunca AI'a "fix it" | "Hata şu, kök sebep [şu], düzeltme [şu]. Şimdi şu satırı değiştirelim." |
| Aynı task'i 3 kez prompt | İlk çıktıyı 80% kabul et, manuel polish yap |

### Görüşmeciye sesli düşün

- "Burada singleton kullanmıyorum çünkü test edilemez, GameServices locator var"
- "Bu profile etmem lazım ama hızlı bir şey için Update'te cache ediyorum, profiler aşamasını şimdi atladım"
- "Object pool ekleyebilirim ama scope'u büyütür, MVP sonrası eklerim"

### Hız yedekleri

- **Sahne kurulumu** Unity MCP ile 30 saniye, manuel 5 dakika
- **Boilerplate script** (MonoBehaviour iskeleti) Claude'a 1 prompt
- **Component bind** Inspector'dan vs. `execute_code` SerializedObject patch — 2-3 bind ise Inspector hızlı, 10+ bind ise script

---

## 35-40 dk — toparlama

- **Test play** — Play tuşu, gerçekten oynanıyor mu kontrol et
- **Bug varsa fix** — kalan zaman içinde MVP path çalışsın
- **Code review** — AI'a "bu kodun zayıf yanları ne?" diye sor → görüşmeciye senin not aldığın iyileştirmeleri söyle
- **Git commit** — conventional commits, commit'leri sonradan göstermek profesyonel

---

## 40-45 dk — sunum

1. **Play et + ekranda göster** — "Ana akış şu, bu çalışıyor"
2. **Mimari özet** — "Character = composition, CharacterStats = SO, Click = NavMesh"
3. **Trade-off'lar** — "Object pool eklemedim çünkü scope, kalan zamanda olsa şunu yapardım"
4. **AI kullanım açıklaması** — "Boilerplate'i AI'a yaptırdım, business logic'i ben yazdım, edge case'leri tek tek doğruladım"

---

## "Bilmiyorum" demek

Bilmediğin bir API/feature gelirse:
- **Yalan söyleme** — "Bunu kullanmadım, hızlıca docs'a bakıp deneyebilir miyim?"
- WebSearch / Unity docs aç, 30 sn'de pattern bul, uygula
- Görüşmeci "araştırma + öğrenme" hızını da ölçer

---

## "Yanlış yön gittim" anı

Test'in 25. dakikasında çalışmıyor / kötü tasarım fark ettin:
- **Geri al** — "Burada şunu farkettim, [sebep], 5 dk'da [pivot] yapıyorum"
- Mevcut kodu sıfırlama; pivot eden parçayı yeniden yaz, geri kalan korunsun
- Görüşmeci "yanlış yönü tanıyıp düzelten" davranışı pozitif görür

---

## Görüşmeci'nin sevdiği davranışlar

✅ Sesli düşünme
✅ Trade-off açıklama
✅ "Bunu yapmam çünkü [sebep]" netliği
✅ AI çıktısını eleştirme
✅ Test edilebilir kod (interface, DI)
✅ Naming + namespace tutarlılığı
✅ Conventional Commits
✅ "Burası eksik, kalan zamanda eklerim" honestly

---

## Görüşmeci'nin sevmediği davranışlar

❌ AI'a 10 prompt + körü körüne kopyala
❌ Singleton heryerde
❌ Update'te `FindObjectOfType`
❌ `public field` (encapsulation yok)
❌ Magic numbers (`if (hp < 50)` yerine `if (hp < CriticalThreshold)`)
❌ Sessiz çalışma — düşünce sürecini paylaşmama
❌ Scope'u kontrol edemeden başlamak
❌ "Bilmiyorum" demek yerine yalan

---

## Şarj listesi (test günü öncesi gece)

- [ ] Unity Hub güncel
- [ ] Test projesi sanity-check (`project-bootstrap.md` §6)
- [ ] Claude Code çalışıyor (`claude --version`)
- [ ] MCP bağlı (`claude mcp list`)
- [ ] Internet stable (Ethernet > WiFi)
- [ ] 2. monitor varsa Editor + terminal split
- [ ] Mikrofon + kamera test
- [ ] Su + kahve hazır
- [ ] Bu kit Desktop'tan açık (cookbook + patterns) yan ekran

İyi şanslar Kaan. 🎯
