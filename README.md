<div align="center">

<img src="https://img.shields.io/badge/version-1.9.4-1E6EB4?style=for-the-badge" alt="Version">
<img src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D4?style=for-the-badge&logo=windows" alt="Platform">
<img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-76B900?style=for-the-badge" alt="GPU Support">
<img src="https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?style=for-the-badge" alt=".NET 4.8">

# Spectra

**Professional Digital Vibrance Controller for NVIDIA & AMD**

*Control display color saturation in real time. Portable, lightweight, zero performance overhead.*

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
| **Vibrance Slider** | Real-time desktop vibrance control |
| **Quick Presets** | Default / Low / High / Max one-click presets on the main screen |
| **Tray Quick Presets** | Apply any preset directly from the system tray context menu — no window needed |
| **Global Hotkey** | Toggle vibrance on/off with any key or Ctrl/Alt modifier combo. Persisted across restarts. Safe on Cyrillic and all non-Latin keyboard layouts |
| **Monitor Selection** | Apply vibrance to all monitors, primary only, or a specific secondary display |
| **6-Language UI** | English, Turkish, Russian, German, French, Dutch. All labels and buttons fully visible in every language |
| **Vibrance Apply Delay** | Configurable delay (ms) before applying vibrance |
| **Startup Integration** | Optional Windows startup via Registry |
| **System Tray** | Run silently in the background with full tray control |
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

1. **Desktop Vibrance** — Drag the slider or click a preset button (Default / Low / High / Max)
2. **Hotkey** — Click the hotkey button and press any key combination to set a global vibrance toggle
3. **Tray Presets** — Right-click the tray icon → *Quick Presets* to switch vibrance without opening the window
4. **Settings** — Click ⚙ to access Behavior, Display, and About tabs

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
dotnet build Spectra/Spectra.csproj /p:Configuration=Release /p:Platform=x86
```

Output: `Spectra/bin/x86/Release/Spectra.exe`

### Changelog

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

**Spectra v1.9.4** — Professional Digital Vibrance Control  
[GitHub](https://github.com/X1NPAR1/Spectra) · [Releases](https://github.com/X1NPAR1/Spectra/releases)

</div>
