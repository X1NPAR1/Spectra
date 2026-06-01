<div align="center">

<img src="https://img.shields.io/badge/version-1.9.0-1E6EB4?style=for-the-badge" alt="Version">
<img src="https://img.shields.io/badge/platform-Windows-0078D4?style=for-the-badge&logo=windows" alt="Platform">
<img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-76B900?style=for-the-badge" alt="GPU">
<img src="https://img.shields.io/badge/.NET-4.8-512BD4?style=for-the-badge" alt=".NET">

# Spectra

**Professional Digital Vibrance Controller for NVIDIA & AMD**

</div>

---

## Languages / Diller / Языки / Sprachen / Langues / Talen

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

**Spectra** is a professional-grade digital vibrance management utility for Windows, designed for competitive gamers and visual professionals. It leverages native GPU driver APIs — NVIDIA's Digital Vibrance Control (DVC) and AMD's Display Library (ADL) — to automate per-application color saturation profiles with zero performance overhead.

When you launch a configured game, Spectra instantly applies your custom vibrance settings. When you exit, it restores your desktop profile seamlessly.

### Features

| Feature | Description |
|---------|-------------|
| **NVIDIA & AMD Support** | Full support via NVAPI and AMD ADL driver interfaces |
| **Per-Game Profiles** | Individual vibrance levels for each game |
| **Global Hotkey** | Toggle vibrance on/off with any key combo (default: F9). Supports Ctrl/Alt modifiers — safe on all keyboard layouts including Cyrillic |
| **Quick Presets** | Default / Low / High / Max presets on main UI and in system tray |
| **Tray Quick Presets** | Apply vibrance presets instantly from the system tray without opening the app |
| **6-Language Interface** | English, Turkish, Russian, German, French, Dutch |
| **Resolution Switching** | Auto-switch resolution per game (borderless/windowed) |
| **System Tray** | Runs silently in background with full tray menu control |
| **Startup Integration** | Optional Windows startup registration |
| **Primary Monitor Control** | Optionally limit vibrance to primary display only |
| **Profile Import/Export** | Back up and restore your game profiles |
| **Vibrance Apply Delay** | Configurable delay before applying vibrance when a game launches |

### Requirements

- Windows 10 / 11
- NVIDIA GPU with official driver **OR** AMD GPU with official driver
- .NET Framework 4.8 (included in Windows 10/11)

### Installation

1. Download `Spectra.exe` from the [Releases](../../releases) page
2. Run `Spectra.exe` — no installation required
3. Configure your game profiles and preferred vibrance levels
4. Optionally enable **Launch on startup** for automatic background operation

### Usage

