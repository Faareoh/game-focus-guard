namespace FocusTool.Ui;

/// <summary>
/// Centralized localization. All user-visible strings go through here.
/// </summary>
internal static class Strings
{
    public static Language Current { get; private set; } = Language.EN;

    /// <summary>Fired after <see cref="Current"/> changes.</summary>
    public static event Action? LanguageChanged;

    public static void SetLanguage(Language lang)
    {
        if (Current == lang)
        {
            return;
        }

        Current = lang;
        LanguageChanged?.Invoke();
    }

    // ── Form / Controls ───────────────────────────────────────────────────────

    public static string AppTitle => Current switch
    {
        Language.FR => "Game Focus Guard",
        Language.ZH => "游戏防失焦工具",
        _ => "Game Focus Guard"
    };

    public static string ToggleButton => Current switch
    {
        Language.FR => "Activer / Désactiver la protection sur la fenêtre repérée",
        Language.ZH => "对候选前台窗口启用/关闭防失焦",
        _ => "Enable / Disable Focus Guard on candidate window"
    };

    public static string RebindHotKeyButton => Current switch
    {
        Language.FR => "Modifier raccourci",
        Language.ZH => "改热键",
        _ => "Change Hotkey"
    };

    public static string CancelRebindButton => Current switch
    {
        Language.FR => "Annuler",
        Language.ZH => "取消重绑",
        _ => "Cancel Rebind"
    };

    public static string LanguageLabel => Current switch
    {
        Language.FR => "Langue :",
        Language.ZH => "语言：",
        _ => "Language:"
    };

    // ── Label prefixes ────────────────────────────────────────────────────────

    public static string CurrentHotKeyPrefix => Current switch
    {
        Language.FR => "Raccourci actuel",
        Language.ZH => "当前热键",
        _ => "Current Hotkey"
    };

    public static string HotKeyWaiting => Current switch
    {
        Language.FR => "Raccourci actuel : En attente d'une combinaison… (Échap = annuler)",
        Language.ZH => "当前热键: 等待新的组合键… (Esc 取消)",
        _ => "Current Hotkey: Waiting for new combination… (Esc to cancel)"
    };

    public static string CandidateWindowPrefix => Current switch
    {
        Language.FR => "Fenêtre repérée",
        Language.ZH => "候选窗口",
        _ => "Candidate Window"
    };

    // ── Status messages (Form1) ───────────────────────────────────────────────

    public static string Initializing => Current switch
    {
        Language.FR => "Initialisation",
        Language.ZH => "初始化中",
        _ => "Initializing"
    };

    public static string WaitingForNonModifier => Current switch
    {
        Language.FR => "En attente d'une combinaison incluant une touche non-modificatrice",
        Language.ZH => "等待包含非修饰键的组合键",
        _ => "Waiting for a non-modifier key combination"
    };

    public static string HotkeyRebindCancelled => Current switch
    {
        Language.FR => "Modification du raccourci annulée",
        Language.ZH => "热键重绑已取消",
        _ => "Hotkey rebind cancelled"
    };

    public static string PressNewHotkey => Current switch
    {
        Language.FR => "Appuyez sur une nouvelle combinaison de touches. Échap pour annuler.",
        Language.ZH => "请按下新的组合键，Esc 取消。",
        _ => "Press a new hotkey combination. Esc cancels."
    };

    public static string Unavailable => Current switch
    {
        Language.FR => "(indisponible)",
        Language.ZH => "(不可用)",
        _ => "(unavailable)"
    };

    // ── HookController messages ───────────────────────────────────────────────

    public static string Idle => Current switch
    {
        Language.FR => "Inactif",
        Language.ZH => "空闲",
        _ => "Idle"
    };

    public static string NoUsableTarget => Current switch
    {
        Language.FR => "Aucune fenêtre cible utilisable",
        Language.ZH => "没有可用的目标窗口",
        _ => "No usable target window"
    };

    public static string DisabledCurrentTarget => Current switch
    {
        Language.FR => "Cible actuelle désactivée",
        Language.ZH => "已禁用当前目标",
        _ => "Disabled current target"
    };

    public static string Disabled => Current switch
    {
        Language.FR => "Désactivé",
        Language.ZH => "已禁用",
        _ => "Disabled"
    };

    public static string Enabled => Current switch
    {
        Language.FR => "Activé",
        Language.ZH => "已启用",
        _ => "Enabled"
    };

