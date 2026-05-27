# Game Focus Guard — Guide pour Claude

Outil Windows minimal pour empêcher les jeux de perdre le focus ou de se mettre en pause en arrière-plan.

---

## Architecture du projet

```
game-focus-guard/
├── FocusTool.sln
├── FocusTool.Ui/          ← Application WinForms .NET 10
│   ├── Form1.cs / Form1.Designer.cs   — UI principale + sélecteur de langue
│   ├── HookController.cs              — Chargement du DLL natif, SetWindowsHookEx
│   ├── HotKeyService.cs               — Enregistrement/gestion du raccourci global
│   ├── HotKeySettingsStore.cs         — Persistance JSON du raccourci
│   ├── HotKeyBinding.cs               — Struct immuable représentant un raccourci
│   ├── AppInstanceCoordinator.cs      — Instance unique (mutex + activation)
│   ├── NativeMethods.cs               — P/Invoke Win32
│   ├── Strings.cs                     — Toutes les chaînes localisées (EN/FR/ZH)
│   ├── Language.cs                    — Enum Language { EN, FR, ZH }
│   ├── LanguageStore.cs               — Persistance de la langue choisie
│   └── HookStatusSnapshot.cs          — Records de snapshot d'état
└── FocusTool.Hook/        ← DLL native C++20 (compilée par MSVC)
    ├── focus_hook.cpp                 — Procédures de hook Windows
    └── Build-Hook.ps1                 — Script de build natif (pwsh)
```

### Flux de données

```
Form1 (UI)
  ├── HookController      → charge FocusTool.Hook.dll, installe les hooks Win32
  │     └── focus_hook.cpp : CallWndProc / CallWndRetProc / GetMsgProc / LowLevelKeyboardProc
  ├── HotKeyService       → RegisterHotKey / UnregisterHotKey
  ├── HotKeySettingsStore → %LocalAppData%\FocusToolPrototype\settings.json
  ├── LanguageStore       → %LocalAppData%\FocusToolPrototype\language.txt
  └── Strings             → propriétés statiques EN/FR/ZH, événement LanguageChanged
```

---

## Prérequis

| Composant | Version minimale | Notes |
|---|---|---|
| Windows | 10 x64 | x64 obligatoire (DLL compilée en x64) |
| .NET SDK | 10.0 | `net10.0-windows` |
| PowerShell | 7+ (`pwsh`) | Pas `powershell.exe` (PS 5) |
| MSVC C++ | VS 2022 (toute édition) | Voir détail ci-dessous |

### Installations MSVC acceptées (dans l'ordre de priorité)

`Build-Hook.ps1` cherche `vcvars64.bat` dans cet ordre :

1. `C:\Program Files\Microsoft Visual Studio\2022\Community\…`
2. `C:\Program Files\Microsoft Visual Studio\2022\Professional\…`
3. `C:\Program Files\Microsoft Visual Studio\2022\Enterprise\…`
4. `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\…` ← fallback

**Installation minimale (BuildTools uniquement) :**
```
vs_BuildTools.exe --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended
```

---

## Build

### Commande standard (Debug)

```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj
```

Ce que fait cette commande :
1. Compile le code C# → `FocusTool.Ui\bin\Debug\net10.0-windows\`
2. MSBuild déclenche automatiquement `Build-Hook.ps1` via `AfterTargets="Build"`
3. Le script localise `vcvars64.bat`, compile `focus_hook.cpp` avec `cl.exe /LD`
4. Copie `FocusTool.Hook.dll` dans le répertoire de sortie du step 1

### Build Release

```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj -c Release
```

### Publish (package redistribuable)

```powershell
dotnet publish .\FocusTool.Ui\FocusTool.Ui.csproj -c Release -r win-x64 --self-contained
```

La DLL native est copiée dans `PublishDir` via `AfterTargets="Publish"`.

### Build du DLL natif seul

```powershell
pwsh -NoProfile -ExecutionPolicy Bypass `
     -File .\FocusTool.Hook\Build-Hook.ps1 `
     -OutputDir .\FocusTool.Ui\bin\Debug\net10.0-windows
```

### Exécution après build

```powershell
# Doit être lancé en tant qu'Administrateur
.\FocusTool.Ui\bin\Debug\net10.0-windows\FocusTool.Ui.exe
```

> **Administrateur requis** : `SetWindowsHookEx` (hooks thread) et `RegisterHotKey` nécessitent des privilèges élevés dans certains contextes.

---

## Erreurs de build courantes

| Erreur | Cause | Solution |
|---|---|---|
| `vcvars64.bat introuvable` | MSVC non installé | Installer VS 2022 Build Tools avec la charge C++ |
| `pwsh : command not found` | PowerShell 7 absent | Installer depuis `winget install Microsoft.PowerShell` |
| `FocusTool.Hook.dll not found` | Build du DLL échoué | Vérifier la sortie de `Build-Hook.ps1` ; relancer le build |
| `Native hook DLL not found` (runtime) | DLL absente du répertoire EXE | Relancer `dotnet build` pour déclencher la copie |
| Erreur `cl.exe` | Mauvaise architecture | Vérifier que `vcvars64.bat` est utilisé (pas `vcvars32`) |

---

## Localisation

### Ajouter une chaîne

Toutes les chaînes visibles par l'utilisateur sont dans `Strings.cs`. Structure type :

