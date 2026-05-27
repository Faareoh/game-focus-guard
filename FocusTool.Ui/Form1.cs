namespace FocusTool.Ui;

public partial class Form1 : Form
{
    private const int HotKeyId = 0x5501;

    private readonly HookController _controller = new();
    private readonly HotKeySettingsStore _settingsStore = new();
    private readonly System.Windows.Forms.Timer _statusTimer = new() { Interval = 250 };
    private HotKeyService? _hotKeyService;
    private bool _isWaitingForHotKey;
    private string _startupStatus = string.Empty;

    /// <summary>Mis à true par le menu "Quitter" pour autoriser la fermeture réelle.</summary>
    private bool _allowClose;

    /// <summary>Évite d'afficher le balloon tip plusieurs fois.</summary>
    private bool _trayBalloonShown;

    /// <summary>Icône de l'application chargée depuis icon.ico.</summary>
    private Icon? _appIcon;

    public Form1()
    {
        InitializeComponent();
        Strings.LanguageChanged += ApplyLanguage;
        _statusTimer.Tick += StatusTimer_Tick;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        // Icône de la fenêtre et de la barre système
        var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
        if (File.Exists(iconPath))
        {
            _appIcon = new Icon(iconPath);
            Icon = _appIcon;
            notifyIcon.Icon = _appIcon;
        }
        else
        {
            notifyIcon.Icon = SystemIcons.Shield; // fallback si le fichier est absent
        }

        // Restore saved language
        var savedLang = LanguageStore.Load();
        languageComboBox.SelectedIndex = savedLang switch
        {
            Language.FR => 1,
            Language.ZH => 2,
            _ => 0
        };
        Strings.SetLanguage(savedLang);

        _startupStatus = Strings.Initializing;

        var settings = _settingsStore.LoadOrDefault();
        _startupStatus = settings.StatusMessage;

        _hotKeyService = new HotKeyService(Handle, HotKeyId, settings.Binding);
        if (!_hotKeyService.TryRegisterCurrent(out var registerStatus))
        {
            _startupStatus = registerStatus;
        }

        _statusTimer.Start();
        RefreshStatus();
    }

    // ── Tray icon ─────────────────────────────────────────────────────────────

    private void trayButton_Click(object? sender, EventArgs e) => HideToTray();

    private void notifyIcon_DoubleClick(object? sender, EventArgs e) => RestoreFromTray();

    private void trayMenuRestore_Click(object? sender, EventArgs e) => RestoreFromTray();

    private void trayMenuQuit_Click(object? sender, EventArgs e)
    {
        _allowClose = true;
        Close();
    }

    private void HideToTray()
    {
        Hide();
        notifyIcon.Visible = true;

        if (!_trayBalloonShown)
        {
            _trayBalloonShown = true;
            notifyIcon.ShowBalloonTip(
                4000,
                Strings.TrayBalloonTitle,
                Strings.TrayBalloonText,
                ToolTipIcon.Info);
        }
    }