    public static string FailedRootWindow => Current switch
    {
        Language.FR => "Échec de la résolution de la fenêtre racine",
        Language.ZH => "无法解析根窗口",
        _ => "Failed to resolve root window"
    };

    public static string FailedProcessOrThread => Current switch
    {
        Language.FR => "Échec de la résolution du processus ou du thread",
        Language.ZH => "无法解析进程或线程",
        _ => "Failed to resolve process or thread"
    };

    public static string None => Current switch
    {
        Language.FR => "Aucun",
        Language.ZH => "无",
        _ => "None"
    };

    public static string Untitled => Current switch
    {
        Language.FR => "(sans titre)",
        Language.ZH => "(无标题)",
        _ => "(untitled)"
    };

    public static string Unknown => Current switch
    {
        Language.FR => "(inconnu)",
        Language.ZH => "(未知)",
        _ => "(unknown)"
    };

    // ── HotKeyService messages ────────────────────────────────────────────────

    public static string Unregistered => Current switch
    {
        Language.FR => "Non enregistré",
        Language.ZH => "未注册",
        _ => "Unregistered"
    };

    public static string InvalidHotkey => Current switch
    {
        Language.FR => "Raccourci invalide",
        Language.ZH => "无效的热键",
        _ => "Invalid hotkey"
    };

    public static string RestoredPreviousBinding => Current switch
    {
        Language.FR => "Liaison précédente restaurée",
        Language.ZH => "已恢复上一个绑定",
        _ => "Restored previous binding"
    };

    public static string FailedRestorePreviousBinding => Current switch
    {
        Language.FR => "Échec de la restauration de la liaison précédente",
        Language.ZH => "恢复上一个绑定失败",
        _ => "Failed to restore previous binding"
    };

    // ── HotKeySettingsStore messages ──────────────────────────────────────────

    public static string UsingDefaultHotkey => Current switch
    {
        Language.FR => "Utilisation du raccourci par défaut",
        Language.ZH => "使用默认热键",
        _ => "Using default hotkey"
    };

    public static string SettingsFileInvalid => Current switch
    {
        Language.FR => "Fichier de paramètres invalide, raccourci par défaut utilisé",
        Language.ZH => "设置文件无效，使用默认热键",
        _ => "Settings file invalid, using default hotkey"
    };

    public static string SavedHotkeyInvalid => Current switch
    {
        Language.FR => "Raccourci sauvegardé invalide, raccourci par défaut utilisé",
        Language.ZH => "已保存的热键无效，使用默认热键",
        _ => "Saved hotkey invalid, using default hotkey"
    };

    public static string LoadedSavedHotkey => Current switch
    {
        Language.FR => "Raccourci sauvegardé chargé",
        Language.ZH => "已加载保存的热键",
        _ => "Loaded saved hotkey"
    };

    // ── Status-box field labels ───────────────────────────────────────────────

    public static string FieldHotkey => Current switch
    {
        Language.FR => "Raccourci",
        Language.ZH => "热键",
        _ => "Hotkey"
    };

    public static string FieldHotkeyRegistered => Current switch
    {
        Language.FR => "Raccourci enregistré",
        Language.ZH => "热键已注册",
        _ => "Hotkey Registered"
    };

    public static string FieldHotkeyStatus => Current switch
    {
        Language.FR => "État du raccourci",
        Language.ZH => "热键状态",
        _ => "Hotkey Status"
    };

    public static string FieldUiStatus => Current switch
    {
        Language.FR => "État UI",
        Language.ZH => "界面状态",
        _ => "UI Status"
    };

    public static string FieldEnabled => Current switch
    {
        Language.FR => "Activé",
        Language.ZH => "已启用",
        _ => "Enabled"
    };

    public static string FieldLastStatus => Current switch
    {
        Language.FR => "Dernier état",
        Language.ZH => "上次状态",
        _ => "Last Status"
    };

    public static string FieldCandidateHandle => Current switch
    {
        Language.FR => "Handle repéré",
        Language.ZH => "候选句柄",
        _ => "Candidate Handle"
    };

    public static string FieldCandidatePidTid => Current switch
    {
        Language.FR => "PID/TID repéré",
        Language.ZH => "候选 PID/TID",
        _ => "Candidate PID/TID"
    };

    public static string FieldCandidateProcess => Current switch
    {
        Language.FR => "Processus repéré",
        Language.ZH => "候选进程",
        _ => "Candidate Process"
    };

