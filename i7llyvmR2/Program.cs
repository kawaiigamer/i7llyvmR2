using System.Runtime.InteropServices;

namespace i7llyvmR2
{
    internal static class Program
    {
        private static bool appsPressed = false;
        private static void HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            const int apps_key = 0x5D;
            const int q_key = 0x51;

            int vkCode = Marshal.ReadInt32(lParam);
            switch (vkCode)
            {
                case apps_key:
                    appsPressed = true;
                    break;

                case q_key:
                    if (appsPressed)
                    {

                    }                    
                    break;

                default:
                    appsPressed = false;
                    break;
            }
        }


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            InterceptKeys.SetWindowsHook(HookCallback);
            Application.Run(new MainWindow());
            InterceptKeys.UnhookWindowsHook();
        }
    }
}