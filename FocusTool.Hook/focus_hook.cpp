#include <windows.h>
#include <commctrl.h>

namespace
{
    constexpr wchar_t kPreventDeactivationProp[] = L"__DFPreventDeactivation__";
    constexpr wchar_t kSubClassIdProp[] = L"__DFSubClassID__";
    constexpr wchar_t kSwallowMessageProp[] = L"__DFSwallowMessage__";
    constexpr wchar_t kIgnoreGetMsgProp[] = L"__DFIgnoreGetMsgProcessingInHooks__";
    constexpr wchar_t kAllowFocusMessageName[] = L"WMBFS_DFAllowFocus";

    HWND g_targetWindow = nullptr;
    DWORD g_targetProcessId = 0;
    DWORD g_targetThreadId = 0;
    bool g_enabled = false;
    UINT g_allowFocusMessage = 0;
    volatile LONG64 g_subClassCounter = 100;

    using SwitchToThisWindowFn = VOID (WINAPI*)(HWND, BOOL);

    UINT EnsureAllowFocusMessage()
    {
        if (g_allowFocusMessage == 0)
        {
            g_allowFocusMessage = RegisterWindowMessageW(kAllowFocusMessageName);
        }
        return g_allowFocusMessage;
    }

    bool IsFocusLossMessage(UINT message, WPARAM wParam)
    {
        switch (message)
        {
        case WM_NCACTIVATE:
            return wParam == FALSE;
        case WM_ACTIVATE:
            return LOWORD(wParam) == WA_INACTIVE;
        case WM_ACTIVATEAPP:
            return wParam == FALSE;
        case WM_KILLFOCUS:
            return true;
        default:
            return false;
        }
    }

    bool IsTargetWindow(HWND hwnd)
    {
        return hwnd != nullptr && GetPropW(hwnd, kPreventDeactivationProp) != nullptr;
    }

    void ResetSubclassProps(HWND hwnd)
    {
        RemovePropW(hwnd, kSwallowMessageProp);
        RemovePropW(hwnd, kSubClassIdProp);
    }

    LRESULT CALLBACK SwallowFocusLossSubclassProc(
        HWND hwnd,
        UINT message,
        WPARAM wParam,
        LPARAM lParam,
        UINT_PTR subclassId,
        DWORD_PTR)
    {
        const auto currentId = reinterpret_cast<UINT_PTR>(GetPropW(hwnd, kSubClassIdProp));
        const auto expectedMessage = static_cast<UINT>(reinterpret_cast<UINT_PTR>(GetPropW(hwnd, kSwallowMessageProp)));

        if (currentId == subclassId && expectedMessage == message)
        {
            RemoveWindowSubclass(hwnd, SwallowFocusLossSubclassProc, subclassId);
            ResetSubclassProps(hwnd);
            return 0;
        }

        return DefSubclassProc(hwnd, message, wParam, lParam);
    }

    void RemoveExistingSubclass(HWND hwnd)
    {
        const auto existingId = reinterpret_cast<UINT_PTR>(GetPropW(hwnd, kSubClassIdProp));
        if (existingId != 0)
        {
            RemoveWindowSubclass(hwnd, SwallowFocusLossSubclassProc, existingId);
            ResetSubclassProps(hwnd);
        }
    }

    void InstallOneShotSubclass(HWND hwnd, UINT messageToSwallow)
    {
        RemoveExistingSubclass(hwnd);

        const auto subclassId = static_cast<UINT_PTR>(InterlockedIncrement64(&g_subClassCounter));
        SetPropW(hwnd, kSubClassIdProp, reinterpret_cast<HANDLE>(subclassId));
        SetPropW(hwnd, kSwallowMessageProp, reinterpret_cast<HANDLE>(static_cast<UINT_PTR>(messageToSwallow)));
        SetWindowSubclass(hwnd, SwallowFocusLossSubclassProc, subclassId, 0);
    }

    void HandleAllowFocus(DWORD processId, HWND focusWindow)
    {
        if (processId != 0)
        {
            AllowSetForegroundWindow(processId);
        }

        if (focusWindow != nullptr)
        {
            auto user32 = GetModuleHandleW(L"user32.dll");
            if (user32 != nullptr)
            {
                auto switchToThisWindow = reinterpret_cast<SwitchToThisWindowFn>(
                    GetProcAddress(user32, "SwitchToThisWindow"));

                if (switchToThisWindow != nullptr)
                {
                    switchToThisWindow(focusWindow, TRUE);
                }
            }
        }
    }