    private void RestoreFromTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
        notifyIcon.Visible = false;
    }

    // ── Language ──────────────────────────────────────────────────────────────

    private void languageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var lang = languageComboBox.SelectedIndex switch
        {
            1 => Language.FR,
            2 => Language.ZH,
            _ => Language.EN
        };
        Strings.SetLanguage(lang);
        LanguageStore.Save(lang);
    }

    private void ApplyLanguage()
    {
        Text = Strings.AppTitle;
        toggleButton.Text = Strings.ToggleButton;
        rebindHotKeyButton.Text = _isWaitingForHotKey
            ? Strings.CancelRebindButton
            : Strings.RebindHotKeyButton;
        languageLabel.Text = Strings.LanguageLabel;
        trayButton.Text = Strings.TrayButton;
        trayMenuRestore.Text = Strings.TrayMenuRestore;
        trayMenuQuit.Text = Strings.TrayMenuQuit;
        notifyIcon.Text = Strings.TrayTooltip;
        RefreshStatus();
    }

    // ── Core handlers ─────────────────────────────────────────────────────────

    private void toggleButton_Click(object? sender, EventArgs e)
    {
        _controller.ToggleForBestAvailableTarget(Handle);
        RefreshStatus();
    }

    private void StatusTimer_Tick(object? sender, EventArgs e)
    {
        _controller.CaptureCandidateWindow(Handle);
        RefreshStatus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (_isWaitingForHotKey)
        {
            if (keyData == Keys.Escape)
            {
                CancelRebindMode(Strings.HotkeyRebindCancelled);
                return true;
            }

            if (!HotKeyBinding.TryFromKeyData(keyData, out var newBinding))
            {
                _startupStatus = Strings.WaitingForNonModifier;
                RefreshStatus();
                return true;
            }

            if (_hotKeyService == null)
            {
                return true;
            }

            if (_hotKeyService.TryUpdateBinding(newBinding, out var status))
            {
                var path = _settingsStore.Save(newBinding);
                _startupStatus = $"{status} | {Strings.SavedTo(path)}";
                _isWaitingForHotKey = false;
                rebindHotKeyButton.Text = Strings.RebindHotKeyButton;
            }
            else
            {
                _startupStatus = status;
                _isWaitingForHotKey = false;
                rebindHotKeyButton.Text = Strings.RebindHotKeyButton;
            }

            RefreshStatus();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WmHotKey && m.WParam == HotKeyId)
        {
            _controller.ToggleForBestAvailableTarget(Handle);
            RefreshStatus();
        }

        base.WndProc(ref m);
    }

    private void RefreshStatus()
    {
        var snapshot = _controller.GetSnapshot();
        var hotKeyText = _hotKeyService?.CurrentBinding.ToDisplayString() ?? Strings.Unavailable;

        candidateLabel.Text =
            $"{Strings.CandidateWindowPrefix}: {snapshot.CandidateWindow.ProcessName} | {snapshot.CandidateWindow.WindowTitle}";

        hotKeyLabel.Text = _isWaitingForHotKey
            ? Strings.HotKeyWaiting
            : $"{Strings.CurrentHotKeyPrefix}: {hotKeyText}";

        statusTextBox.Text =
            $"{Strings.FieldHotkey}: {hotKeyText}{Environment.NewLine}" +
            $"{Strings.FieldHotkeyRegistered}: {_hotKeyService?.IsRegistered}{Environment.NewLine}" +
            $"{Strings.FieldHotkeyStatus}: {_hotKeyService?.LastStatus}{Environment.NewLine}" +
            $"{Strings.FieldUiStatus}: {_startupStatus}{Environment.NewLine}" +
            $"{Strings.FieldEnabled}: {snapshot.IsEnabled}{Environment.NewLine}" +
            $"{Strings.FieldLastStatus}: {snapshot.LastError}{Environment.NewLine}{Environment.NewLine}" +
            $"{Strings.FieldCandidateHandle}: 0x{snapshot.CandidateWindow.Handle.ToInt64():X}{Environment.NewLine}" +
            $"{Strings.FieldCandidatePidTid}: {snapshot.CandidateWindow.ProcessId}/{snapshot.CandidateWindow.ThreadId}{Environment.NewLine}" +
            $"{Strings.FieldCandidateProcess}: {snapshot.CandidateWindow.ProcessName}{Environment.NewLine}" +
            $"{Strings.FieldCandidateTitle}: {snapshot.CandidateWindow.WindowTitle}{Environment.NewLine}{Environment.NewLine}" +
            $"{Strings.FieldTargetHandle}: 0x{snapshot.CurrentTarget.Handle.ToInt64():X}{Environment.NewLine}" +
            $"{Strings.FieldTargetPidTid}: {snapshot.CurrentTarget.ProcessId}/{snapshot.CurrentTarget.ThreadId}{Environment.NewLine}" +
            $"{Strings.FieldTargetProcess}: {snapshot.CurrentTarget.ProcessName}{Environment.NewLine}" +
            $"{Strings.FieldTargetTitle}: {snapshot.CurrentTarget.WindowTitle}{Environment.NewLine}{Environment.NewLine}" +
            $"CallWndProc Hook: 0x{snapshot.Hooks.CallWndProc.ToInt64():X}{Environment.NewLine}" +
            $"CallWndRet Hook: 0x{snapshot.Hooks.CallWndRetProc.ToInt64():X}{Environment.NewLine}" +
            $"GetMsg Hook: 0x{snapshot.Hooks.GetMsg.ToInt64():X}{Environment.NewLine}" +
            $"KeyboardLL Hook: 0x{snapshot.Hooks.KeyboardLl.ToInt64():X}{Environment.NewLine}{Environment.NewLine}" +
            Strings.FieldLogic;
    }

    private void rebindHotKeyButton_Click(object? sender, EventArgs e)
    {
        if (_hotKeyService == null)
        {
            return;
        }

        if (_isWaitingForHotKey)
        {
            CancelRebindMode(Strings.HotkeyRebindCancelled);
            return;
        }

        _isWaitingForHotKey = true;
        _hotKeyService.Unregister();
        rebindHotKeyButton.Text = Strings.CancelRebindButton;
        _startupStatus = Strings.PressNewHotkey;
        ActiveControl = null;
        RefreshStatus();
    }

    private void CancelRebindMode(string status)
    {
        _isWaitingForHotKey = false;
        rebindHotKeyButton.Text = Strings.RebindHotKeyButton;
        if (_hotKeyService != null)
        {
            _hotKeyService.TryRegisterCurrent(out var registerStatus);
            _startupStatus = $"{status} | {registerStatus}";
        }
        else
        {
            _startupStatus = status;
        }

        RefreshStatus();
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Clic sur la croix → minimiser dans la barre au lieu de fermer
        if (e.CloseReason == CloseReason.UserClosing && !_allowClose)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }

        // Fermeture réelle (menu "Quitter" ou fin de session Windows)
        _statusTimer.Stop();
        _hotKeyService?.Unregister();
        _controller.Dispose();
        Strings.LanguageChanged -= ApplyLanguage;
        notifyIcon.Visible = false;
        _appIcon?.Dispose();
    }
}
