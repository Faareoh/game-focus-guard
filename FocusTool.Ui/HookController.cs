using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FocusTool.Ui;

internal sealed class HookController : IDisposable
{
    private const int WhCallWndProc = 4;
    private const int WhCallWndRetProc = 12;
    private const int WhGetMessage = 3;
    private const int WhKeyboardLl = 13;
    private const string PreventDeactivationProp = "__DFPreventDeactivation__";
    private const string AllowFocusMessageName = "WMBFS_DFAllowFocus";
    private const string HookDllName = "FocusTool.Hook.dll";

    private IntPtr _hookModule;
    private IntPtr _callWndProcAddress;
    private IntPtr _callWndRetProcAddress;
    private IntPtr _getMsgProcAddress;
    private IntPtr _keyboardProcAddress;
    private ResetWindowStateDelegate? _resetWindowState;
    private SetGlobalHookValuesDelegate? _setGlobalHookValues;
    private SetupHooksDelegate? _setupHooks;

    private IntPtr _callWndHook;
    private IntPtr _callWndRetHook;
    private IntPtr _getMsgHook;
    private IntPtr _keyboardHook;

    private IntPtr _currentTarget;
    private uint _currentProcessId;
    private uint _currentThreadId;
    private string _currentProcessName = Strings.None;
    private string _currentWindowTitle = Strings.None;

    private IntPtr _candidateWindow;
    private uint _candidateProcessId;
    private uint _candidateThreadId;
    private string _candidateProcessName = Strings.None;
    private string _candidateWindowTitle = Strings.None;

    private readonly uint _allowFocusMessage;
    private bool _disposed;
    private string _lastError = Strings.Idle;

    public HookController()
    {
        _allowFocusMessage = (uint)NativeMethods.RegisterWindowMessage(AllowFocusMessageName);
    }

    public bool IsEnabled => _currentTarget != IntPtr.Zero;

    public HookStatusSnapshot GetSnapshot()
    {
        return new HookStatusSnapshot(
            IsEnabled,
            _lastError,
            new WindowSnapshot(_candidateWindow, _candidateProcessId, _candidateThreadId, _candidateProcessName, _candidateWindowTitle),
            new WindowSnapshot(_currentTarget, _currentProcessId, _currentThreadId, _currentProcessName, _currentWindowTitle),
            new HookHandlesSnapshot(_callWndHook, _callWndRetHook, _getMsgHook, _keyboardHook));
    }

    public void CaptureCandidateWindow(IntPtr ownerWindowHandle)
    {
        var foreground = NativeMethods.GetForegroundWindow();
        if (foreground == IntPtr.Zero)
        {
            return;
        }

        var root = NativeMethods.GetAncestor(foreground, NativeMethods.GetAncestorFlags.GA_ROOT);
        if (root == IntPtr.Zero || root == ownerWindowHandle)
        {
            return;
        }

        var threadId = NativeMethods.GetWindowThreadProcessId(root, out var processId);
        if (processId == 0 || processId == Environment.ProcessId)
        {
            return;
        }

        _candidateWindow = root;
        _candidateThreadId = threadId;
        _candidateProcessId = processId;
        _candidateProcessName = TryGetProcessName(processId);
        _candidateWindowTitle = GetWindowTitle(root);
    }

    public void ToggleForBestAvailableTarget(IntPtr ownerWindowHandle)
    {
        ThrowIfDisposed();

        var foreground = NativeMethods.GetForegroundWindow();
        var root = foreground == IntPtr.Zero
            ? IntPtr.Zero
            : NativeMethods.GetAncestor(foreground, NativeMethods.GetAncestorFlags.GA_ROOT);

        if (root == ownerWindowHandle)
        {
            root = _candidateWindow;
        }

        if (root == IntPtr.Zero)
        {
            _lastError = Strings.NoUsableTarget;
            return;
        }

        if (IsEnabled && root == _currentTarget)
        {
            Disable();
            _lastError = Strings.DisabledCurrentTarget;
            return;
        }

        EnableForTarget(root);
    }

    public void Disable()
    {
        ThrowIfDisposed();

        if (_currentTarget != IntPtr.Zero)
        {
            NativeMethods.RemoveProp(_currentTarget, PreventDeactivationProp);
            _resetWindowState?.Invoke(_currentTarget);
        }

        UnhookAll();
        ClearCurrentTarget();
        _lastError = Strings.Disabled;
    }

