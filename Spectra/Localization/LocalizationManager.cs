using System;
using System.Collections.Generic;

namespace Spectra.Localization
{
    public enum Language { English = 0, Turkish = 1, Russian = 2, German = 3, French = 4 }

    public static class LocalizationManager
    {
        public static Language Current { get; private set; } = Language.English;
        public static event EventHandler LanguageChanged;

        private static readonly Dictionary<string, string[]> _strings =
            new Dictionary<string, string[]>
        {
            // [0]=EN  [1]=TR  [2]=RU  [3]=DE  [4]=FR
            ["AppName"]          = new[]{ "Spectra",          "Spectra",             "Spectra",                   "Spectra",                "Spectra" },
            ["DesktopVibrance"]  = new[]{ "DESKTOP VIBRANCE", "MASAÜSTÜ CANLILIĞI",  "ЯРКОСТЬ РАБОЧЕГО СТОЛА",    "DESKTOP-VIBRANCE",       "VIBRANCE BUREAU" },
            ["Settings"]         = new[]{ "SETTINGS",         "AYARLAR",             "НАСТРОЙКИ",                 "EINSTELLUNGEN",          "PARAMÈTRES" },
            ["GameProfiles"]     = new[]{ "GAME PROFILES",    "OYUN PROFİLLERİ",     "ПРОФИЛИ ИГР",               "SPIELPROFILE",           "PROFILS DE JEU" },
            ["Autostart"]        = new[]{ "Launch on startup","Başlangıçta başlat",  "Запускать при старте",      "Beim Start ausführen",   "Lancer au démarrage" },
            ["PrimaryMonitor"]   = new[]{ "Primary monitor only","Yalnızca ana monitör","Только основной монитор","Nur Primärmonitor",      "Moniteur principal seulement" },
            ["NeverResize"]      = new[]{ "Never change resolution","Çözünürlüğü değiştirme","Не менять разрешение","Auflösung nie ändern", "Ne jamais changer la résolution" },
            ["Theme"]            = new[]{ "THEME",            "TEMA",                "ТЕМА",                      "DESIGN",                 "THÈME" },
            ["Dark"]             = new[]{ "Dark",             "Koyu",                "Тёмная",                    "Dunkel",                 "Sombre" },
            ["Light"]            = new[]{ "Light",            "Açık",                "Светлая",                   "Hell",                   "Clair" },
            ["Language"]         = new[]{ "LANGUAGE",         "DİL",                 "ЯЗЫК",                      "SPRACHE",                "LANGUE" },
            ["Hotkey"]           = new[]{ "TOGGLE HOTKEY",   "KISAYOL TUŞU",        "ГОРЯЧАЯ КЛАВИША",            "SCHNELLTASTE",           "RACCOURCI" },
            ["AddFile"]          = new[]{ "Add File",         "Dosya Ekle",          "Добавить файл",             "Datei hinzufügen",       "Ajouter un fichier" },
            ["BrowseRunning"]    = new[]{ "Browse Running",   "Çalışanı Bul",        "Найти процесс",             "Prozess suchen",         "Chercher processus" },
            ["Remove"]           = new[]{ "Remove",           "Kaldır",              "Удалить",                   "Entfernen",              "Supprimer" },
            ["Running"]          = new[]{ "Running",          "Çalışıyor",           "Работает",                  "Läuft",                  "En cours" },
            ["Stopped"]          = new[]{ "Stopped",          "Durdu",               "Остановлен",                "Gestoppt",               "Arrêté" },
            ["Initializing"]     = new[]{ "Initializing...",  "Başlatılıyor...",     "Инициализация...",          "Initialisierung...",      "Initialisation..." },
            ["StatusRunning"]    = new[]{ "● Running",        "● Çalışıyor",         "● Работает",                "● Läuft",                "● En cours" },
            ["StatusStopped"]    = new[]{ "● Stopped",        "● Durdu",             "● Ostановлен",              "● Gestoppt",             "● Arrêté" },
            ["HotkeyTip"]        = new[]{ "Click to change hotkey", "Değiştirmek için tıkla","Нажмите для смены","Klicken zum Ändern",     "Cliquer pour changer" },
            ["PressHotkey"]      = new[]{ "Press a key...",   "Bir tuşa basın...",   "Нажмите клавишу...",        "Taste drücken...",        "Appuyez sur une touche..." },
            ["ErrorGpuUnknown"]  = new[]{ "Unable to detect your GPU. Please ensure a compatible GPU driver (NVIDIA/AMD) is installed.", "GPU tespit edilemedi. Uyumlu bir GPU sürücüsü (NVIDIA/AMD) yüklü olduğundan emin olun.", "Не удалось определить GPU. Убедитесь, что установлен совместимый драйвер.", "GPU konnte nicht erkannt werden. Bitte stellen Sie sicher, dass ein kompatibler Treiber installiert ist.", "Impossible de détecter le GPU. Veuillez vérifier que le pilote compatible est installé." },
            ["ErrorGpuAmbiguous"]= new[]{ "Both NVIDIA and AMD drivers detected. Please uninstall unused drivers to ensure stability.", "Hem NVIDIA hem AMD sürücüsü bulundu. Kararsızlığı önlemek için kullanılmayan sürücüleri kaldırın.", "Обнаружены оба драйвера. Удалите неиспользуемый для обеспечения стабильности.", "Beide Treiber gefunden. Bitte deinstallieren Sie den nicht verwendeten Treiber.", "Les deux pilotes détectés. Désinstallez le pilote inutilisé pour la stabilité." },
            ["ErrorCaption"]     = new[]{ "Spectra — Error", "Spectra — Hata",      "Spectra — Ошибка",          "Spectra — Fehler",        "Spectra — Erreur" },
            ["RunOnce"]          = new[]{ "Spectra is already running.", "Spectra zaten çalışıyor.", "Spectra уже запущен.", "Spectra läuft bereits.", "Spectra est déjà en cours d'exécution." },
            ["IngameVibrance"]   = new[]{ "IN-GAME VIBRANCE", "OYUN CANLILIĞI",     "ЯРКОСТЬ В ИГРЕ",            "SPIELVIBRANCE",          "VIBRANCE EN JEU" },
            ["IngameResolution"] = new[]{ "IN-GAME RESOLUTION","OYUN ÇÖZÜNÜRLÜĞÜ",  "РАЗРЕШЕНИЕ В ИГРЕ",         "SPIELAUFLÖSUNG",         "RÉSOLUTION EN JEU" },
            ["BorderlessOnly"]   = new[]{ "For borderless windowed mode only", "Yalnızca çerçevesiz pencere modu", "Только для оконного режима без рамки", "Nur für randlosen Fenstermodus", "Uniquement pour le mode fenêtré sans bordure" },
            ["ChangeResIngame"]  = new[]{ "Change resolution in-game", "Oyunda çözünürlüğü değiştir", "Менять разрешение в игре", "Auflösung im Spiel ändern", "Changer la résolution en jeu" },
            ["SaveProfile"]      = new[]{ "Save",             "Kaydet",              "Сохранить",                 "Speichern",              "Enregistrer" },
            ["SettingsFor"]      = new[]{ "Settings for ",    "Ayarlar: ",           "Настройки для ",            "Einstellungen für ",     "Paramètres pour " },
            ["ReloadProc"]       = new[]{ "Reload Processes", "Süreçleri Yenile",    "Обновить процессы",         "Prozesse neu laden",     "Recharger les processus" },
            ["ProcessExplorer"]  = new[]{ "Spectra — Running Processes","Spectra — Çalışan Süreçler","Spectra — Запущенные процессы","Spectra — Laufende Prozesse","Spectra — Processus en cours" },
            ["HotkeyRegistered"] = new[]{ "Hotkey registered: ", "Kısayol kaydedildi: ", "Горячая клавиша зарегистрирована: ", "Schnelltaste gesetzt: ", "Raccourci enregistré : " },
            ["OpenSpectra"]      = new[]{ "Open Spectra",     "Spectra'yı Aç",       "Открыть Spectra",           "Spectra öffnen",         "Ouvrir Spectra" },
            ["Exit"]             = new[]{ "Exit",             "Çıkış",               "Выход",                     "Beenden",                "Quitter" },
            ["SettingsTitle"]    = new[]{ "Spectra — Settings","Spectra — Ayarlar",   "Spectra — Настройки",       "Spectra — Einstellungen", "Spectra — Paramètres" },
            ["TabGeneral"]       = new[]{ "General",          "Genel",               "Общие",                     "Allgemein",              "Général" },
            ["TabVibrance"]      = new[]{ "Vibrance",         "Canlılık",            "Яркость",                   "Vibrance",               "Vibrance" },
            ["TabHotkey"]        = new[]{ "Hotkey",           "Kısayol",             "Горячие клавиши",           "Schnelltaste",           "Raccourci" },
            ["TabAbout"]         = new[]{ "About",            "Hakkında",            "О программе",               "Über",                   "À propos" },
            ["Appearance"]       = new[]{ "Appearance",       "Görünüm",             "Внешний вид",               "Erscheinungsbild",       "Apparence" },
            ["Behavior"]         = new[]{ "Behavior",         "Davranış",            "Поведение",                 "Verhalten",              "Comportement" },
            ["MinimizeToTray"]   = new[]{ "Minimize to tray on close","Kapatınca tepside küçült","Свернуть в трей при закрытии","In Tray minimieren","Réduire dans le tray à la fermeture" },
            ["ShowNotifications"]= new[]{ "Show tray notifications","Tepsi bildirimlerini göster","Показывать уведомления","Benachrichtigungen anzeigen","Afficher les notifications" },
            ["DesktopDefault"]   = new[]{ "DESKTOP VIBRANCE DEFAULT","MASAÜSTÜ CANLILIĞI","ЯРКОСТЬ РАБОЧЕГО СТОЛА","DESKTOP-VIBRANCE","VIBRANCE BUREAU PAR DÉFAUT" },
            ["DesktopDefaultNote"]= new[]{ "Applied when no game profile is active","Oyun profili aktif olmadığında uygulanır","Применяется при отсутствии активного профиля игры","Wird angewandt, wenn kein Spielprofil aktiv ist","Appliqué quand aucun profil de jeu n'est actif" },
            ["MonitorConfig"]    = new[]{ "MONITOR CONFIGURATION","MONİTÖR YAPILANDIRMA","НАСТРОЙКА МОНИТОРА","MONITOR-KONFIGURATION","CONFIGURATION MONITEUR" },
            ["ResetDefault"]     = new[]{ "Reset","Sıfırla","Сбросить","Zurücksetzen","Réinitialiser" },
            ["GlobalHotkey"]     = new[]{ "GLOBAL VIBRANCE TOGGLE","GLOBAL CANLILIĞI DÜĞME","ГЛОБАЛЬНЫЙ ПЕРЕКЛЮЧАТЕЛЬ","GLOBALE UMSCHALTTASTE","BASCULEMENT GLOBAL" },
            ["HotkeyNote"]       = new[]{ "Press this key anywhere to toggle vibrance","Bu tuşa basarak vibrance'ı aç/kapat","Нажмите эту клавишу в любом месте для переключения","Drücken Sie diese Taste überall zum Umschalten","Appuyez sur cette touche partout pour basculer" },
            ["HotkeyBehavior"]   = new[]{ "HOTKEY BEHAVIOR","KISA YOL DAVRANIŞI","ПОВЕДЕНИЕ ГОРЯЧЕЙ КЛАВИШИ","SCHNELLTASTEN-VERHALTEN","COMPORTEMENT DU RACCOURCI" },
            ["HotkeyToggle"]     = new[]{ "Toggle — Press once to enable, press again to disable","Geçiş — Etkinleştirmek için bir kez, devre dışı bırakmak için tekrar basın","Переключение — нажмите раз для включения, ещё раз для выключения","Umschalten — Einmal drücken zum Aktivieren, erneut zum Deaktivieren","Basculer — Appuyer une fois pour activer, une nouvelle fois pour désactiver" },
            ["HotkeyHold"]       = new[]{ "Hold — Active only while key is held","Basılı tut — Tuş basılıyken aktif","Удержание — активно только пока клавиша нажата","Halten — Nur aktiv, während die Taste gedrückt ist","Maintenir — Actif uniquement tant que la touche est enfoncée" },
            ["ClearHotkey"]      = new[]{ "Clear","Temizle","Сбросить","Löschen","Effacer" },
            ["None"]             = new[]{ "None","Yok","Нет","Keine","Aucun" },
            ["AboutDesc"]        = new[]{ "Professional Digital Vibrance Controller","Profesyonel Dijital Canlılık Kontrolörü","Профессиональный контроллер цифровой яркости","Professioneller Digitaler Vibrance-Controller","Contrôleur professionnel de vibrance numérique" },
            ["AboutGpu"]         = new[]{ "✓ NVIDIA Digital Vibrance Control (NVAPI)\r\n✓ AMD Saturation Control (ADL)","✓ NVIDIA Dijital Canlılık Kontrolü (NVAPI)\r\n✓ AMD Doygunluk Kontrolü (ADL)","✓ Управление яркостью NVIDIA (NVAPI)\r\n✓ Управление насыщенностью AMD (ADL)","✓ NVIDIA Digital Vibrance Control (NVAPI)\r\n✓ AMD Sättigungssteuerung (ADL)","✓ Contrôle de vibrance numérique NVIDIA (NVAPI)\r\n✓ Contrôle de saturation AMD (ADL)" },
            ["Apply"]            = new[]{ "Apply","Uygula","Применить","Anwenden","Appliquer" },
            ["Cancel"]           = new[]{ "Cancel","İptal","Отмена","Abbrechen","Annuler" },
        };

        public static string Get(string key)
        {
            if (!_strings.TryGetValue(key, out var arr)) return key;
            int idx = (int)Current;
            return idx < arr.Length ? arr[idx] : arr[0];
        }

        public static void SetLanguage(Language lang)
        {
            Current = lang;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static string[] LanguageNames => new[] { "English", "Türkçe", "Русский", "Deutsch", "Français" };
    }
}