```csharp
public static string MaChaine => Current switch
{
    Language.FR => "Texte en français",
    Language.ZH => "中文文本",
    _           => "English text"   // Language.EN = default
};
```

### Ajouter une langue

1. Ajouter la valeur dans `Language.cs` :
   ```csharp
   public enum Language { EN, FR, ZH, DE }  // exemple
   ```
2. Ajouter un `case` dans **chaque** propriété de `Strings.cs`
3. Ajouter l'item dans `languageComboBox.Items.AddRange(...)` dans `Form1.Designer.cs`
4. Ajouter le mapping index↔langue dans `languageComboBox_SelectedIndexChanged` et `Form1_Load` dans `Form1.cs`

### Persistance de la langue

Fichier : `%LocalAppData%\FocusToolPrototype\language.txt`
Contenu : `EN`, `FR` ou `ZH` (valeur de l'enum en texte)

---

## Structure des hooks natifs (`focus_hook.cpp`)

| Export | Hook Win32 | Rôle |
|---|---|---|
| `SetupHooks` | — | Initialise `InitCommonControlsEx`, enregistre le message custom |
| `SetGlobalHookValues` | — | Définit la cible (`HWND`, `PID`, `TID`), active/désactive |
| `ResetWindowState` | — | Nettoie les props et le sous-classement sur la fenêtre cible |
| `CallWndProc` | `WH_CALLWNDPROC` | Intercèpte les messages de perte de focus (`WM_ACTIVATE`, `WM_KILLFOCUS`…) et installe un sous-classement one-shot |
| `CallWndRetProc` | `WH_CALLWNDPROCRET` | Pass-through (réservé pour extensions futures) |
| `GetMsgProc` | `WH_GETMESSAGE` | Pass-through avec garde `kIgnoreGetMsgProp` |
| `LowLevelKeyboardProc` | `WH_KEYBOARD_LL` | Avale Alt+Tab quand la fenêtre cible est au premier plan |

Les hooks thread (`WH_CALLWNDPROC`, `WH_CALLWNDPROCRET`, `WH_GETMESSAGE`) sont installés sur le thread de la fenêtre cible. Le hook clavier est global (thread ID = 0).

---

## Fichiers de configuration utilisateur

Tout est dans `%LocalAppData%\FocusToolPrototype\` :

| Fichier | Format | Contenu |
|---|---|---|
| `settings.json` | JSON | `{ "Modifiers": <uint>, "Key": <int> }` — raccourci global |
| `language.txt` | Texte | `EN` / `FR` / `ZH` |

---

## CI / CD — GitHub Actions

Workflow : `.github/workflows/release.yml`

### Déclencheurs

| Événement | Ce qui se passe |
|---|---|
| Push sur `main` ou PR | Build Release de vérification (CI) — aucun artefact |
| Push d'un tag `v*` | Build → Publish win-x64 self-contained → ZIP → GitHub Release |

### Créer une release

```powershell
git tag v0.2.0
git push origin v0.2.0
```

Le workflow crée automatiquement la release GitHub avec :
- L'archive `GameFocusGuard-v0.2.0-win-x64.zip` (exécutable autonome)
- Les notes de release générées depuis les commits depuis le tag précédent

### Pré-releases

Les tags contenant un tiret sont automatiquement marqués comme pré-release :
- `v1.0.0-rc1` → pré-release ✓
- `v1.0.0-beta.2` → pré-release ✓
- `v1.0.0` → release stable ✓

### Fonctionnement du build natif sur le runner

`windows-latest` dispose de Visual Studio 2022 Enterprise → `vcvars64.bat` est trouvé via le 3e candidat dans `Build-Hook.ps1`. Le DLL natif est compilé automatiquement lors du `dotnet publish` (via `AfterTargets="Publish"` dans le `.csproj`).

Un step de sanity-check vérifie explicitement que `FocusTool.Hook.dll` est présent avant de zipper.

---

## Points d'attention pour les modifications

- **Une seule instance** : `AppInstanceCoordinator` utilise un mutex nommé. Un deuxième lancement active la fenêtre existante et quitte.
- **DLL copiée à la main** : MSBuild ne référence pas le projet C++ ; c'est `Build-Hook.ps1` qui copie `FocusTool.Hook.dll` après chaque build/publish.
- **`Strings.LanguageChanged`** : événement statique. S'abonner dans le constructeur de `Form1`, se désabonner dans `FormClosing` pour éviter les fuites.
- **Fermeture → barre système** : cliquer la croix annule la fermeture (`e.Cancel = true`) et appelle `HideToTray()`. La fermeture réelle n'est déclenchée que par le menu contextuel "Quitter" via le flag `_allowClose`, ou par une fin de session Windows (`CloseReason` ≠ `UserClosing`).
- **`NotifyIcon`** : créé dans `InitializeComponent` via `new NotifyIcon(components)` — il est donc automatiquement disposé par `Form1.Dispose`. `notifyIcon.Visible = false` jusqu'au premier appel de `HideToTray()`.
- **Messages stockés vs langue courante** : les chaînes stockées dans `_lastError` (HookController) et `LastStatus` (HotKeyService) sont figées dans la langue active au moment de l'opération. Elles se mettent à jour à la prochaine opération.
- **Droits administrateur** : l'application demande l'élévation via `app.manifest` (`requireAdministrator`).
- **`pwsh` requis** : le `.csproj` appelle `pwsh` explicitement. PowerShell 5 (`powershell.exe`) n'est pas supporté.