    private void EnableForTarget(IntPtr targetWindow)
    {
        Disable();
        EnsureHookDllLoaded();

        var root = NativeMethods.GetAncestor(targetWindow, NativeMethods.GetAncestorFlags.GA_ROOT);
        if (root == IntPtr.Zero)
        {
            _lastError = Strings.FailedRootWindow;
            return;
        }

        _currentTarget = root;
        _currentThreadId = NativeMethods.GetWindowThreadProcessId(root, out _currentProcessId);
        if (_currentProcessId == 0 || _currentThreadId == 0)
        {
            ClearCurrentTarget();
            _lastError = Strings.FailedProcessOrThread;
            return;
        }

        _currentProcessName = TryGetProcessName(_currentProcessId);
        _currentWindowTitle = GetWindowTitle(root);

        _setupHooks?.Invoke();
        _setGlobalHookValues?.Invoke(root, _currentProcessId, _currentThreadId, true);
        NativeMethods.SetProp(root, PreventDeactivationProp, (IntPtr)1);

        _callWndHook = NativeMethods.SetWindowsHookEx(WhCallWndProc, _callWndProcAddress, _hookModule, _currentThreadId);
        _callWndRetHook = NativeMethods.SetWindowsHookEx(WhCallWndRetProc, _callWndRetProcAddress, _hookModule, _currentThreadId);
        _getMsgHook = NativeMethods.SetWindowsHookEx(WhGetMessage, _getMsgProcAddress, _hookModule, _currentThreadId);
        _keyboardHook = NativeMethods.SetWindowsHookEx(WhKeyboardLl, _keyboardProcAddress, _hookModule, 0);

        if (_callWndHook == IntPtr.Zero || _callWndRetHook == IntPtr.Zero || _getMsgHook == IntPtr.Zero || _keyboardHook == IntPtr.Zero)
        {
            _lastError = Strings.FailedInstallHooks(Marshal.GetLastWin32Error());
            Disable();
            return;
        }

        NativeMethods.SendMessageTimeout(
            root,
            _allowFocusMessage,
            (UIntPtr)_currentProcessId,
            root,
            NativeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
            150,
            out _);

        _lastError = Strings.Enabled;
    }

    private void EnsureHookDllLoaded()
    {
        if (_hookModule != IntPtr.Zero)
        {
            return;
        }

        var dllPath = Path.Combine(AppContext.BaseDirectory, HookDllName);
        if (!File.Exists(dllPath))
        {
            throw new InvalidOperationException(
                $"Native hook DLL not found: {dllPath}. The current build or release package is incomplete.");
        }

        _hookModule = NativeMethods.LoadLibrary(dllPath);
        if (_hookModule == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Failed to load native hook DLL: {dllPath}. Win32={Marshal.GetLastWin32Error()}");
        }

        _callWndProcAddress = GetExport("CallWndProc");
        _callWndRetProcAddress = GetExport("CallWndRetProc");
        _getMsgProcAddress = GetExport("GetMsgProc");
        _keyboardProcAddress = GetExport("LowLevelKeyboardProc");

        _setupHooks = Marshal.GetDelegateForFunctionPointer<SetupHooksDelegate>(GetExport("SetupHooks"));
        _setGlobalHookValues = Marshal.GetDelegateForFunctionPointer<SetGlobalHookValuesDelegate>(GetExport("SetGlobalHookValues"));
        _resetWindowState = Marshal.GetDelegateForFunctionPointer<ResetWindowStateDelegate>(GetExport("ResetWindowState"));
    }

    private IntPtr GetExport(string name)
    {
        var address = NativeMethods.GetProcAddress(_hookModule, name);
        if (address == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Failed to resolve export: {name}");
        }

        return address;
    }

    private void UnhookAll()
    {
        Unhook(ref _callWndHook);
        Unhook(ref _callWndRetHook);
        Unhook(ref _getMsgHook);
        Unhook(ref _keyboardHook);

        _setGlobalHookValues?.Invoke(IntPtr.Zero, 0, 0, false);
    }

    private static void Unhook(ref IntPtr hookHandle)
    {
        if (hookHandle == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.UnhookWindowsHookEx(hookHandle);
        hookHandle = IntPtr.Zero;
    }

    private void ClearCurrentTarget()
    {
        _currentTarget = IntPtr.Zero;
        _currentProcessId = 0;
        _currentThreadId = 0;
        _currentProcessName = Strings.None;
        _currentWindowTitle = Strings.None;
    }

    private static string GetWindowTitle(IntPtr hwnd)
    {
        var builder = new StringBuilder(512);
        NativeMethods.GetWindowText(hwnd, builder, builder.Capacity);
        return string.IsNullOrWhiteSpace(builder.ToString()) ? Strings.Untitled : builder.ToString();
    }

    private static string TryGetProcessName(uint processId)
    {
        try
        {
            return Process.GetProcessById((int)processId).ProcessName;
        }
        catch
        {
            return Strings.Unknown;
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Disable();

        if (_hookModule != IntPtr.Zero)
        {
            NativeMethods.FreeLibrary(_hookModule);
            _hookModule = IntPtr.Zero;
        }

        _disposed = true;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool SetupHooksDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool SetGlobalHookValuesDelegate(IntPtr targetWindow, uint processId, uint threadId, bool enabled);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool ResetWindowStateDelegate(IntPtr hwnd);
}
