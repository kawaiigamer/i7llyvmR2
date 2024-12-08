using System.Threading;
using System.Runtime.InteropServices;
using XInputium;
using XInputium.XInput;

namespace i7llyvmR2
{
    internal static class i7llyvmMain
    {
        private static volatile bool _mainLoop = false;
        private static MainWindow _mainForm;
        private static XGamepad _gamepad = new();
        private static GamepadStatistics _statistics = new();
        
        private static void GamepadButtonReleased(object? sender, DigitalButtonEventArgs<XInputButton> e)
        {
            //MessageBox.Show($"Hello, universe!. You release button '{e.Button}'.");
            switch (e.Button.Button)
            {
                case XButtons.A:
                    _statistics.A++;
                    break;

                case XButtons.B:
                    _statistics.B++;
                    break;

                case XButtons.X:
                    _statistics.X++;
                    break;

                case XButtons.Y:
                    _statistics.Y++;
                    break;

                case XButtons.LB:
                    _statistics.LB++;
                    break;

                case XButtons.RB:
                    _statistics.RB++;
                    break;

                case XButtons.LS:
                    _statistics.LS++;
                    break;

                case XButtons.RS:
                    _statistics.RS++;
                    break;

                case XButtons.Start:
                    _statistics.Start++;
                    break;

                case XButtons.Back:
                    _statistics.Back++;
                    break;



                default:
                    return;
            }
            _mainForm.buttonsStatisticsLabel.BeginInvoke((MethodInvoker)delegate {
                _mainForm.buttonsStatisticsLabel.Text = _statistics.ToString(); }); 

        }

        // 1. Load from file/ SaveToFile
        // 2. Multipie devices
        // 3. non buttons

        [STAThread]
        static void Main()
        {
            _gamepad = new();
            _gamepad.ButtonReleased += GamepadButtonReleased;
            _mainLoop = true;

            //foreach (var userIndex in XInputDevice.GetConnectedDeviceIndexes())
            //{
            //  MessageBox.Show($"Device {gamepad.Buttons.A.IsPressed} is connected.");
            // }


            Thread newThread = new Thread(DoMoreWork);
            newThread.Start("The answer.");

            ApplicationConfiguration.Initialize();
            _mainForm = new MainWindow();
            i7llyvmWindowWorker.KeyboardHook(_mainForm);
            Application.Run(_mainForm);
            i7llyvmWindowWorker.KeyboardUnhook();            
        }

        private static void DoMoreWork(object? obj)
        {
            while (_mainLoop)
            {
                _gamepad.Update();
                Thread.Sleep(10);
            }           
        }
    }
}