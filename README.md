<div align="center">

<img src="https://img.shields.io/badge/version-2.1.0-1E6EB4?style=for-the-badge" alt="Version">
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
| **Brightness & Contrast** | GPU-agnostic brightness and contrast sliders (gamma-based), with one-click reset |
| **Per-Game Profiles** | Add an executable (or pick from running processes / auto-detect the focused full-screen game) and assign a custom vibrance level, applied automatically when the game is in focus and restored on exit |
| **Auto Game Detection** | One-click "Detect Game" creates a profile for the currently focused full-screen application |
| **Gaming Mode** | One-click toggle to temporarily enable/disable all game profiles |
| **Automatic Schedule** | Day/night automatic vibrance — set day and night levels and switch times (blue-light style) |
| **Light / Dark Theme** | Switch the interface between light and dark with a single header button |
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
5. **Theme** — Click 🌙 / ☀ in the header to switch between light and dark.
6. **Schedule** — In Settings → Behavior, enable the day/night automatic vibrance and set levels and switch times.
7. **Hotkey** — Click the hotkey button and press any key combination to set a global vibrance toggle.
8. **Tray Presets** — Right-click the tray icon → *Quick Presets* to switch vibrance without opening the window.
9. **Settings** — Click ⚙ to access Behavior, Display, Profiles, and About tabs.

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
dotnet build Spectra/Spectra.csproj /p:Configuration=Release /p:Platform=x86
```

Output: `Spectra/bin/x86/Release/Spectra.exe`

### Changelog

#### v2.1.0 — Major Feature Release
- **Per-monitor vibrance:** every connected display now has its own independent slider
- **Brightness & contrast** control (GPU-agnostic, gamma-based) with one-click reset
- **Automatic day/night schedule** — set day and night vibrance levels and switch times
- **Light / dark theme** toggle in the header
- **Auto game detection** — one click creates a profile for the focused full-screen game
- **Gaming mode** — temporarily enable/disable all game profiles with one click
- **Minimize to tray** on close (optional) with a background-running notification
- **Profile import/export** fully wired to JSON / XML
- **Discord** community link added for release announcements

#### v1.9.5 — Vibrance Engine Fix & Profiles Restored
- **Fixed (critical):** Desktop vibrance now applies to the screen the moment you move the slider or click a preset. Previously the level was only stored and applied on the next window-focus event, so changes appeared to do nothing.
- **Fixed:** Monitor selection now resets non-target displays to a neutral level, so vibrance truly affects only the chosen monitor (NVIDIA & AMD)
- **Restored:** Per-game profiles — add an executable and assign a custom in-game vibrance level, applied automatically on focus
- **Improved:** Settings window resized and the tab bar no longer shows overflow scroll arrows; all three tabs fit cleanly

#### v1.9.4 — Simplified & Focused
- **Removed:** Game profiles section — simplified the UI to focus on desktop vibrance control
- **Fix:** Window title now shows `v1.9.4` instead of `v1.9.4.0`
- **Improved:** Smaller, more compact main window
- **Improved:** Settings window has 3 clean tabs: Behavior, Display, About

#### v1.9.3
- Settings window height increased to 600 px — OK button always visible
- Tray menu GitHub link added (underlined, opens project page)
- Profile export now produces real JSON format

#### v1.9.2
- Monitor selection now correctly restricts vibrance to the chosen display (NVIDIA & AMD)
- Vibrance preset buttons localize with language selection
- Tray Quick Presets menu items also localize

#### v1.9.1
- Preset buttons no longer overlap the vibrance trackbar
- Settings window widened; all button text visible in all 6 languages
- Monitor combo full-width stacked layout

#### v1.9.0
- Hotkey modifier support (Ctrl/Alt+key) — no accidental Cyrillic triggers
- Tray Quick Presets submenu
- Hotkey persisted across restarts

#### v1.8.0
- Single professional navy-blue theme
- Dutch (Nederlands) added as sixth language
- Settings dialog redesigned

---

<a name="türkçe"></a>
## Türkçe

### Genel Bakış

Spectra, Windows'ta gerçek zamanlı renk doygunluğu (vibrance) kontrolü için profesyonel bir araçtır. NVIDIA NVAPI ve AMD ADL üzerinden doğrudan GPU ile konuşur; performans kaybı sıfırdır.

### Özellikler

| Özellik | Açıklama |
|---------|----------|
| **NVIDIA & AMD** | NVAPI ve AMD ADL 32/64-bit üzerinden tam destek |
| **Vibrance Kaydırıcı** | Gerçek zamanlı masaüstü canlılık kontrolü |
| **Hızlı Ön Ayarlar** | Ana ekranda tek tıkla Default/Low/High/Max |
| **Tepsi Hızlı Ön Ayarları** | Tepsiden ön ayar uygulama |
| **Global Kısayol Tuşu** | Ctrl/Alt modifier destekli, Kiril klavyelerde güvenli |
| **Monitör Seçimi** | Tüm monitörler, yalnızca birincil veya belirli bir monitör |
| **6 Dil Desteği** | Türkçe dahil tüm dillerde tam görünüm |
| **Başlangıç Entegrasyonu** | İsteğe bağlı Windows başlangıç kaydı |

### Kurulum

1. [Releases](../../releases) sayfasından `Spectra.exe` indirin
2. Çalıştırın — kurulum veya yönetici hakkı gerekmez

---

<a name="русский"></a>
## Русский

### Обзор

Spectra — профессиональный инструмент управления цифровой яркостью для Windows. Использует NVIDIA NVAPI и AMD ADL для мгновенного изменения насыщенности цветов монитора без нагрузки на производительность.

### Возможности

| Функция | Описание |
|---------|----------|
| **NVIDIA и AMD** | Полная поддержка через NVAPI и AMD ADL |
| **Слайдер яркости** | Управление в реальном времени |
| **Быстрые пресеты** | Default/Low/High/Max одним кликом |
| **Пресеты в трее** | Смена яркости из системного трея |
| **Горячая клавиша** | Ctrl/Alt+клавиша — безопасно на кириллической раскладке |
| **Выбор монитора** | Все мониторы, основной или конкретный дисплей |
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
| **Vibrance-Schieberegler** | Echtzeit-Steuerung |
| **Schnellpresets** | Default/Low/High/Max mit einem Klick |
| **Tray-Presets** | Vibrance direkt aus dem Systemtray ändern |
| **Schnelltaste** | Ctrl/Alt+Taste — sicher auf allen Tastaturlayouts |
| **Monitorauswahl** | Alle, nur primär oder ein bestimmter Monitor |
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
| **Curseur de vibrance** | Contrôle en temps réel |
| **Préréglages rapides** | Default/Low/High/Max en un clic |
| **Préréglages dans le tray** | Changer la vibrance depuis la barre système |
| **Raccourci global** | Ctrl/Alt+touche — sûr sur tous les claviers |
| **Sélection du moniteur** | Tous, primaire uniquement ou un moniteur spécifique |
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
| **Vibrance-schuifregelaar** | Realtime controle |
| **Snelle presets** | Default/Low/High/Max met één klik |
| **Tray-presets** | Vibrance direct vanuit de systeembalk |
| **Globale sneltoets** | Ctrl/Alt+toets — veilig op alle toetsenbordindelingen |
| **Monitorselectie** | Alle, alleen primair of een specifiek scherm |
| **6 talen** | Nederlands volledig ondersteund |

### Installatie

1. Download `Spectra.exe` van de [Releases](../../releases) pagina
2. Uitvoeren — geen installatie vereist

---

<div align="center">

**Spectra v2.1.0** — Professional Digital Vibrance, Brightness & Contrast Control  
[Discord](https://discord.gg/CdpuNUGPDe) · 
[GitHub](https://github.com/X1NPAR1/Spectra) · [Releases](https://github.com/X1NPAR1/Spectra/releases)

</div>
