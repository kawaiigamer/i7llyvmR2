using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace i7llyvmR2
{ 
    internal class InterceptKeys
    {
        public delegate void LowLevelKeyboardHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardHookDelegate _proc;// = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;     

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

        public static void SetWindowsHook(LowLevelKeyboardHookDelegate callback)
        {
            _proc = callback;
            _hookID = SetLowLevelHook(PrivateHookCallback);
        }

        public static void UnhookWindowsHook()
        {
            if(_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
            }            
        }

        private static IntPtr SetLowLevelHook(LowLevelKeyboardProcDelegate proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr PrivateHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {                
                _proc(nCode, wParam, lParam);               
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
