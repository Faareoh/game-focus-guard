using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FocusTool.Ui;

internal sealed class AppInstanceCoordinator : IDisposable
{
    private const string MutexName = @"Local\FocusToolPrototype.SingleInstance";

    private readonly Mutex _mutex;

    private AppInstanceCoordinator(Mutex mutex, bool isPrimaryInstance)
    {
        _mutex = mutex;
        IsPrimaryInstance = isPrimaryInstance;
    }

    public bool IsPrimaryInstance { get; }

    public static AppInstanceCoordinator Create()
    {
        var mutex = new Mutex(initiallyOwned: true, name: MutexName, createdNew: out var createdNew);
        return new AppInstanceCoordinator(mutex, createdNew);
    }

    public void ActivateExistingInstance()
    {
        var current = Process.GetCurrentProcess();
        foreach (var process in Process.GetProcessesByName(current.ProcessName))
        {
            if (process.Id == current.Id)
            {
                continue;
            }

            var handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero)
            {
                continue;
            }

            NativeMethods.ShowWindowAsync(handle, NativeMethods.ShowWindowCommands.Restore);
            NativeMethods.SetForegroundWindow(handle);
            return;
        }
    }

    public void Dispose()
    {
        if (IsPrimaryInstance)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // Ignore if ownership has already been released.
            }
        }

        _mutex.Dispose();
    }
}
