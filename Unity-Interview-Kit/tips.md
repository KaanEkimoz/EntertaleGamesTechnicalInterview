# tips.md — Mülakat Saatinde Yan Ekran Cheat Sheet

> 45 dk canlı + ekran paylaşımlı + AI serbest. Tek ekranda görüsünür kalsın. Tarama amaçlı, okumak için değil.

---

## 0. İlk 60 saniye — kalibrasyon

1. **Görevi tekrarla** sözlü: "Anladığım: X yapacağım, kısıt Y, çıktı Z. Doğru mu?"
2. **Scope çek**: "MVP olarak şu, vakit kalırsa şu — sizce öncelik?"
3. **Plan yaz** (terminal yorumu / Notes): max 5 madde
4. Bunları **göster ekrandan** — görüşmeci scope farkındalığını görür

Kural: scope onayı almadan kod yazma. Scope onayı = en pahalı 2 dakika ama 30 dk kurtarır.

---

## 1. Zaman bütçesi (45 dk)

| Süre | Faz | Çıktı |
|---|---|---|
| 0-5 | Anlama + plan | Görevi tekrar et, scope çek, planı yaz |
| 5-15 | İskele | Character / class / sahne objeleri (MVP path) |
| 15-30 | Çekirdek | İstenen davranış çalışıyor (MVP green) |
| 30-38 | Polish + bug fix | Test play, gözle gör, kritik bug fix |
| 38-43 | Sunum hazırlığı | Git log temizle, ne anlatacağını kafanda topla |
| 43-45 | Sunum | Play et, anlat, sor |

**30. dk hatırlatması**: yeni feature ekleme. Yarım kalan ölü kod = eksi puan. Tamam bitti = artı.

---

## 2. "Yapma" listesi (sık yapılan hatalar)

- AI'ın ürettiği koda bakmadan **next prompt**'a geçmek
- Aynı soruyu 3 kez farklı promptla AI'a sormak (ikinciden sonra manuel yaz)
- Sessiz çalışmak — görüşmeci 30 sn sessizlikten sonra panikler
- 25. dakikada hâlâ "doğru mimariyi" arıyor olmak — MVP path'e dön
- `Camera.main`, `FindObjectOfType`, `GetComponent` Update içinde
- `public field` (SerializeField + private property)
- Compile error'ı yokmuş gibi devam etmek — `read_console` ile bak
- `manage_script` ile büyük .cs edit (yavaş) — Edit/Write tool kullan, sonra `refresh_unity`
- Conventional Commit yazmadan commit (`feat:`, `fix:` koy)
- "Bilmiyorum" yerine yalan — "kullanmadım, hızlıca docs'a bakıyorum" daha güvenli

---

## 3. Karar tablosu (tetikleyici → aksiyon)

| Eğer... | O zaman... |
|---|---|
| Görev belirsiz | Görüşmeciye 1 netleştirme sorusu sor (max 2) |
| Scope çok büyük | "MVP olarak X, kalan zamanda Y" diyerek küçült |
| Aynı pattern 2+ kez yazıyorum | Cookbook'tan ilgili bölüme referans ver: `@unity-cookbook.md §N` |
| AI yanlış yön gidiyor | Dur. Manuel 5 satır yaz, AI'a "şu satırı temel al" de |
| Compile error | `read_console` → kök sebebi 1 cümlede özetle → tek atımda fix |
| 25+ dk geçti, çalışmıyor | Pivot: en küçük çalışan parçaya geri dön, üstüne ekleme |
| 35+ dk, MVP yarım | Yeni feature dur. Mevcut bug'ı fix, sahneyi göster |
| Sahnede 5+ obje bind | `execute_code` ile SerializedObject patch — manuel Inspector yapma |
| Sahnede 1-2 obje bind | Inspector hızlı, MCP overkill |
| Görüşmeci sözlü soru sordu | Önce cevapla, sonra koda dön. Asla göz kırpma |
| AI'a brief vermek 30 sn'den uzun sürecek | Manuel yaz, daha hızlı |
| `public class Foo : Bar` yazıyorum | Dur. Composition mı daha iyi? Cevap evetse component yap |

---

## 4. Sözlü açıklama formülleri (görüşmeciye söyle)

### Plan açıklarken
- **TR:** "Şunu hedefliyorum: [MVP], [scope-cut sebebi]. Vakit kalırsa [stretch] eklerim."
- **EN:** "I'm aiming for [MVP] first; I'll skip [stretch] unless time allows. Sound right?"

### Karar/trade-off söylerken
- **TR:** "Burada [pattern X] kullanıyorum çünkü [sebep]. [Pattern Y]'yi tercih etmedim çünkü [sebep] — bu scope için overkill."
- **EN:** "I'm using [X] here because [reason]. I skipped [Y] — overkill for this scope, but I'd add it if [condition]."

### AI çıktısını eleştirirken (yüksek puan getirir)
- **TR:** "AI bu önerdi. Şu kısmı OK ama [şu] yanlış / eksik — düzeltiyorum."
- **EN:** "AI gave me this — the [part] is fine, but [issue]. Let me fix that piece."

### Bilmediği şey gelince
- **TR:** "Bunu hiç kullanmadım. 30 saniye docs'a bakıp deneyeyim mi?"
- **EN:** "I haven't used this API. Let me check docs for 30 seconds and try."

### Pivot ederken
- **TR:** "Şunu farkettim — [problem]. 5 dakikada [pivot] yapıyorum, mevcut kod %80 korunur."
- **EN:** "I realized [issue]. Pivoting now — 5 min cost, most existing code stays."

### Eksik bırakırken
- **TR:** "Object pool eklemedim, scope dışı. Production'da [şu hot path] için pool şart."
- **EN:** "Skipped object pooling — out of scope. In production this hot path needs it."