    public static string FieldCandidateTitle => Current switch
    {
        Language.FR => "Titre repéré",
        Language.ZH => "候选标题",
        _ => "Candidate Title"
    };

    public static string FieldTargetHandle => Current switch
    {
        Language.FR => "Handle cible actuel",
        Language.ZH => "当前目标句柄",
        _ => "Current Target Handle"
    };

    public static string FieldTargetPidTid => Current switch
    {
        Language.FR => "PID/TID cible actuel",
        Language.ZH => "当前目标 PID/TID",
        _ => "Current Target PID/TID"
    };

    public static string FieldTargetProcess => Current switch
    {
        Language.FR => "Processus cible actuel",
        Language.ZH => "当前目标进程",
        _ => "Current Target Process"
    };

    public static string FieldTargetTitle => Current switch
    {
        Language.FR => "Titre cible actuel",
        Language.ZH => "当前目标标题",
        _ => "Current Target Title"
    };

    public static string FieldLogic => Current switch
    {
        Language.FR => "Logique : hooks thread + sous-classe one-shot + canal AllowFocus",
        Language.ZH => "逻辑：线程 hook + 单次子类 + AllowFocus 通道",
        _ => "Logic: thread hooks + one-shot subclass + AllowFocus channel"
    };

    // ── Tray icon ─────────────────────────────────────────────────────────────

    public static string TrayButton => Current switch
    {
        Language.FR => "Réduire dans la barre",
        Language.ZH => "最小化到托盘",
        _ => "Minimize to tray"
    };

    /// <summary>Tooltip affiché au survol de l'icône dans la barre système.</summary>
    public static string TrayTooltip => "Game Focus Guard";

    public static string TrayMenuRestore => Current switch
    {
        Language.FR => "Afficher la fenêtre",
        Language.ZH => "显示窗口",
        _ => "Show window"
    };

    public static string TrayMenuQuit => Current switch
    {
        Language.FR => "Quitter",
        Language.ZH => "退出",
        _ => "Quit"
    };

    public static string TrayBalloonTitle => Current switch
    {
        Language.FR => "Game Focus Guard est actif",
        Language.ZH => "Game Focus Guard 正在运行",
        _ => "Game Focus Guard is still running"
    };

    public static string TrayBalloonText => Current switch
    {
        Language.FR => "L'application continue de fonctionner en arrière-plan.\nDouble-cliquez sur l'icône pour rouvrir la fenêtre.",
        Language.ZH => "程序仍在后台运行。\n双击托盘图标可重新打开窗口。",
        _ => "The application keeps running in the background.\nDouble-click the tray icon to restore the window."
    };

    // ── Formatted messages (with parameters) ─────────────────────────────────

    public static string SavedTo(string path) => Current switch
    {
        Language.FR => $"Sauvegardé dans {path}",
        Language.ZH => $"已保存到 {path}",
        _ => $"Saved to {path}"
    };

    public static string FailedLoadSettings(string msg) => Current switch
    {
        Language.FR => $"Échec du chargement des paramètres, raccourci par défaut utilisé : {msg}",
        Language.ZH => $"加载设置失败，使用默认热键：{msg}",
        _ => $"Failed to load settings, using default hotkey: {msg}"
    };

    public static string RegisteredHotkey(string display) => Current switch
    {
        Language.FR => $"Raccourci {display} enregistré",
        Language.ZH => $"已注册 {display}",
        _ => $"Registered {display}"
    };

    public static string HotkeyAlreadyRegistered(string display) => Current switch
    {
        Language.FR => $"Raccourci déjà enregistré par une autre instance ou un autre programme ({display})",
        Language.ZH => $"热键已被其他实例或程序注册（{display}）",
        _ => $"Hotkey already registered by another instance or program ({display})"
    };

    public static string RegisterHotKeyFailed(int error) => Current switch
    {
        Language.FR => $"Échec de RegisterHotKey. Win32={error}",
        Language.ZH => $"RegisterHotKey 失败，Win32={error}",
        _ => $"RegisterHotKey failed. Win32={error}"
    };

    public static string FailedInstallHooks(int error) => Current switch
    {
        Language.FR => $"Échec de l'installation des hooks. Win32={error}",
        Language.ZH => $"安装 Hook 失败，Win32={error}",
        _ => $"Failed to install hooks. Win32={error}"
    };
}
