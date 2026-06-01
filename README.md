<div align="center">

<img src="https://img.shields.io/badge/version-1.9.1-1E6EB4?style=for-the-badge" alt="Version">
<img src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D4?style=for-the-badge&logo=windows" alt="Platform">
<img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-76B900?style=for-the-badge" alt="GPU Support">
<img src="https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?style=for-the-badge" alt=".NET 4.8">

# Spectra

**Professional Digital Vibrance Controller for NVIDIA & AMD**

*Automate per-application color saturation on Windows with zero performance overhead.*

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

Spectra is a professional-grade digital vibrance management utility for Windows, built for competitive gamers and visual professionals. It communicates directly with GPU driver APIs — NVIDIA's Digital Vibrance Control (NVAPI) and AMD's Display Library (ADL) — to apply per-application color saturation profiles automatically, with no measurable performance impact.

When a configured application launches, Spectra applies its vibrance profile instantly. When the application closes, the desktop profile is restored seamlessly.

### Features

| Feature | Description |
|---------|-------------|
| **NVIDIA & AMD** | Native support via NVAPI (NVIDIA) and ADL 32/64-bit (AMD) |
| **Per-Game Profiles** | Individual vibrance settings per executable |
| **Global Hotkey** | Toggle vibrance with any key or modifier combination (Ctrl/Alt+key). Safe on all keyboard layouts including Cyrillic |
| **Tray Quick Presets** | Apply Default / Low / High / Max vibrance instantly from the system tray, without opening the application |
| **6-Language UI** | English, Turkish, Russian, German, French, Dutch — all labels and buttons sized correctly for every language |
| **Vibrance Apply Delay** | Configurable delay (ms) before applying vibrance when a game launches |
| **Resolution Switching** | Automatic per-game resolution change for borderless/windowed modes |
| **System Tray** | Full tray integration: open, toggle, presets, exit |
| **Startup Integration** | Optional Windows startup entry via Registry |
| **Primary Monitor Control** | Optionally restrict vibrance to the primary display |
| **Profile Import / Export** | Back up and restore game profiles in XML format |
| **Persistent Hotkey** | Custom hotkey is saved and restored across restarts |

### System Requirements

- Windows 10 / 11
- NVIDIA GPU with official driver, **or** AMD GPU with official driver
- .NET Framework 4.8 *(pre-installed on Windows 10 v1903+ and Windows 11)*

### Installation

1. Download `Spectra.exe` from the [Releases](../../releases) page
2. Run `Spectra.exe` — no installation or administrator rights required
3. The application detects your GPU automatically at startup

### Usage

