using System.Diagnostics;
using System.Runtime.InteropServices;

namespace i7llyvmR2
{
    internal static class InterceptKeys
    {
        public delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr hook_id = IntPtr.Zero;
        private static LowLevelKeyboardProcDelegate? _userCallback;
        private static LowLevelKeyboardProcDelegate? _internalCallback;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static bool IsHooked => hook_id != IntPtr.Zero;

        public static void UnregisterKeyboardHook()
        {
            if (IsHooked)
            {
                try
                {
                    UnhookWindowsHookEx(hook_id);
                }
                finally
                {
                    hook_id = IntPtr.Zero;
                    _internalCallback = null;
                    _userCallback = null;
                }
            }
        }

        public static void RegisterKeyboardHook(LowLevelKeyboardProcDelegate callback)
        {
            const int WH_KEYBOARD_LL = 13;
            UnregisterKeyboardHook();            
            _userCallback = callback;
            _internalCallback = (nCode, wParam, lParam) =>
            {
                const int WM_KEYDOWN = 0x0100;
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    if (_userCallback != null && _userCallback(nCode, wParam, lParam) != IntPtr.Zero)
                    {
                        return (IntPtr)1;
                    }
                }
                return CallNextHookEx(hook_id, nCode, wParam, lParam);
            };
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule? curModule = curProcess.MainModule;
            IntPtr moduleHandle = curModule != null ? GetModuleHandle(curModule.ModuleName) : IntPtr.Zero;
            hook_id = SetWindowsHookEx(WH_KEYBOARD_LL, _internalCallback, moduleHandle, 0);
        }
    }
}
