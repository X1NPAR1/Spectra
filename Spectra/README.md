<p align="center">
  <img src="setting.ico" width="64" height="64" alt="Spectra icon"/>
</p>

<h1 align="center">Spectra</h1>
<p align="center">
  <b>Professional Digital Vibrance Control for Windows</b><br/>
  NVIDIA &amp; AMD · Per-monitor · Profiles · Schedule · Brightness/Contrast
</p>

<p align="center">
  <a href="https://github.com/X1NPAR1/Spectra/releases/latest"><img src="https://img.shields.io/github/v/release/X1NPAR1/Spectra?style=flat-square&label=latest" alt="Latest Release"/></a>
  <img src="https://img.shields.io/badge/platform-Windows-blue?style=flat-square"/>
  <img src="https://img.shields.io/badge/.NET-4.8-purple?style=flat-square"/>
  <img src="https://img.shields.io/badge/GPU-NVIDIA%20%7C%20AMD-green?style=flat-square"/>
</p>

---

## Features

| Feature | Description |
|---|---|
| **Per-monitor vibrance** | Independent vibrance slider for each connected display |
| **Brightness & Contrast** | One-click gamma control with hardware-level precision |
| **Game Profiles** | Auto-switch vibrance when a game gains focus |
| **Auto detection** | Detect the foreground full-screen process in one click |
| **Day/Night schedule** | Automatically lower vibrance at night, restore at dawn |
| **Hotkey toggle** | Toggle vibrance on/off (default: F9, fully rebindable) |
| **System tray** | Minimize to tray, quick-preset context menu |
| **Profile import/export** | JSON and XML format support |
| **6 languages** | English, Turkish, Russian, German, French, Dutch |

## Requirements

- Windows 10 / 11 (64-bit recommended, x86 build)
- .NET Framework 4.8 (pre-installed on Windows 10 v1903+)
- NVIDIA GPU with NVAPI support **or** AMD GPU with ADL support

## Installation

1. Download `Spectra.exe` from the [latest release](https://github.com/X1NPAR1/Spectra/releases/latest).
2. Run the executable — no installer required.
3. Optionally enable **Start with Windows** in the Settings dialog.

> **Upgrading from v2.1.x?** Settings are automatically migrated from the old `vibranceGUI` folder on first launch.

## Usage

### Main Window

- **Vibrance sliders** — one per connected monitor. Drag to adjust; changes apply instantly.
- **Presets** — Default / Low / High / Max for quick switching.
- **Brightness & Contrast** — Sliders adjust the system gamma ramp. Click **Reset** to restore neutral values.
- **Game Profiles** — Click **Add** to browse for an executable, **Browse Running** to pick a live process, or **Detect Game** to auto-capture the focused full-screen app.
- **Hotkey** — Click the hotkey button and press any key to rebind. Function keys are registered directly; other keys get an automatic Ctrl modifier to avoid conflicts.

### Settings Dialog

| Tab | Contents |
|---|---|
| **Behavior** | Autostart, minimize-to-tray, notifications, apply delay, day/night schedule |
| **Display** | Monitor targeting (All / Primary / Specific), resolution-switch behavior |
| **Data** | Profile list, import/export/clear, log folder, reset all |
| **About** | GPU info, system info, GitHub link |

### System Tray

Right-click the tray icon for quick presets, toggle vibrance, or exit.

## Settings Location

All settings are stored in:
```
%AppData%\Spectra\
  Spectra.ini          ← vibrance levels, hotkey, behavior flags
  applicationData.xml  ← game profiles
  spectra.log          ← error log
```

## Building from Source

```
Requirements: Visual Studio 2019+ with .NET Framework 4.8 workload

msbuild Spectra.csproj /p:Configuration=Release /p:Platform=x86
```

Output: `bin\x86\Release\Spectra.exe`

## Changelog

### v2.2.0 — 2026-06-02
- **Light theme** — clean white/light UI replacing the dark palette
- **Settings path fix** — migrated from legacy `vibranceGUI` folder to `Spectra`; automatic migration on first run
- **Backend hardening** — directory is always created before writing settings; legacy data is preserved

### v2.1.0 — 2026-06-02
- Per-monitor independent vibrance sliders
- Brightness & Contrast control with one-click reset
- Automatic day/night schedule
- Light/Dark theme toggle (subsequently replaced by fixed light theme in v2.2)
- Auto game detection and Gaming Mode
- Profile import/export (JSON/XML)
- Minimize to tray on close + notification

### v1.9.x
- Vibrance engine fixes, compact UI, settings polish, localization improvements

## License

MIT © 2026 X1NPAR1