1. **Desktop Vibrance** — Adjust the slider or click Default / Low / High / Max
2. **Add a Game** — Click *Add File* to browse for an `.exe`, or *Browse Running* to select from active processes
3. **Edit a Profile** — Double-click any game entry to configure its vibrance level and optional resolution override
4. **Hotkey** — Click the hotkey button and press any key (with optional Ctrl/Alt modifier) to register a global toggle
5. **Tray Presets** — Right-click the tray icon → *Quick Presets* to switch vibrance levels without opening the window
6. **Settings** — Click ⚙ to access Behavior, Display, Profile management, and About tabs

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
dotnet build Spectra/Spectra.csproj /p:Configuration=Release /p:Platform=x86
```

Output: `Spectra/bin/x86/Release/Spectra.exe`

### Changelog

#### v1.9.1 — Layout & Localization Polish
- **Fix:** Preset buttons (Default/Low/High/Max) no longer overlap with the vibrance trackbar thumb at any DPI setting
- **Fix:** Settings window widened to 540 px — all tab content has room to breathe
- **Fix:** Profile action buttons (Add / Browse / Remove) are now equal-width columns, no clipping in any language
- **Fix:** Data tab buttons (Export / Import / Clear / Open Log / Reset) use a two-column grid layout — all translations always fully visible
- **Fix:** Behavior & Display tab checkboxes span full content width — no text truncation in any supported language
- **Fix:** Delay description label wraps correctly instead of overflowing the tab

#### v1.9.0 — Hotkey Modifiers & Tray Presets
- Hotkey modifier key support (Ctrl/Alt/Shift+key) — eliminates accidental triggers on non-Latin keyboard layouts
- Tray Quick Presets submenu (Default / Low / High / Max without opening the app)
- Hotkey persisted across restarts
- Settings About panel displays the real application icon
- Removed legacy `spectra.ico`

#### v1.8.0 — New Theme & Dutch Support
- Single professional navy-blue theme matching the application icon
- Card-based UI panels with accent borders
- Dutch (Nederlands) added as the sixth interface language
- Settings dialog redesigned with Behavior / Display / Profiles / About tabs

#### v1.7.1 — UI/UX Overhaul
- Complete visual redesign

#### v1.7.0 — Initial Release

---

<a name="türkçe"></a>
## Türkçe

### Genel Bakış

Spectra, rekabetçi oyuncular ve görsel profesyoneller için tasarlanmış, Windows platformunda çalışan profesyonel bir dijital canlılık (vibrance) yönetim aracıdır. NVIDIA NVAPI ve AMD ADL sürücü arayüzleri üzerinden doğrudan GPU ile iletişim kurarak, uygulama başına renk doygunluğu profillerini sıfır performans kaybıyla otomatik olarak uygular.

### Özellikler

| Özellik | Açıklama |
|---------|----------|
| **NVIDIA & AMD Desteği** | NVAPI ve AMD ADL 32/64-bit arayüzleri üzerinden tam destek |
| **Oyun Başına Profil** | Her oyun yürütülebiliri için ayrı vibrance ayarı |
| **Global Kısayol Tuşu** | Ctrl/Alt modifier destekli — Kiril klavyelerde güvenli |
| **Tepsi Hızlı Ön Ayarları** | Uygulamayı açmadan tepsi menüsünden Default/Low/High/Max |
| **6 Dil Desteği** | Türkçe dahil tüm dillerde buton ve etiketler eksiksiz görünür |
| **Çözünürlük Değişimi** | Oyun başına otomatik çözünürlük geçişi |
| **Sistem Tepsisi** | Açma, geçiş, ön ayar ve çıkış işlemleri tepsi üzerinden |
| **Başlangıç Entegrasyonu** | İsteğe bağlı Windows başlangıç kaydı |

### Kurulum

1. [Releases](../../releases) sayfasından `Spectra.exe` dosyasını indirin
2. `Spectra.exe` dosyasını çalıştırın — kurulum veya yönetici hakkı gerekmez

---

<a name="русский"></a>
## Русский

### Обзор

Spectra — профессиональная утилита управления цифровой яркостью для Windows. Взаимодействует напрямую с API драйверов GPU — NVIDIA NVAPI и AMD ADL — для автоматического применения профилей насыщенности цветов к каждому приложению без нагрузки на производительность.

### Возможности

| Функция | Описание |
|---------|----------|
| **NVIDIA и AMD** | Нативная поддержка через NVAPI и AMD ADL 32/64-bit |
| **Профили для игр** | Индивидуальные настройки яркости для каждого исполняемого файла |
| **Горячая клавиша** | Ctrl/Alt+клавиша — нет случайных срабатываний на кириллической раскладке |
| **Быстрые пресеты в трее** | Default / Low / High / Max без открытия приложения |
| **6 языков интерфейса** | Все тексты и кнопки отображаются полностью на каждом языке |
| **Системный трей** | Полное управление через контекстное меню трея |

### Установка

1. Скачайте `Spectra.exe` со страницы [Releases](../../releases)
2. Запустите — установка не требуется, права администратора не нужны

---

<a name="deutsch"></a>
## Deutsch

### Überblick

Spectra ist ein professionelles Digital-Vibrance-Management-Werkzeug für Windows. Es kommuniziert direkt mit den GPU-Treiber-APIs — NVIDIA NVAPI und AMD ADL — und wendet Farbsättigungsprofile automatisch pro Anwendung an, ohne messbare Leistungseinbußen.

### Funktionen

| Funktion | Beschreibung |
|----------|--------------|
| **NVIDIA & AMD** | Native Unterstützung über NVAPI und AMD ADL 32/64-bit |
| **Spielprofile** | Individuelle Vibrance-Einstellungen pro Programm |
| **Schnelltaste** | Ctrl/Alt+Taste — sicher auf allen Tastaturlayouts |
| **Tray-Schnellpresets** | Default / Low / High / Max direkt aus dem Systemtray |
| **6 Sprachen** | Alle Beschriftungen vollständig in jeder Sprache |
| **Systemtray** | Vollständige Steuerung über das Tray-Kontextmenü |

### Installation

1. `Spectra.exe` von der [Releases](../../releases) Seite herunterladen
2. Ausführen — keine Installation, keine Administratorrechte erforderlich

---

<a name="français"></a>
## Français

### Aperçu

Spectra est un outil professionnel de gestion de la vibrance numérique pour Windows. Il communique directement avec les API des pilotes GPU — NVIDIA NVAPI et AMD ADL — pour appliquer automatiquement des profils de saturation couleur par application, sans impact mesurable sur les performances.

### Fonctionnalités

| Fonctionnalité | Description |
|----------------|-------------|
| **NVIDIA & AMD** | Support natif via NVAPI et AMD ADL 32/64-bit |
| **Profils par jeu** | Paramètres de vibrance individuels par exécutable |
| **Raccourci global** | Ctrl/Alt+touche — sûr sur tous les agencements de clavier |
| **Préréglages rapides** | Default / Low / High / Max directement depuis le tray système |
| **6 langues** | Tous les libellés et boutons s'affichent entièrement dans chaque langue |
| **Barre système** | Contrôle complet via le menu contextuel du tray |

### Installation

1. Téléchargez `Spectra.exe` depuis la page [Releases](../../releases)
2. Exécutez — aucune installation ni droits administrateur requis

---

<a name="nederlands"></a>
## Nederlands

### Overzicht

Spectra is een professionele digitale vibrance-beheertool voor Windows. Het communiceert rechtstreeks met GPU-stuurprogramma-API's — NVIDIA NVAPI en AMD ADL — om per applicatie automatisch kleurverzadigingsprofielen toe te passen, zonder meetbaar prestatieverlies.

### Functies

| Functie | Beschrijving |
|---------|-------------|
| **NVIDIA & AMD** | Native ondersteuning via NVAPI en AMD ADL 32/64-bit |
| **Per-game profielen** | Individuele vibrance-instellingen per uitvoerbaar bestand |
| **Globale sneltoets** | Ctrl/Alt+toets — veilig op alle toetsenbordindelingen |
| **Snelle presets in tray** | Default / Low / High / Max direct vanuit de systeembalk |
| **6 talen** | Alle labels en knoppen volledig zichtbaar in elke taal |
| **Systeembalk** | Volledige bediening via het contextmenu van de systeembalk |

### Installatie

1. Download `Spectra.exe` van de [Releases](../../releases) pagina
2. Uitvoeren — geen installatie of beheerdersrechten vereist

---

<div align="center">

**Spectra v1.9.1** — Professional Digital Vibrance Control  
[GitHub](https://github.com/X1NPAR1/Spectra) · [Releases](https://github.com/X1NPAR1/Spectra/releases)

</div>