1. **Desktop Vibrance** — Set your preferred desktop color level using the slider or quick presets
2. **Add Game** — Click *Add File* to browse for an executable, or *Browse Running* to pick from active processes
3. **Profile Settings** — Double-click any game to configure its vibrance level and optional resolution
4. **Hotkey** — Click the hotkey button and press any key (or key combination with Ctrl/Alt) to set a global vibrance toggle
5. **Tray Quick Presets** — Right-click the tray icon → Quick Presets for instant vibrance changes
6. **Settings** — Open the Settings panel (⚙) for advanced options: behavior, display, profile management, and about

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
msbuild Spectra/Spectra.csproj /p:Configuration=Release /p:Platform=x86
```

### Changelog

#### v1.9.0
- **New:** Tray Quick Presets — apply Default/Low/High/Max vibrance directly from tray menu
- **New:** Hotkey modifier key support (Ctrl+key, Alt+key) — eliminates accidental triggers on Russian/Cyrillic and other non-Latin keyboard layouts
- **New:** Hotkey is now persisted across restarts
- **Fix:** Russian keyboard layout no longer triggers hotkey accidentally during typing
- **Fix:** About panel now shows the application icon (setting.ico) instead of a placeholder
- **Removed:** Old legacy icon (spectra.ico) deleted
- **Updated:** Version bumped to 1.9.0

#### v1.8.0
- Single professional navy-blue theme matching application icon
- Card-based UI panels with accent borders
- Dutch language support added
- New Settings dialog with Behavior/Display/Profiles/About tabs

#### v1.7.1
- Complete UI/UX overhaul

#### v1.7.0
- Initial release

---

<a name="türkçe"></a>
## Türkçe

### Genel Bakış

**Spectra**, rekabetçi oyuncular ve görsel profesyoneller için tasarlanmış Windows platformunda çalışan profesyonel bir dijital canlılık (vibrance) yönetim aracıdır. NVIDIA'nın Dijital Canlılık Kontrolü (DVC) ve AMD'nin Görüntü Kütüphanesi (ADL) sürücü API'lerini kullanarak uygulama başına renk doygunluğu profillerini sıfır performans kaybıyla otomatikleştirir.

### Özellikler

| Özellik | Açıklama |
|---------|----------|
| **NVIDIA & AMD Desteği** | NVAPI ve AMD ADL arayüzleri üzerinden tam destek |
| **Oyun Başına Profil** | Her oyun için ayrı vibrance seviyesi |
| **Global Kısayol Tuşu** | Ctrl/Alt modifier destekli — Kiril klavyelerde güvenli |
| **Hızlı Ön Ayarlar** | Ana ekran ve tepsi menüsünden Default/Low/High/Max |
| **Tepsi Hızlı Ön Ayarlar** | Uygulamayı açmadan tepsiden vibrance seviyesi değiştir |
| **6 Dil Desteği** | Türkçe dahil 6 dil |
| **Çözünürlük Değişimi** | Oyun başına otomatik çözünürlük geçişi |
| **Sistem Tepsisi** | Arka planda sessiz çalışma, tam tepsi menü kontrolü |
| **Başlangıç Entegrasyonu** | İsteğe bağlı Windows başlangıcına kayıt |

### Kurulum

1. [Releases](../../releases) sayfasından `Spectra.exe` dosyasını indirin
2. `Spectra.exe` dosyasını çalıştırın — kurulum gerekmez
3. Oyun profillerinizi ve vibrance seviyelerinizi yapılandırın

---

<a name="русский"></a>
## Русский

### Обзор

**Spectra** — профессиональный инструмент управления цифровой яркостью (Digital Vibrance) для Windows. Использует нативные API драйверов — NVIDIA DVC и AMD ADL — для автоматического применения профилей насыщенности цветов для каждого приложения без нагрузки на производительность.

### Возможности

| Функция | Описание |
|---------|----------|
| **NVIDIA и AMD** | Полная поддержка через NVAPI и AMD ADL |
| **Профили для игр** | Индивидуальные уровни яркости для каждой игры |
| **Горячая клавиша** | Поддержка Ctrl/Alt+клавиша — нет случайных срабатываний на кириллической раскладке |
| **Быстрые пресеты в трее** | Смена уровня яркости прямо из системного трея |
| **6 языков** | Русский интерфейс включён |
| **Системный трей** | Тихая фоновая работа с полным управлением из трея |

### Установка

1. Скачайте `Spectra.exe` со страницы [Releases](../../releases)
2. Запустите — установка не требуется
3. Настройте профили игр и уровни яркости

---

<a name="deutsch"></a>
## Deutsch

### Überblick

**Spectra** ist ein professionelles Digital-Vibrance-Management-Werkzeug für Windows. Es nutzt native GPU-Treiber-APIs — NVIDIA DVC und AMD ADL — um Farbsättigungsprofile pro Anwendung ohne Leistungseinbußen zu automatisieren.

### Funktionen

| Funktion | Beschreibung |
|----------|--------------|
| **NVIDIA & AMD** | Volle Unterstützung über NVAPI und AMD ADL |
| **Spielprofile** | Individuelle Vibrance-Level pro Spiel |
| **Schnelltaste** | Ctrl/Alt+Taste Unterstützung — sicher bei nichtlateinischen Tastaturen |
| **Tray-Schnellpresets** | Vibrance direkt aus dem Systemtray ändern |
| **6 Sprachen** | Deutsch inklusive |
| **Systemtray** | Stiller Hintergrundbetrieb mit vollständiger Traykontrolle |

### Installation

1. `Spectra.exe` von der [Releases](../../releases) Seite herunterladen
2. Ausführen — keine Installation erforderlich
3. Spielprofile und Vibrance-Level konfigurieren

---

<a name="français"></a>
## Français

### Aperçu

**Spectra** est un outil professionnel de gestion de la vibrance numérique pour Windows. Il utilise les API natives des pilotes GPU — NVIDIA DVC et AMD ADL — pour automatiser les profils de saturation des couleurs par application, sans aucune perte de performance.

### Fonctionnalités

| Fonctionnalité | Description |
|----------------|-------------|
| **NVIDIA & AMD** | Support complet via NVAPI et AMD ADL |
| **Profils par jeu** | Niveaux de vibrance individuels par jeu |
| **Raccourci global** | Support Ctrl/Alt+touche — sûr sur les claviers cyrilliques |
| **Préréglages rapides dans le tray** | Changer la vibrance directement depuis la barre système |
| **6 langues** | Français inclus |
| **Barre système** | Fonctionnement silencieux en arrière-plan |

### Installation

1. Téléchargez `Spectra.exe` depuis la page [Releases](../../releases)
2. Exécutez — aucune installation requise
3. Configurez vos profils de jeu et niveaux de vibrance

---

<a name="nederlands"></a>
## Nederlands

### Overzicht

**Spectra** is een professionele digitale vibrance-beheerder voor Windows, ontworpen voor competitieve gamers en visuele professionals. Het maakt gebruik van native GPU-stuurprogramma-API's — NVIDIA DVC en AMD ADL — om automatisch kleurverzadigingsprofielen per applicatie toe te passen zonder prestatieverlies.

### Functies

| Functie | Beschrijving |
|---------|-------------|
| **NVIDIA & AMD** | Volledige ondersteuning via NVAPI en AMD ADL |
| **Per-game profielen** | Individuele vibrance-niveaus per game |
| **Globale sneltoets** | Ctrl/Alt+toets ondersteuning — veilig op Cyrillische toetsenborden |
| **Snelle presets in tray** | Vibrance wijzigen direct vanuit de systeembalk |
| **6 talen** | Nederlands inbegrepen |
| **Systeembalk** | Stille achtergrondwerking met volledig traybeheer |

### Installatie

1. Download `Spectra.exe` van de [Releases](../../releases) pagina
2. Uitvoeren — geen installatie vereist
3. Configureer je game-profielen en vibrance-niveaus

---

<div align="center">

**Spectra v1.9.0** — Professional Digital Vibrance Control

</div>
