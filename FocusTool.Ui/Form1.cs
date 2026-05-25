namespace FocusTool.Ui;

public partial class Form1 : Form
{
    private const int HotKeyId = 0x5501;

    private readonly HookController _controller = new();
    private readonly HotKeySettingsStore _settingsStore = new();
    private readonly System.Windows.Forms.Timer _statusTimer = new() { Interval = 250 };
    private HotKeyService? _hotKeyService;
    private bool _isWaitingForHotKey;
    private string _startupStatus = "Initializing";

    public Form1()
    {
        InitializeComponent();
        _statusTimer.Tick += StatusTimer_Tick;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
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
                CancelRebindMode("Hotkey rebind cancelled");
                return true;
            }

            if (!HotKeyBinding.TryFromKeyData(keyData, out var newBinding))
            {
                _startupStatus = "Waiting for a non-modifier key combination";
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
                _startupStatus = $"{status} | Saved to {path}";
                _isWaitingForHotKey = false;
                rebindHotKeyButton.Text = "改热键";
            }
            else
            {
                _startupStatus = status;
                _isWaitingForHotKey = false;
                rebindHotKeyButton.Text = "改热键";
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
        var hotKeyText = _hotKeyService?.CurrentBinding.ToDisplayString() ?? "(unavailable)";
        candidateLabel.Text = $"候选窗口: {snapshot.CandidateWindow.ProcessName} | {snapshot.CandidateWindow.WindowTitle}";
        hotKeyLabel.Text = _isWaitingForHotKey
            ? "当前热键: 等待新的组合键... (Esc 取消)"
            : $"当前热键: {hotKeyText}";

        statusTextBox.Text =
            $"Hotkey: {hotKeyText}{Environment.NewLine}" +
            $"Hotkey Registered: {_hotKeyService?.IsRegistered}{Environment.NewLine}" +
            $"Hotkey Status: {_hotKeyService?.LastStatus}{Environment.NewLine}" +
            $"UI Status: {_startupStatus}{Environment.NewLine}" +
            $"Enabled: {snapshot.IsEnabled}{Environment.NewLine}" +
            $"Last Status: {snapshot.LastError}{Environment.NewLine}{Environment.NewLine}" +
            $"Candidate Handle: 0x{snapshot.CandidateWindow.Handle.ToInt64():X}{Environment.NewLine}" +
            $"Candidate PID/TID: {snapshot.CandidateWindow.ProcessId}/{snapshot.CandidateWindow.ThreadId}{Environment.NewLine}" +
            $"Candidate Process: {snapshot.CandidateWindow.ProcessName}{Environment.NewLine}" +
            $"Candidate Title: {snapshot.CandidateWindow.WindowTitle}{Environment.NewLine}{Environment.NewLine}" +
            $"Current Target Handle: 0x{snapshot.CurrentTarget.Handle.ToInt64():X}{Environment.NewLine}" +
            $"Current Target PID/TID: {snapshot.CurrentTarget.ProcessId}/{snapshot.CurrentTarget.ThreadId}{Environment.NewLine}" +
            $"Current Target Process: {snapshot.CurrentTarget.ProcessName}{Environment.NewLine}" +
            $"Current Target Title: {snapshot.CurrentTarget.WindowTitle}{Environment.NewLine}{Environment.NewLine}" +
            $"CallWndProc Hook: 0x{snapshot.Hooks.CallWndProc.ToInt64():X}{Environment.NewLine}" +
            $"CallWndRet Hook: 0x{snapshot.Hooks.CallWndRetProc.ToInt64():X}{Environment.NewLine}" +
            $"GetMsg Hook: 0x{snapshot.Hooks.GetMsg.ToInt64():X}{Environment.NewLine}" +
            $"KeyboardLL Hook: 0x{snapshot.Hooks.KeyboardLl.ToInt64():X}{Environment.NewLine}{Environment.NewLine}" +
            "Logic: thread hooks + one-shot subclass + AllowFocus channel";
    }

    private void rebindHotKeyButton_Click(object? sender, EventArgs e)
    {
        if (_hotKeyService == null)
        {
            return;
        }

        if (_isWaitingForHotKey)
        {
            CancelRebindMode("Hotkey rebind cancelled");
            return;
        }

        _isWaitingForHotKey = true;
        _hotKeyService.Unregister();
        rebindHotKeyButton.Text = "取消重绑";
        _startupStatus = "Press a new hotkey combination. Esc cancels.";
        ActiveControl = null;
        RefreshStatus();
    }

    private void CancelRebindMode(string status)
    {
        _isWaitingForHotKey = false;
        rebindHotKeyButton.Text = "改热键";
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
        _statusTimer.Stop();
        _hotKeyService?.Unregister();
        _controller.Dispose();
    }
}
