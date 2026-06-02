<div align="center">

<img src="https://img.shields.io/badge/version-2.2.3-1E6EB4?style=for-the-badge" alt="Version">
<img src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D4?style=for-the-badge&logo=windows" alt="Platform">
<img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-76B900?style=for-the-badge" alt="GPU Support">
<img src="https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?style=for-the-badge" alt=".NET 4.8">

# Spectra

**Professional Digital Vibrance, Brightness & Contrast Controller for NVIDIA & AMD**

*Control display color in real time. Portable, lightweight, zero performance overhead.*

[![Discord](https://img.shields.io/badge/Discord-Join%20for%20updates-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/CdpuNUGPDe)

📢 **Follow [our Discord](https://discord.gg/CdpuNUGPDe) for release announcements and updates.**

</div>

---

## Language / Dil / Язык / Sprache / Langue / Taal

- [English](#english)
- [Türkçe](#türkçe)
- [Русский](#русский)
- [Deutsch](#deutsch)
- [Français](#français)
- [Nederlands](#nederlands)

---

<a name="english"></a>
## English

### Overview

Spectra is a professional digital vibrance management tool for Windows. It communicates directly with GPU driver APIs — NVIDIA's Digital Vibrance Control (NVAPI) and AMD's Display Library (ADL) — to instantly adjust display color saturation with no performance overhead.

### Features

| Feature | Description |
|---------|-------------|
| **NVIDIA & AMD** | Full native support via NVAPI and AMD ADL 32/64-bit |
| **Per-Monitor Vibrance** | Independent vibrance slider for every connected display — set each monitor separately |
| **Quick Presets** | Default / Low / High / Max one-click presets, applied to all monitors |
| **Tray Quick Presets** | Apply any preset directly from the system tray context menu — no window needed |
| **Brightness & Contrast** | GPU-agnostic brightness and contrast sliders (GDI gamma ramp, per-monitor), with one-click reset |
| **Per-Game Profiles** | Add an executable (or pick from running processes / auto-detect the focused full-screen game) and assign a custom vibrance level, applied automatically when the game is in focus and restored on exit |
| **Reliable Profile Engine** | Dual-mode detection: instant WinEvent hook + 500 ms polling backup so profiles never miss a launch or alt-tab |
| **Auto Game Detection** | One-click "Detect Game" creates a profile for the currently focused full-screen application |
| **Gaming Mode** | One-click toggle to temporarily enable/disable all game profiles |
| **Automatic Schedule** | Day/night automatic vibrance — set day and night levels and switch times |
| **Global Hotkey** | Toggle vibrance on/off with any key or Ctrl/Alt modifier combo. Persisted across restarts. Safe on Cyrillic and all non-Latin keyboard layouts |
| **Monitor Selection** | Apply vibrance to all monitors, primary only, or a specific secondary display — other displays stay neutral |
| **Profile Import / Export** | Back up and restore your game profiles as JSON / XML |
| **Minimize to Tray** | Optionally keep running in the tray when the window is closed, with a startup notification |
| **6-Language UI** | English, Turkish, Russian, German, French, Dutch. All labels and buttons fully visible in every language |
| **Startup Integration** | Optional Windows startup via Registry |
| **GitHub Tray Link** | Open the project repository directly from the tray menu |

### System Requirements

- Windows 10 / 11
- NVIDIA GPU with official driver, **or** AMD GPU with official driver
- .NET Framework 4.8 *(pre-installed on Windows 10 v1903+ and Windows 11)*

### Installation

1. Download `Spectra.exe` from the [Releases](../../releases) page
2. Run `Spectra.exe` — no installation or administrator rights required
3. GPU is detected automatically at startup

### Usage

1. **Desktop Vibrance** — Each connected monitor has its own slider; drag it or click a preset (Default / Low / High / Max). Changes apply instantly.
2. **Brightness & Contrast** — Use the dedicated sliders; press *Reset* to return to neutral.
3. **Game Profiles** — Click *Add File*, *Browse Running*, or *Detect Game* (auto-detects the focused full-screen game). Double-click an entry to set its in-game vibrance level. The profile applies automatically when the game gains focus.
4. **Gaming Mode** — Click 🎮 in the status bar to temporarily enable/disable all game profiles.
5. **Schedule** — In Settings → Behavior, enable the day/night automatic vibrance and set levels and switch times.
6. **Hotkey** — Click the hotkey button and press any key combination to set a global vibrance toggle.
7. **Tray Presets** — Right-click the tray icon → *Quick Presets* to switch vibrance without opening the window.
8. **Settings** — Click ⚙ to access Behavior, Display, Profiles, and About tabs.

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
dotnet build Spectra/Spectra.csproj --configuration Release
```

Output: `Spectra/bin/x86/Release/Spectra.exe`

### Changelog

#### v2.2.3 — Desktop Vibrance Stability Fix
- **Fixed (critical):** Desktop vibrance no longer drops to 0 when switching between programs — root cause was `SetVibranceForMonitor` never updating `userVibranceSettingDefault`, causing the restore logic to apply level 0 on every window-focus event
- **Fixed (critical):** WinEventHook else branch now has a guard: vibrance is only touched when transitioning OUT of a game profile — normal program switching has zero effect on the user's desktop vibrance
- **Fixed:** Per-monitor desktop levels are tracked individually so each monitor returns to its own user-set level after a game session, not a single averaged fallback
- **UI:** Added horizontal separator line between Brightness and Contrast sliders

#### v2.2.1 — Profile Engine Reliability & Display Fixes
- **Fixed (critical):** Game profiles now reliably apply vibrance on every launch and alt-tab — dual detection (WinEventHook + 500 ms polling timer) means events are never missed by either mechanism alone
- **Fixed (critical):** Profile matching now uses the executable filename as the primary key, not the display name — profiles work correctly regardless of what the profile entry is labelled
- **Fixed (critical):** NVIDIA — vibrance was silently skipped when `getAssociatedNvidiaDisplayHandle` returned −1 (common during game transitions); now falls back to the default handle so vibrance is always applied
- **Fixed:** Brightness & Contrast sliders now work on all displays — switched from `GetDC(IntPtr.Zero)` to per-monitor `CreateDC` calls (Windows 10/11 requirement for hardware-accelerated compositing)
- **Fixed:** `isWindowActive` / `GetForegroundWindow` guards that silently blocked vibrance restoration on both NVIDIA and AMD are removed
- **Removed:** Light / Dark theme toggle — single clean professional theme

#### v2.2.0 — UI & Stability
- Brightness & Contrast control with GDI gamma ramp
- Per-monitor independent vibrance sliders
- Day/Night automatic schedule
- Minimize to tray on close
- Profile import/export (JSON/XML)

#### v2.1.0 — Major Feature Release
- **Per-monitor vibrance:** every connected display now has its own independent slider
- **Brightness & contrast** control (GPU-agnostic, gamma-based) with one-click reset
- **Automatic day/night schedule** — set day and night vibrance levels and switch times
- **Auto game detection** — one click creates a profile for the focused full-screen game
- **Gaming mode** — temporarily enable/disable all game profiles with one click
- **Minimize to tray** on close (optional) with a background-running notification
- **Profile import/export** fully wired to JSON / XML
- **Discord** community link added for release announcements

#### v1.9.5 — Vibrance Engine Fix & Profiles Restored
- **Fixed (critical):** Desktop vibrance now applies the moment you move the slider or click a preset
- **Fixed:** Monitor selection now resets non-target displays to neutral (NVIDIA & AMD)
- **Restored:** Per-game profiles — assign a custom in-game vibrance level, applied automatically on focus

---

<a name="türkçe"></a>
## Türkçe

### Genel Bakış

Spectra, Windows'ta gerçek zamanlı renk doygunluğu (vibrance) kontrolü için profesyonel bir araçtır. NVIDIA NVAPI ve AMD ADL üzerinden doğrudan GPU ile konuşur; performans kaybı sıfırdır.

### Özellikler

| Özellik | Açıklama |
|---------|----------|
| **NVIDIA & AMD** | NVAPI ve AMD ADL 32/64-bit üzerinden tam destek |
| **Monitör Başına Vibrance** | Her monitör için bağımsız kaydırıcı |
| **Hızlı Ön Ayarlar** | Tek tıkla Default / Low / High / Max |
| **Tepsi Hızlı Ön Ayarları** | Tepsiden ön ayar uygulama, pencere açmaya gerek yok |
| **Parlaklık & Kontrast** | GPU bağımsız gamma ramp kontrolü, tek tıkla sıfırlama |
| **Oyun Profilleri** | Oyun başladığında otomatik vibrance uygulama, çıkışta masaüstü seviyesi geri yükleme |
| **Güvenilir Profil Motoru** | WinEvent hook + 500ms polling yedekleme — hiçbir başlatma veya alt-tab atlanmaz |
| **Otomatik Oyun Algılama** | Tek tıkla aktif tam ekran oyun için profil oluşturma |
| **Oyun Modu** | Tüm profilleri geçici olarak devre dışı bırakma / etkinleştirme |
| **Otomatik Zamanlama** | Gündüz/gece vibrance seviyeleri ve geçiş saatleri |
| **Global Kısayol Tuşu** | Ctrl/Alt modifier destekli, Kiril klavyelerde güvenli |
| **Monitör Seçimi** | Tüm monitörler, yalnızca birincil veya belirli bir monitör |
| **Profil İçe/Dışa Aktarma** | JSON / XML ile yedekleme ve geri yükleme |
| **Tepsiye Küçültme** | Pencere kapatıldığında arka planda çalışmaya devam etme |
| **6 Dil Desteği** | Türkçe dahil tüm dillerde tam görünüm |
| **Başlangıç Entegrasyonu** | İsteğe bağlı Windows başlangıç kaydı |

### Kurulum

1. [Releases](../../releases) sayfasından `Spectra.exe` indirin
2. Çalıştırın — kurulum veya yönetici hakkı gerekmez
3. GPU otomatik algılanır

---

<a name="русский"></a>
## Русский

### Обзор

Spectra — профессиональный инструмент управления цифровой яркостью для Windows. Использует NVIDIA NVAPI и AMD ADL для мгновенного изменения насыщенности цветов монитора без нагрузки на производительность.

### Возможности

| Функция | Описание |
|---------|----------|
| **NVIDIA и AMD** | Полная поддержка через NVAPI и AMD ADL |
| **Насыщенность на монитор** | Независимый слайдер для каждого дисплея |
| **Быстрые пресеты** | Default/Low/High/Max одним кликом |
| **Яркость и контраст** | Гамма-управление без привязки к GPU |
| **Игровые профили** | Автоматическое применение вибранса при запуске игры |
| **Надёжный движок профилей** | WinEvent + опрос каждые 500 мс — события не пропускаются |
| **Горячая клавиша** | Ctrl/Alt+клавиша — безопасно на кириллической раскладке |
| **6 языков** | Русский интерфейс включён |

### Установка

1. Скачайте `Spectra.exe` со страницы [Releases](../../releases)
2. Запустите — установка не требуется

---

<a name="deutsch"></a>
## Deutsch

### Überblick

Spectra ist ein professionelles Vibrance-Steuerungswerkzeug für Windows. Es nutzt NVIDIA NVAPI und AMD ADL für sofortige Farbsättigungsanpassungen ohne Leistungsverlust.

### Funktionen

| Funktion | Beschreibung |
|----------|--------------|
| **NVIDIA & AMD** | Volle Unterstützung via NVAPI und AMD ADL |
| **Vibrance pro Monitor** | Unabhängiger Schieberegler für jeden Bildschirm |
| **Schnellpresets** | Default/Low/High/Max mit einem Klick |
| **Helligkeit & Kontrast** | GPU-unabhängige Gamma-Steuerung |
| **Spielprofile** | Automatische Vibrance-Anpassung beim Spielstart |
| **Zuverlässige Profilerkennung** | WinEvent + 500ms Polling-Backup |
| **Schnelltaste** | Ctrl/Alt+Taste — sicher auf allen Tastaturlayouts |
| **6 Sprachen** | Deutsch vollständig unterstützt |

### Installation

1. `Spectra.exe` von der [Releases](../../releases) Seite herunterladen
2. Ausführen — keine Installation erforderlich

---

<a name="français"></a>
## Français

### Aperçu

Spectra est un outil professionnel de contrôle de la vibrance numérique pour Windows. Il utilise NVIDIA NVAPI et AMD ADL pour ajuster instantanément la saturation des couleurs sans impact sur les performances.

### Fonctionnalités

| Fonctionnalité | Description |
|----------------|-------------|
| **NVIDIA & AMD** | Support complet via NVAPI et AMD ADL |
| **Vibrance par moniteur** | Curseur indépendant pour chaque écran |
| **Préréglages rapides** | Default/Low/High/Max en un clic |
| **Luminosité & Contraste** | Contrôle gamma indépendant du GPU |
| **Profils de jeu** | Application automatique de la vibrance au lancement |
| **Moteur fiable** | WinEvent + sondage 500ms de secours |
| **Raccourci global** | Ctrl/Alt+touche — sûr sur tous les claviers |
| **6 langues** | Français entièrement supporté |

### Installation

1. Téléchargez `Spectra.exe` depuis la page [Releases](../../releases)
2. Exécutez — aucune installation requise

---

<a name="nederlands"></a>
## Nederlands

### Overzicht

Spectra is een professionele vibrance-beheertool voor Windows. Het gebruikt NVIDIA NVAPI en AMD ADL voor onmiddellijke kleurverzadigingsaanpassingen zonder prestatieverlies.

### Functies

| Functie | Beschrijving |
|---------|-------------|
| **NVIDIA & AMD** | Volledige ondersteuning via NVAPI en AMD ADL |
| **Vibrance per monitor** | Onafhankelijke schuifregelaar per scherm |
| **Snelle presets** | Default/Low/High/Max met één klik |
| **Helderheid & Contrast** | GPU-onafhankelijke gamma-regeling |
| **Spelprofielen** | Automatische vibrance bij het starten van een spel |
| **Betrouwbare detectie** | WinEvent + 500ms polling back-up |
| **Globale sneltoets** | Ctrl/Alt+toets — veilig op alle toetsenbordindelingen |
| **6 talen** | Nederlands volledig ondersteund |

### Installatie

1. Download `Spectra.exe` van de [Releases](../../releases) pagina
2. Uitvoeren — geen installatie vereist

---

<div align="center">

**Spectra v2.2.3** — Professional Digital Vibrance, Brightness & Contrast Control  
[Discord](https://discord.gg/CdpuNUGPDe) · [GitHub](https://github.com/X1NPAR1/Spectra) · [Releases](https://github.com/X1NPAR1/Spectra/releases)

</div>
