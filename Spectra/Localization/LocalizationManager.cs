using System;
using System.Collections.Generic;

namespace Spectra.Localization
{
    // [0]=EN  [1]=TR  [2]=RU  [3]=DE  [4]=FR  [5]=NL
    public enum Language { English = 0, Turkish = 1, Russian = 2, German = 3, French = 4, Dutch = 5 }

    public static class LocalizationManager
    {
        public static Language Current { get; private set; } = Language.English;
        public static event EventHandler LanguageChanged;

        private static readonly Dictionary<string, string[]> S = new Dictionary<string, string[]>
        {
            // ── Main form labels ──────────────────────────────────────────
            ["AppName"]          = new[]{"Spectra","Spectra","Spectra","Spectra","Spectra","Spectra"},
            ["DesktopVibrance"]  = new[]{"DESKTOP VIBRANCE","MASAÜSTÜ CANLILIĞI","ЯРКОСТЬ ЭКРАНА","DESKTOP-VIBRANCE","VIBRANCE BUREAU","BUREAUBLADKLEUR"},
            ["Settings"]         = new[]{"SETTINGS","AYARLAR","НАСТРОЙКИ","EINSTELLUNGEN","PARAMÈTRES","INSTELLINGEN"},
            ["GameProfiles"]     = new[]{"GAME PROFILES","OYUN PROFİLLERİ","ПРОФИЛИ ИГР","SPIELPROFILE","PROFILS DE JEU","SPELPROFIELEN"},

            // ── Checkboxes (main) ─────────────────────────────────────────
            ["Autostart"]        = new[]{"Launch on startup","Başlangıçta başlat","Запускать при старте","Beim Start ausführen","Lancer au démarrage","Opstarten bij start"},
            ["PrimaryMonitor"]   = new[]{"Primary monitor only","Yalnızca ana monitör","Только основной монитор","Nur Primärmonitor","Moniteur principal seulement","Alleen primair scherm"},
            ["NeverResize"]      = new[]{"Never change resolution","Çözünürlüğü değiştirme","Не менять разрешение","Auflösung nie ändern","Ne jamais changer la résolution","Resolutie niet wijzigen"},

            // ── Quick row (short labels) ──────────────────────────────────
            ["LangLabel"]        = new[]{"LANGUAGE","DİL","ЯЗЫК","SPRACHE","LANGUE","TAAL"},
            ["HotkeyLabel"]      = new[]{"HOTKEY","KISA YOL","КЛАВИША","TASTE","TOUCHE","SNELTOETS"},

            // ── Buttons (main) ────────────────────────────────────────────
            ["AddFile"]          = new[]{"Add File","Dosya Ekle","Добавить файл","Datei hinzufügen","Ajouter un fichier","Bestand toevoegen"},
            ["BrowseRunning"]    = new[]{"Browse Running","Çalışanı Bul","Найти процесс","Prozess suchen","Chercher processus","Bladeren"},
            ["Remove"]           = new[]{"Remove","Kaldır","Удалить","Entfernen","Supprimer","Verwijderen"},

            // ── Status bar ────────────────────────────────────────────────
            ["StatusRunning"]    = new[]{"● Running","● Çalışıyor","● Работает","● Läuft","● En cours","● Actief"},
            ["StatusStopped"]    = new[]{"● Stopped","● Durdu","● Остановлен","● Gestoppt","● Arrêté","● Gestopt"},
            ["StatusInit"]       = new[]{"Initializing...","Başlatılıyor...","Инициализация...","Initialisierung...","Initialisation...","Initialiseren..."},

            // ── Tray ─────────────────────────────────────────────────────
            ["OpenSpectra"]      = new[]{"Open Spectra","Spectra'yı Aç","Открыть Spectra","Spectra öffnen","Ouvrir Spectra","Spectra openen"},
            ["TrayToggle"]       = new[]{"Toggle Vibrance","Canlılığı Değiştir","Переключить яркость","Vibrance umschalten","Basculer la vibrance","Vibrance wisselen"},
            ["Exit"]             = new[]{"Exit","Çıkış","Выход","Beenden","Quitter","Afsluiten"},

            // ── HotkeyManager ─────────────────────────────────────────────
            ["PressHotkey"]      = new[]{"Press a key...","Bir tuşa basın...","Нажмите клавишу...","Taste drücken...","Appuyez sur une touche...","Druk op een toets..."},
            ["None"]             = new[]{"None","Yok","Нет","Keine","Aucun","Geen"},

            // ── Error messages ────────────────────────────────────────────
            ["ErrorGpuUnknown"]  = new[]{"Unable to detect your GPU. Please ensure a compatible GPU driver (NVIDIA/AMD) is installed.","GPU tespit edilemedi. Uyumlu bir GPU sürücüsü (NVIDIA/AMD) yüklü olduğundan emin olun.","Не удалось определить GPU. Убедитесь, что установлен совместимый драйвер.","GPU konnte nicht erkannt werden. Bitte stellen Sie sicher, dass ein kompatibler Treiber installiert ist.","Impossible de détecter le GPU. Veuillez vérifier que le pilote compatible est installé.","GPU niet gevonden. Zorg dat een compatibel stuurprogramma (NVIDIA/AMD) is geïnstalleerd."},
            ["ErrorGpuAmbiguous"]= new[]{"Both NVIDIA and AMD drivers detected. Uninstall unused drivers for stability.","Hem NVIDIA hem AMD sürücüsü bulundu. Kararsızlığı önlemek için kullanılmayan sürücüleri kaldırın.","Обнаружены оба драйвера. Удалите неиспользуемый для стабильности.","Beide Treiber gefunden. Deinstallieren Sie den ungenutzten für Stabilität.","Les deux pilotes détectés. Désinstallez le pilote inutilisé.","Beide stuurprogramma's gevonden. Verwijder het ongebruikte voor stabiliteit."},
            ["ErrorCaption"]     = new[]{"Spectra — Error","Spectra — Hata","Spectra — Ошибка","Spectra — Fehler","Spectra — Erreur","Spectra — Fout"},
            ["RunOnce"]          = new[]{"Spectra is already running.","Spectra zaten çalışıyor.","Spectra уже запущен.","Spectra läuft bereits.","Spectra est déjà en cours d'exécution.","Spectra is al actief."},

            // ── Game settings form ────────────────────────────────────────
            ["IngameVibrance"]   = new[]{"IN-GAME VIBRANCE","OYUN CANLILIĞI","ЯРКОСТЬ В ИГРЕ","SPIELVIBRANCE","VIBRANCE EN JEU","IN-GAME KLEUR"},
            ["IngameResolution"] = new[]{"IN-GAME RESOLUTION","OYUN ÇÖZÜNÜRLÜĞÜ","РАЗРЕШЕНИЕ В ИГРЕ","SPIELAUFLÖSUNG","RÉSOLUTION EN JEU","IN-GAME RESOLUTIE"},
            ["BorderlessOnly"]   = new[]{"For borderless windowed mode only","Yalnızca çerçevesiz pencere modu","Только для оконного режима без рамки","Nur für randlosen Fenstermodus","Uniquement pour le mode fenêtré sans bordure","Alleen voor randloos venstermodus"},
            ["ChangeResIngame"]  = new[]{"Change resolution in-game","Oyunda çözünürlüğü değiştir","Менять разрешение в игре","Auflösung im Spiel ändern","Changer la résolution en jeu","Resolutie in game wijzigen"},
            ["SaveProfile"]      = new[]{"Save","Kaydet","Сохранить","Speichern","Enregistrer","Opslaan"},
            ["SettingsFor"]      = new[]{"Settings for ","Ayarlar: ","Настройки для ","Einstellungen für ","Paramètres pour ","Instellingen voor "},

            // ── Process explorer ──────────────────────────────────────────
            ["ProcessExplorer"]  = new[]{"Spectra — Running Processes","Spectra — Çalışan Süreçler","Spectra — Запущенные процессы","Spectra — Laufende Prozesse","Spectra — Processus en cours","Spectra — Actieve processen"},
            ["ReloadProc"]       = new[]{"Reload","Yenile","Обновить","Neu laden","Recharger","Herladen"},

            // ── Settings dialog ───────────────────────────────────────────
            ["SettingsTitle"]    = new[]{"Spectra — Settings","Spectra — Ayarlar","Spectra — Настройки","Spectra — Einstellungen","Spectra — Paramètres","Spectra — Instellingen"},
            ["TabBehavior"]      = new[]{"Behavior","Davranış","Поведение","Verhalten","Comportement","Gedrag"},
            ["TabDisplay"]       = new[]{"Display","Görüntü","Дисплей","Anzeige","Affichage","Scherm"},
            ["TabData"]          = new[]{"Profiles","Profiller","Профили","Profile","Profils","Profielen"},
            ["TabAbout"]         = new[]{"About","Hakkında","О программе","Über","À propos","Over"},

            // Behavior tab
            ["BehaviorSection"]  = new[]{"STARTUP & BEHAVIOR","BAŞLANGIÇ VE DAVRANIŞ","ЗАПУСК И ПОВЕДЕНИЕ","START & VERHALTEN","DÉMARRAGE ET COMPORTEMENT","START & GEDRAG"},
            ["MinimizeToTray"]   = new[]{"Minimize to tray when closed","Kapatınca tepside küçült","Свернуть в трей при закрытии","In Tray minimieren","Réduire dans le tray à la fermeture","Minimaliseren naar tray bij sluiten"},
            ["ShowNotifications"]= new[]{"Show tray notifications","Tepsi bildirimlerini göster","Показывать уведомления","Benachrichtigungen anzeigen","Afficher les notifications","Meldingen in tray weergeven"},
            ["ApplyDelay"]       = new[]{"Vibrance apply delay","Canlılık uygulama gecikmesi","Задержка применения яркости","Verzögerung der Vibrance-Anwendung","Délai d'application de la vibrance","Vertraging toepassen kleur"},
            ["ApplyDelayNote"]   = new[]{"Wait before applying vibrance when a game launches (ms)","Oyun başladığında canlılık uygulamadan önce bekle (ms)","Ожидание перед применением яркости при запуске игры (мс)","Warten bevor Vibrance angewendet wird (ms)","Attendre avant d'appliquer la vibrance au lancement (ms)","Wachten voor toepassing bij gameopstart (ms)"},

            // Display tab
            ["DisplaySection"]   = new[]{"MONITOR SETTINGS","MONİTÖR AYARLARI","НАСТРОЙКИ МОНИТОРА","MONITOR-EINSTELLUNGEN","PARAMÈTRES MONITEUR","MONITORINSTELLINGEN"},
            ["MonitorTarget"]    = new[]{"Apply to","Uygula:","Применять к:","Anwenden auf:","Appliquer à:","Toepassen op:"},
            ["MonitorAll"]       = new[]{"All Monitors","Tüm Monitörler","Все мониторы","Alle Monitore","Tous les moniteurs","Alle schermen"},
            ["MonitorPrimary"]   = new[]{"Primary Monitor Only","Yalnızca Ana Monitör","Только основной монитор","Nur Primärmonitor","Moniteur principal seulement","Alleen primair scherm"},
            ["ResolutionSection"]= new[]{"RESOLUTION BEHAVIOR","ÇÖZÜNÜRLÜK DAVRANIŞI","ПОВЕДЕНИЕ РАЗРЕШЕНИЯ","AUFLÖSUNGSVERHALTEN","COMPORTEMENT RÉSOLUTION","RESOLUTIEGEDRAG"},
            ["ResetOnExit"]      = new[]{"Reset vibrance to default on exit","Çıkışta canlılığı sıfırla","Сбрасывать яркость при выходе","Vibrance beim Beenden zurücksetzen","Réinitialiser la vibrance à la sortie","Kleur herstellen bij afsluiten"},

            // Data tab
            ["ProfilesSection"]  = new[]{"PROFILE MANAGEMENT","PROFİL YÖNETİMİ","УПРАВЛЕНИЕ ПРОФИЛЯМИ","PROFILVERWALTUNG","GESTION DES PROFILS","PROFIELBEHEER"},
            ["ProfileCount"]     = new[]{"Saved profiles:","Kayıtlı profil:","Сохранённых профилей:","Gespeicherte Profile:","Profils enregistrés:","Opgeslagen profielen:"},
            ["ExportProfiles"]   = new[]{"Export to file","Dosyaya aktar","Экспортировать в файл","In Datei exportieren","Exporter vers un fichier","Exporteren naar bestand"},
            ["ImportProfiles"]   = new[]{"Import from file","Dosyadan yükle","Импортировать из файла","Aus Datei importieren","Importer depuis un fichier","Importeren uit bestand"},
            ["ClearProfiles"]    = new[]{"Clear all profiles","Tüm profilleri sil","Очистить все профили","Alle Profile löschen","Supprimer tous les profils","Alle profielen wissen"},
            ["ResetAll"]         = new[]{"Reset all settings","Tüm ayarları sıfırla","Сбросить все настройки","Alle Einstellungen zurücksetzen","Réinitialiser tous les paramètres","Alle instellingen resetten"},
            ["OpenLogFolder"]    = new[]{"Open log folder","Günlük klasörünü aç","Открыть папку журналов","Protokollordner öffnen","Ouvrir le dossier des logs","Logmap openen"},
            ["DataNote"]         = new[]{"Profiles are stored in %APPDATA%\\Spectra","Profiller %APPDATA%\\Spectra klasöründe","Профили хранятся в %APPDATA%\\Spectra","Profile gespeichert in %APPDATA%\\Spectra","Profils stockés dans %APPDATA%\\Spectra","Profielen opgeslagen in %APPDATA%\\Spectra"},
            ["ConfirmClear"]     = new[]{"Clear all profiles? This cannot be undone.","Tüm profiller silinsin mi? Bu geri alınamaz.","Очистить все профили? Это нельзя отменить.","Alle Profile löschen? Dies kann nicht rückgängig gemacht werden.","Effacer tous les profils? Cette action est irréversible.","Alle profielen wissen? Dit kan niet ongedaan worden gemaakt."},
            ["ConfirmReset"]     = new[]{"Reset all settings to defaults?","Tüm ayarlar varsayılanlara sıfırlansın mı?","Сбросить все настройки до значений по умолчанию?","Alle Einstellungen auf Standardwerte zurücksetzen?","Réinitialiser tous les paramètres aux valeurs par défaut?","Alle instellingen naar standaard resetten?"},

            // About tab
            ["AboutDesc"]        = new[]{"Professional Digital Vibrance Controller","Profesyonel Dijital Canlılık Kontrolörü","Профессиональный контроллер цифровой яркости","Professioneller Digitaler Vibrance-Controller","Contrôleur professionnel de vibrance numérique","Professionele digitale kleurcontroller"},
            ["AboutSupport"]     = new[]{"GPU Support","GPU Desteği","Поддержка GPU","GPU-Unterstützung","Support GPU","GPU-ondersteuning"},
            ["AboutGpuLine1"]    = new[]{"✓  NVIDIA — Digital Vibrance Control (NVAPI)","✓  NVIDIA — Dijital Canlılık Kontrolü (NVAPI)","✓  NVIDIA — Digital Vibrance Control (NVAPI)","✓  NVIDIA — Digital Vibrance Control (NVAPI)","✓  NVIDIA — Contrôle de vibrance numérique (NVAPI)","✓  NVIDIA — Digital Vibrance Control (NVAPI)"},
            ["AboutGpuLine2"]    = new[]{"✓  AMD — Saturation Control (ADL 32/64-bit)","✓  AMD — Doygunluk Kontrolü (ADL 32/64-bit)","✓  AMD — Saturation Control (ADL 32/64-bit)","✓  AMD — Sättigungssteuerung (ADL 32/64-bit)","✓  AMD — Contrôle de saturation (ADL 32/64-bit)","✓  AMD — Verzadigingsregeling (ADL 32/64-bit)"},
            ["ViewOnGitHub"]     = new[]{"★  View on GitHub","★  GitHub'da Görüntüle","★  Просмотр на GitHub","★  Auf GitHub anzeigen","★  Voir sur GitHub","★  Bekijk op GitHub"},
            ["OpenLogFolderShort"]= new[]{"Open logs","Günlükleri aç","Открыть журналы","Logs öffnen","Ouvrir les logs","Logs openen"},
            ["SysInfo"]          = new[]{"System Information","Sistem Bilgisi","Системная информация","Systeminformationen","Informations système","Systeeminformatie"},

            // Common buttons
            ["Apply"]            = new[]{"Apply","Uygula","Применить","Anwenden","Appliquer","Toepassen"},
            ["Cancel"]           = new[]{"Cancel","İptal","Отмена","Abbrechen","Annuler","Annuleren"},
            ["OK"]               = new[]{"OK","Tamam","ОК","OK","OK","OK"},
        };

        public static string Get(string key)
        {
            if (!S.TryGetValue(key, out var arr)) return key;
            int idx = (int)Current;
            return idx < arr.Length ? arr[idx] : arr[0];
        }

        public static void SetLanguage(Language lang)
        {
            Current = lang;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static string[] LanguageNames
            => new[] { "English", "Türkçe", "Русский", "Deutsch", "Français", "Nederlands" };
    }
}
