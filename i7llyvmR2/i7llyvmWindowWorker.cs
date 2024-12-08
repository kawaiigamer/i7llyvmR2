using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace i7llyvmR2
{
    internal static class i7llyvmWindowWorker
    {
        private static bool appsPressed = false;
        private static MainWindow mainWindow;

        private const int apps_key = 0x5D;
        private const int slash_key = 0xBF;
        private const int quotes_key = 0xDE;
        private const int backslash_key = 0xDC;

        private static bool IsMainFormActive() => Form.ActiveForm != mainWindow;

        private static bool PublicKeyHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            switch (vkCode)
            {
                case apps_key:
                    appsPressed = true;
                    break;

                case slash_key:
                    if (appsPressed)
                    {
                        if (IsMainFormActive())
                        {
                            if (mainWindow.Visible)
                            {
                                mainWindow.Activate();
                            }
                            else
                            {
                                mainWindow.Show();
                            }
                        }
                        else
                        {
                            mainWindow.Hide();
                        }
                    }

                    break;

                case quotes_key:
                    if (appsPressed && mainWindow.Visible)
                    {
                        mainWindow.WindowState = mainWindow.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    }
                    break;

                case backslash_key:
                    if (appsPressed && mainWindow.Visible)
                    {
                        mainWindow.ManuallyExit();
                    }
                    break;

                default:
                    appsPressed = false;
                    return false;
            }
            return true;
        }

        internal static void KeyboardHook(MainWindow window)
        {
            mainWindow = window;
            InterceptKeys.SetWindowsHook(PublicKeyHookCallback);
        }
        internal static void KeyboardUnhook()
        {
            mainWindow = null;
            InterceptKeys.UnhookWindowsHook();
        }     
    }
}
