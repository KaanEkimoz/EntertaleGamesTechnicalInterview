# Unity MCP Tool Guide (Coplay v9.6.6+)

> Hangi tool'u ne için, AI'a (Claude) hangi prompt'la kullandırırsın.

---

## Sahne / Hierarchy

| Tool | Ne yapar | Örnek |
|---|---|---|
| `manage_scene` (action="get_active") | Aktif sahne adı + path | "Aktif sahneyi söyle" |
| `manage_scene` (action="get_hierarchy") | Root + child ağacı | "Root GameObject'leri listele" |
| `manage_scene` (action="load") | Sahne yükle (single/additive) | "Test_Scene.unity'i yükle" |
| `manage_scene` (action="save") | Mevcut sahneyi kaydet | "Sahneyi kaydet" |
| `find_gameobjects` | İsim/tag/component'a göre ara | "HudCanvas isimli GameObject'i bul" |

---

## GameObject CRUD

| Tool | Ne yapar |
|---|---|
| `manage_gameobject` (action="create") | Yeni GameObject + primitive (cube/sphere/empty) |
| `manage_gameobject` (action="modify") | Pozisyon/rotation/scale/parent değiştir |
| `manage_gameobject` (action="delete") | Sil |
| `manage_gameobject` (action="duplicate") | Kopyala |
| `manage_components` | Component ekle/sil/property set |

**Örnek prompt:**
```
"Sahneye 'Player' adında boş GameObject yarat, position (0,1,0). Üstüne Rigidbody + CapsuleCollider + PlayerController ekle."
```

---

## Asset CRUD

| Tool | Ne yapar |
|---|---|
| `manage_asset` (action="create") | Material, ScriptableObject, AnimatorController vs. |
| `manage_asset` (action="search") | Asset Database'de ara |
| `manage_asset` (action="move") | Asset taşı (.meta GUID korunur) |
| `manage_asset` (action="delete") | Asset sil |

---

## Scriptable Object

| Tool | Ne yapar |
|---|---|
| `manage_scriptable_object` (action="create") | SO instance yarat |
| `manage_scriptable_object` (action="modify") | Field değerleri set et (patches array) |

**Örnek:**
```
"CharacterStats SO yarat, _maxHealth=100, _moveSpeed=5, _damage=15. Path Assets/_Project/SO/Swordsman.asset"
```

---

## Script

| Tool | Ne yapar |
|---|---|
| `manage_script` (action="create") | Yeni .cs dosya |
| `manage_script` (action="modify") | Mevcut script'i edit |
| `apply_text_edits` / `script_apply_edits` | Çoklu surgical edit |
| `find_in_file` | Script içinde regex ara |
| `read_console` | Compile error / runtime log |

⚠️ Çoğu zaman **native Read/Write/Edit daha hızlı** (.cs dosya editlemek). MCP script edit'i Unity Editor recompile tetiklediğinden yavaş — büyük edit'lerde Edit tool kullan, sonra `refresh_unity` ile compile tetikle.

---

## Compile / Console

| Tool | Ne yapar |
|---|---|
| `refresh_unity` (mode="force", compile="request") | Asset DB refresh + compile |
| `read_console` (types=["error"]) | Hata mesajları |
| `read_console` (action="clear") | Console temizle |

⏱ Compile sonrası 15-30s domain reload. Bash sleep 25 + run_in_background:true ile bekle.

---

## Editor kontrol

| Tool | Ne yapar |
|---|---|
| `manage_editor` (action="play") | Play mode'a geç |
| `manage_editor` (action="stop") | Play mode'dan çık |
| `manage_camera` (action="screenshot") | Game View screenshot (UI Overlay yakalamaz!) |

⚠️ **Canvas Screen Space Overlay** screenshot alamıyor — geçici Screen Space Camera moduna geçirip al, sonra revert.

---

## Direkt Editor C# çalıştırma

| Tool | Ne yapar |
|---|---|
| `execute_code` (action="execute") | Editor'de raw C# çalıştır (UnityEditor + UnityEngine namespace açık) |

**Süper güçlü** — bir prompt'la 10 işi yapabilir:
```csharp
// Sahnedeki tüm Character'ların _maxHealth'ını 200 yap
foreach (var character in UnityEngine.Object.FindObjectsByType<Character>(FindObjectsSortMode.None)) {
    var so = new UnityEditor.SerializedObject(character);
    so.FindProperty("_maxHealth").floatValue = 200f;
    so.ApplyModifiedPropertiesWithoutUndo();
}
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(...);
```

---

## Tipik akış (test sırasında)

1. **Plan** (sözlü, 1 cümle): "Character yaratacağım, Health + Mover bileşenleri ile"
2. **Script create** (Write tool, native): `Assets/_Project/Scripts/Character.cs`
3. **Compile** (`refresh_unity` + sleep)
4. **Console check** (`read_console` errors)
5. **Sahnede instantiate** (`manage_gameobject create` + `manage_components add`)
6. **Bind fields** (`execute_code` ile SerializedObject patch)
7. **Save scene** (`manage_scene save`)
8. **Test play** (`manage_editor play`)
9. **Commit** (Bash git)

---

## "Tool çağrılarını paralel yap" — hız

Bağımsız işleri tek mesajda paralel çağır:

```
Aynı anda:
- manage_scene get_hierarchy
- find_gameobjects search="Camera"
- manage_asset search="t:Material"
```

3 ayrı response yerine 1 → 3x hızlı.
