<div align="center">

<img src="https://img.shields.io/badge/version-1.6.23-7B2FF7?style=for-the-badge" alt="Version">
<img src="https://img.shields.io/badge/platform-Windows-0078D4?style=for-the-badge&logo=windows" alt="Platform">
<img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-76B900?style=for-the-badge" alt="GPU">
<img src="https://img.shields.io/badge/.NET-4.8-512BD4?style=for-the-badge" alt=".NET">

# ⚡ Spectra

**Professional Digital Vibrance Controller for NVIDIA & AMD**

</div>

---

## 🌐 Languages / Diller / Языки / Sprachen / Langues

- [English](#english)
- [Türkçe](#türkçe)
- [Русский](#русский)
- [Deutsch](#deutsch)
- [Français](#français)

---

<a name="english"></a>
## English

### Overview

**Spectra** is a professional-grade digital vibrance management utility for Windows, designed with competitive gamers and visual professionals in mind. It leverages native GPU driver APIs — NVIDIA's Digital Vibrance Control (DVC) and AMD's Display Library (ADL) — to automate per-application color saturation profiles with zero performance overhead.

When you launch a configured game, Spectra instantly applies your custom vibrance settings. When you exit, it restores your desktop profile seamlessly.

### Features

| Feature | Description |
|---------|-------------|
| **NVIDIA & AMD Support** | Full support via NVAPI and AMD ADL driver interfaces |
| **Per-Game Profiles** | Individual vibrance levels for each game |
| **Global Hotkey** | Instantly toggle vibrance on/off (default: F9) |
| **Dark / Light Theme** | Professional UI with switchable themes |
| **5-Language Interface** | English, Turkish, Russian, German, French |
| **Resolution Switching** | Auto-switch resolution per game (windowed/borderless) |
| **System Tray** | Runs silently in background with tray access |
| **Startup Integration** | Optional Windows startup registration |
| **Primary Monitor Control** | Optionally limit vibrance to primary display only |

### Requirements

- Windows 10 / 11 (64-bit recommended)
- NVIDIA GPU with official driver **OR** AMD GPU with official driver
- .NET Framework 4.8 (included in Windows 10/11)

### Installation

1. Download `Spectra.exe` from the [Releases](../../releases) page
2. Run `Spectra.exe` — no installation required
3. Configure your game profiles and preferred vibrance levels
4. Optionally enable **Launch on startup** for automatic background operation

### Usage

1. **Desktop Vibrance** — Set your preferred desktop color level using the slider
2. **Add Game** — Click *Add File* to browse for an executable, or *Browse Running* to pick from active processes
3. **Profile Settings** — Double-click any game to configure its vibrance level and optional resolution
4. **Hotkey** — Click the hotkey button and press any key to set a global vibrance toggle
5. **Theme & Language** — Switch between Dark/Light themes and 5 languages in the Settings panel

### Build from Source

```
git clone https://github.com/X1NPAR1/Spectra.git
cd Spectra
dotnet build Spectra/Spectra.csproj --configuration Release
```

---

<a name="türkçe"></a>
## Türkçe

### Genel Bakış

**Spectra**, rekabetçi oyuncular ve görsel profesyoneller için tasarlanmış, Windows platformunda çalışan profesyonel bir dijital canlılık (vibrance) yönetim aracıdır. NVIDIA'nın Dijital Canlılık Kontrolü (DVC) ve AMD'nin Görüntü Kütüphanesi (ADL) sürücü API'lerini kullanarak, uygulama başına renk doygunluğu profillerini sıfır performans kaybıyla otomatikleştirir.

Yapılandırılmış bir oyunu başlattığınızda Spectra, özel canlılık ayarlarınızı anında uygular. Oyundan çıktığınızda ise masaüstü profilinizi sorunsuz biçimde geri yükler.

### Özellikler

| Özellik | Açıklama |
|---------|----------|
| **NVIDIA & AMD Desteği** | NVAPI ve AMD ADL sürücü arayüzleri aracılığıyla tam destek |
| **Oyun Başına Profil** | Her oyun için ayrı vibrance seviyesi |
| **Global Kısayol Tuşu** | Vibrance'ı anında aç/kapat (varsayılan: F9) |
| **Koyu / Açık Tema** | Değiştirilebilir profesyonel arayüz |
| **5 Dil Desteği** | Türkçe dahil 5 dil |
| **Çözünürlük Değişimi** | Oyun başına otomatik çözünürlük geçişi |
| **Sistem Tepsisi** | Arka planda sessiz çalışma |
| **Başlangıç Entegrasyonu** | İsteğe bağlı Windows başlangıcına kayıt |

### Sistem Gereksinimleri

- Windows 10 / 11
- NVIDIA veya AMD resmi sürücülü ekran kartı
- .NET Framework 4.8

### Kurulum

1. [Releases](../../releases) sayfasından `Spectra.exe` dosyasını indirin
2. `Spectra.exe` dosyasını çalıştırın — kurulum gerekmez
3. Oyun profillerinizi ve vibrance seviyelerinizi yapılandırın

---

<a name="русский"></a>
## Русский

### Обзор

**Spectra** — профессиональный инструмент управления цифровой яркостью (Digital Vibrance) для Windows, разработанный для соревновательных геймеров и специалистов по визуализации. Программа использует нативные API драйверов GPU — NVIDIA DVC и AMD ADL — для автоматического применения профилей насыщенности цветов для каждого приложения без нагрузки на производительность.

### Возможности

| Функция | Описание |
|---------|----------|
| **NVIDIA и AMD** | Полная поддержка через NVAPI и AMD ADL |
| **Профили для игр** | Индивидуальные уровни яркости для каждой игры |
| **Горячая клавиша** | Мгновенное переключение яркости (F9 по умолчанию) |
| **Тёмная / Светлая тема** | Профессиональный интерфейс с переключаемыми темами |
| **5 языков интерфейса** | Русский в том числе |
| **Системный трей** | Тихая фоновая работа |

### Требования

- Windows 10 / 11
- NVIDIA или AMD с официальным драйвером
- .NET Framework 4.8

### Установка

1. Скачайте `Spectra.exe` со страницы [Releases](../../releases)
2. Запустите — установка не требуется
3. Настройте профили игр и уровни яркости

---

<a name="deutsch"></a>
## Deutsch

### Überblick

**Spectra** ist ein professionelles Digital-Vibrance-Management-Werkzeug für Windows, entwickelt für kompetitive Gamer und visuelle Fachleute. Es nutzt native GPU-Treiber-APIs — NVIDIA DVC und AMD ADL — um pro-Anwendung Farbsättigungsprofile ohne Leistungseinbußen zu automatisieren.

### Funktionen

| Funktion | Beschreibung |
|----------|--------------|
| **NVIDIA & AMD** | Volle Unterstützung über NVAPI und AMD ADL |
| **Spielprofile** | Individuelle Vibrance-Level pro Spiel |
| **Schnelltaste** | Sofortiges Umschalten (Standard: F9) |
| **Dunkel / Hell Theme** | Umschaltbare professionelle Oberfläche |
| **5 Sprachen** | Deutsch inklusive |
| **Systemtray** | Stiller Hintergrundbetrieb |

### Anforderungen

- Windows 10 / 11
- NVIDIA oder AMD GPU mit offiziellem Treiber
- .NET Framework 4.8

### Installation

1. `Spectra.exe` von der [Releases](../../releases) Seite herunterladen
2. Ausführen — keine Installation erforderlich
3. Spielprofile und Vibrance-Level konfigurieren

---

<a name="français"></a>
## Français

### Aperçu

**Spectra** est un outil professionnel de gestion de la vibrance numérique pour Windows, conçu pour les joueurs compétitifs et les professionnels visuels. Il utilise les API natives des pilotes GPU — NVIDIA DVC et AMD ADL — pour automatiser les profils de saturation des couleurs par application, sans aucune perte de performance.

### Fonctionnalités

| Fonctionnalité | Description |
|----------------|-------------|
| **NVIDIA & AMD** | Support complet via NVAPI et AMD ADL |
| **Profils par jeu** | Niveaux de vibrance individuels par jeu |
| **Raccourci global** | Basculement instantané (défaut : F9) |
| **Thème Sombre / Clair** | Interface professionnelle commutable |
| **5 langues** | Français inclus |
| **Barre système** | Fonctionnement silencieux en arrière-plan |

### Configuration requise

- Windows 10 / 11
- GPU NVIDIA ou AMD avec pilote officiel
- .NET Framework 4.8

### Installation

1. Téléchargez `Spectra.exe` depuis la page [Releases](../../releases)
2. Exécutez — aucune installation requise
3. Configurez vos profils de jeu et niveaux de vibrance

---

<div align="center">

**Spectra v1.6.23** — Professional Digital Vibrance Control

</div>
