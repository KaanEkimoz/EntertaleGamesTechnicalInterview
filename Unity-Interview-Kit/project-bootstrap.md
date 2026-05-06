# Boş Unity Projesi Bootstrap

Test günü öncesi tek seferlik kurulum. Tahmini süre: 30-45 dk (paket indirme dahil).

---

## 1. Proje oluşturma (5 dk)

1. Unity Hub → **New Project**
2. Editor versiyonu: **Unity 6 LTS** (6000.x.x). Yoksa Hub → Installs → Add → "6000.x LTS"
3. Template: **Universal 3D** (URP)
4. Name: `MyGame` veya kendi seçtiğin
5. Location: `~/Unity Projects/` veya başka stable path (boşluk içermesin → CLI rahat olsun)
6. **Create**

İlk açılış 2-3 dk sürer (paket resolve + initial compile).

---

## 2. Standart paketler (10 dk)

Window → Package Manager → Unity Registry → şunları install et:

| Paket | Sebep |
|---|---|
| **Input System** | Yeni input standardı; "Active Input Handling" sorulursa **Both** seç |
| **TextMeshPro** | UI label'lar; ilk kullanımda Window → TextMeshPro → Import TMP Essentials |
| **Cinemachine** | Kamera ihtiyacı çıkarsa hızlı çözüm |
| **Recorder** | Bonus — playable demo kaydı |

İlk açılış sonrası **Console temiz** mi kontrol et. Sarı warning OK, kırmızı error olmamalı.

---

## 3. Unity MCP kurulumu (5 dk)

Window → Package Manager → **+** → Add package from git URL:

```
https://github.com/CoplayDev/unity-mcp.git?path=/UnityMcpBridge#v9.6.6
```

> **Versiyon-pin önemli** — `#v9.6.6` etiketi olmadan upstream `main`'e çeker, mülakat günü breaking change beklenebilir. Tag listesi: github.com/CoplayDev/unity-mcp/tags

Yüklendikten sonra:
- Window → **MCP For Unity** → **Start Server**
- Server status: "Listening on http://127.0.0.1:8080/mcp"

Terminal'de:
```bash
cd ~/Unity\ Projects/MyGame
claude mcp list
```
Beklenen: `UnityMCP ✓ Connected`

---

## 4. Proje root dosyaları

### `CLAUDE.md` (Claude'a kurallar)

Bu kit'in `CLAUDE.md`'sini projenin root'una **kopyala** (Assets/ değil, bir üst klasör — `MyGame/CLAUDE.md`).

### `.gitignore` (Unity standart)

Proje root'unda:

```gitignore
# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Mm]emoryCaptures/
[Uu]serSettings/

# Visual Studio / Rider
.vs/
.idea/
*.csproj
*.unityproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# OS
.DS_Store
Thumbs.db

# Unity-specific
sysinfo.txt
*.apk
*.aab
*.unitypackage
*.app
crashlytics-build.properties
.gradle/

# Asset cache backups
.git.bak/
```

### `Assets/_Project/` klasörü

Kendi kodunu Asset Store paketlerinden ayırmak için:

```
Assets/
├── _Project/
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Materials/
│   └── ScriptableObjects/
└── Settings/  (Unity default)
```

---

## 5. İlk commit (5 dk)

```bash
cd ~/Unity\ Projects/MyGame
git init
git add .gitignore CLAUDE.md
git commit -m "chore: initial gitignore + Claude rules"
git add ProjectSettings/ Packages/ Assets/
git commit -m "chore: initial Unity 6 LTS URP scaffold"
git log --oneline
```

**GitHub'a push (opsiyonel):** `gh repo create MyGame --private --source=. --push`

---

## 6. Sanity check (5 dk)

Test günü sabahı her sefer kontrol:

- [ ] Unity Hub açılıyor, proje listede
- [ ] Proje açılıyor, Console error'suz
- [ ] **Play tuşu → Stop** çalışıyor (1 saniye)
- [ ] Window → MCP For Unity → Server "Running"
- [ ] Terminal → `claude mcp list` → UnityMCP ✓
- [ ] Test komut: `claude` aç → "boş sahneye küp ekle, kırmızı yap" → MCP'den çalıştığını gör
- [ ] VS Code / Rider script açma test: bir `.cs` dosyasına çift tıkla → editor açılıyor mu
- [ ] Console error count = **0** (sarı warning OK)
- [ ] Editor → Play → 1 saniye bekle → Stop → hâlâ error yok

---

## 7. Hızlı slash command'lar (opsiyonel ama hızlandırıcı)

`~/.claude/commands/unity-feature.md`:

```markdown
Unity'de yeni bir feature ekle.

Adımlar:
1. Plan: 2 cümle
2. Script(ler) Assets/_Project/Scripts/<Feature>/ altına Write
3. Sahnede gerekli GameObject'leri Unity MCP ile ekle
4. Component bind (SerializedObject)
5. read_console error check
6. git commit (conventional, `feat(<scope>):`)

Argüman: $ARGUMENTS
```

Test sırasında `/unity-feature object pooling for projectiles` ile bir komuta tetikleme.