    bool IsTargetForegroundWindow()
    {
        if (!g_enabled || g_targetWindow == nullptr)
        {
            return false;
        }

        auto foreground = GetForegroundWindow();
        if (foreground == nullptr)
        {
            return false;
        }

        auto foregroundRoot = GetAncestor(foreground, GA_ROOT);
        auto targetRoot = GetAncestor(g_targetWindow, GA_ROOT);
        return foregroundRoot != nullptr && foregroundRoot == targetRoot;
    }

    bool ShouldSwallowAltTab(WPARAM wParam, const KBDLLHOOKSTRUCT* keyInfo)
    {
        if (keyInfo == nullptr || !IsTargetForegroundWindow())
        {
            return false;
        }

        if (keyInfo->vkCode != VK_TAB)
        {
            return false;
        }

        switch (wParam)
        {
        case WM_KEYDOWN:
        case WM_KEYUP:
        case WM_SYSKEYDOWN:
        case WM_SYSKEYUP:
            return (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
        default:
            return false;
        }
    }
}

extern "C" __declspec(dllexport) BOOL __stdcall SetupHooks()
{
    INITCOMMONCONTROLSEX icc = {};
    icc.dwSize = sizeof(icc);
    icc.dwICC = ICC_STANDARD_CLASSES;
    InitCommonControlsEx(&icc);
    EnsureAllowFocusMessage();
    return TRUE;
}

extern "C" __declspec(dllexport) BOOL __stdcall SetGlobalHookValues(
    HWND targetWindow,
    DWORD processId,
    DWORD threadId,
    BOOL enabled)
{
    g_targetWindow = targetWindow;
    g_targetProcessId = processId;
    g_targetThreadId = threadId;
    g_enabled = enabled != FALSE;
    EnsureAllowFocusMessage();
    return TRUE;
}

extern "C" __declspec(dllexport) BOOL __stdcall ResetWindowState(HWND hwnd)
{
    if (hwnd == nullptr)
    {
        return FALSE;
    }

    RemovePropW(hwnd, kPreventDeactivationProp);
    RemovePropW(hwnd, kIgnoreGetMsgProp);
    RemoveExistingSubclass(hwnd);
    return TRUE;
}

extern "C" __declspec(dllexport) LRESULT CALLBACK CallWndProc(
    int code,
    WPARAM wParam,
    LPARAM lParam)
{
    EnsureAllowFocusMessage();

    if (code < 0 || lParam == 0)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    const auto* messageInfo = reinterpret_cast<const CWPSTRUCT*>(lParam);
    if (messageInfo == nullptr || messageInfo->hwnd == nullptr)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    if (messageInfo->message == g_allowFocusMessage)
    {
        HandleAllowFocus(static_cast<DWORD>(messageInfo->wParam), reinterpret_cast<HWND>(messageInfo->lParam));
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    if (IsTargetWindow(messageInfo->hwnd) && IsFocusLossMessage(messageInfo->message, messageInfo->wParam))
    {
        InstallOneShotSubclass(messageInfo->hwnd, messageInfo->message);
    }

    return CallNextHookEx(nullptr, code, wParam, lParam);
}

extern "C" __declspec(dllexport) LRESULT CALLBACK CallWndRetProc(
    int code,
    WPARAM wParam,
    LPARAM lParam)
{
    return CallNextHookEx(nullptr, code, wParam, lParam);
}

extern "C" __declspec(dllexport) LRESULT CALLBACK GetMsgProc(
    int code,
    WPARAM wParam,
    LPARAM lParam)
{
    if (code < 0 || lParam == 0)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    const auto* messageInfo = reinterpret_cast<const MSG*>(lParam);
    if (messageInfo != nullptr && messageInfo->hwnd != nullptr &&
        GetPropW(messageInfo->hwnd, kIgnoreGetMsgProp) != nullptr)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    return CallNextHookEx(nullptr, code, wParam, lParam);
}

extern "C" __declspec(dllexport) LRESULT CALLBACK LowLevelKeyboardProc(
    int code,
    WPARAM wParam,
    LPARAM lParam)
{
    if (code < 0 || lParam == 0)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    const auto* keyInfo = reinterpret_cast<const KBDLLHOOKSTRUCT*>(lParam);
    if (ShouldSwallowAltTab(wParam, keyInfo))
    {
        return 1;
    }

    return CallNextHookEx(nullptr, code, wParam, lParam);
}