### Bug fix'lerken (kök sebep formülü)
- **TR:** "Hata: [şu]. Kök sebep: [şu]. Fix: [şu]. Test: [şu]."
- **EN:** "Error: [X]. Root cause: [Y]. Fix: [Z]. Verified by: [test]."

### Sunum açılış cümlesi
- **TR:** "60 saniyede özet: [mimari] kullandım, [çalışan akış], [eksikler ve neden]."
- **EN:** "60-second summary: I went with [architecture], here's the working flow, and these are the trade-offs I made."

---

## 5. AI'ı verimli kullanma

### Brief formatı (AI'a verilen prompt)
```
Görev: [tek cümle]
Pattern: cookbook §N (ya da: [örnek pattern adı])
Namespace: MyGame.[Feature]
Constraints: SerializeField private, no FindObjectOfType, composition
Çıktı: tek dosya, [class adı]
```

### Manuel yaz vs AI'a yaptır
| Manuel | AI |
|---|---|
| 5-15 satır boilerplate | 30+ satır class iskeleti |
| Bug fix (lokasyon belirli) | Yeni pattern uygulama |
| Naming/refactor | Test sahne kurulumu |
| Tek satır değişiklik | Animator transition kurulumu |
| Hızlı düşünme gereken karar | Multi-file orchestration |

**Pratik kural:** Yazılacak kod 10 satırdan az ise manuel hızlıdır. Üstüyse AI brief.

### AI body language (görüşmeci izliyor)
- Promptu **tek seferde net** ver, soru-cevap tenisi yapma
- Çıktı geldikten sonra **3 saniye gözle tara** — sonra Apply / Reject
- "Bu hata var, düzelt" yerine "Şu satır yanlış çünkü X, şöyle olsun" — sebep + çözümle
- AI yanlış gittiyse **iki promptta düzelt, üçüncüde manuel yaz**

### Paralel tool call (hız 3x)
Bağımsız iş varsa tek mesajda 3-4 tool çağır. Senior göstergesi.

---

## 6. Ses tonu ve iletişim

- **Düz ton, net cümle**, mırıldanma. Sessizlikten iyi.
- Her 60-90 saniyede 1 cümle düşünce paylaş — "Şimdi şunu yapıyorum çünkü..."
- "Galiba", "sanırım", "yanlış olabilir" → kullanma. Bilmiyorsan "kontrol edeyim" de.
- Görüşmeci soru sordu → **önce cevapla**, sonra koda dön. Asla "bir saniye" deme.
- Hız ne olursa olsun **panik göstermeme** — yavaş yazıp temiz olmak, hızlı yazıp bug'lı olmaktan iyi.
- Türkçe ana iletişim, kod-içi her şey İngilizce. Mix yapma.

---

## 7. Test bittikten sonra (son 2 dk)

1. **Play tuşuna bas, oynanışı 15 saniye göster** — visible proof.
2. **Mimari özeti** 3 madde:
   - "Composition: Character + Health/Mover/Combat alt sınıflar"
   - "Data: CharacterStats SO, designer-tweakable"
   - "Loose coupling: IDamageable interface"
3. **Trade-off**: "Object pool yok çünkü scope. Production'da X için şart."
4. **AI kullanım**: "Boilerplate AI, business logic ben, edge case'leri tek tek doğruladım."
5. **Eksikler**: "Y test'i eklemedim, Z'yi mock'ladım. Vakit olsa şunu yapardım."
6. Görüşmeciye **soru sor**: "Hangi kısmı daha derinden konuşmak istersiniz?" — kontrolü senin elinde tutar.

---

## 8. Acil durum komutları

### Compile error patladı
```
read_console (errors only) → kök sebebi 1 cümle → tek atımda fix → refresh_unity → tekrar oku
```

### Sahne bozuldu
```
Don't panic. Cmd+Z spam etme. Yeni boş sahne aç, prefab'lardan tekrar kur. 5 dk maliyet.
```

### MCP düştü
```
Window → MCP For Unity → Stop Server → Start Server. claude mcp list ile doğrula. 30 sn.
Düzelmezse: manuel Inspector çalış, MCP'yi unut.
```

### AI sürekli yanlış kod üretiyor
```
Dur. Bir defa "ne istediğimi tam yaz" — tek paragraf, tüm constraint'lerle. Yine yanlışsa manuel.
```

### Süre bitti, MVP yarım
```
Hemen Play tuşuna bas. Çalışan kısmı göster. "Eksikler şunlar, sebep zaman" diye dürüst söyle.
Yarım iş + dürüst > tam iş + yalan.
```

---

## 9. Görüşmecinin bakış açısı (puan kriterleri)

Onun gözünde sen şu kriterlerle değerlendiriliyorsun (yaklaşık ağırlık):

| Kriter | Ağırlık | Nasıl gösterirsin |
|---|---|---|
| Çalışan MVP | %30 | Play et, gözle göster |
| Kod kalitesi (naming, encapsulation, pattern) | %20 | SerializeField private, namespace, composition |
| AI'ı kontrollü kullanma | %15 | Çıktıyı eleştir, brief net, manuel müdahale |
| İletişim / sözlü düşünme | %15 | Trade-off açıkla, plan göster, soru sor |
| Trade-off farkındalığı | %10 | "Yapmadım çünkü scope" + "production'da yapardım" |
| Bug handling / debug | %5 | Kök sebep formülü, panik yok |
| Git workflow | %5 | Conventional commits, atomic |

---

## 10. Son hatırlatma

> **AI = junior'ın hızı, senior'ın aklı.**
> Görüşmeci hız değil, **kontrol** görmek istiyor.
> Sessizlikten ve panikten kaçın. Yavaş ve net en güvenli mod.

İyi şanslar.
