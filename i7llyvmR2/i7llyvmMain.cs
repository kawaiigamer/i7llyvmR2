using System.Threading;
using XInputium;
using XInputium.XInput;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace i7llyvmR2
{
    internal static class i7llyvmMain
    {
        private static volatile bool _mainLoop = false;
        private static MainWindow _mainForm;
        private static XGamepad _gamepad = new();
        private static GamepadStatistics _statistics;
       // public static GamepadStatistics MyInt { get { return _statistics; } set { _statistics = value; } }
        private static object _locker = new();
        private const string LogFileName = "i7Log.txt";

        private static TimerCallback CreateLock(Action<object?> f, object l, Action<Exception> exeptionCallback, int timeout = 100) => (e) =>
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(l, timeout, ref lockTaken);
                if (lockTaken)
                {
                    f(e);
                }
                else
                {
                    return;
                }
            }
            catch (Exception exp)
            {
                exeptionCallback(exp);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(l);
                }
            }
        };

        private static void GamepadButtonReleasedLock(object? sender, DigitalButtonEventArgs<XInputButton> e)
        {
            CreateLock((t) =>
            {
                GamepadButtonReleased(sender, e);
            }, _locker, (exp) =>
            {
                _mainForm.errorLabel.BeginInvoke((MethodInvoker)delegate {
                    _mainForm.errorLabel.Text = exp.ToString();
                });                
            }).Invoke(null);
        }
            private static void GamepadButtonReleased(object? sender, DigitalButtonEventArgs<XInputButton> e)
        {
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

                case XButtons.DPadUp:
                    _statistics.DU++;
                    break;

                case XButtons.DPadDown:
                    _statistics.DD++;
                    break;

                case XButtons.DPadLeft:
                    _statistics.DL++;
                    break;

                case XButtons.DPadRight:
                    _statistics.DR++;
                    break;

                default:
                    return;
            }
            UpdateButtonsLabel();
        }

        private static void UpdateButtonsLabel()
        {
            _mainForm.buttonsStatisticsLabel.BeginInvoke((MethodInvoker)delegate {
                _mainForm.buttonsStatisticsLabel.Text = _statistics.ToString();
            });
        }

        private static GamepadStatistics LoadStatistics(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<GamepadStatistics>(File.ReadAllText(path));
            }
            catch
            {
                return new();
            }
        }

        private static void SaveStatistics(string path)
        {
            string jsonString = JsonConvert.SerializeObject(_statistics);
            File.WriteAllText(path, jsonString);
        }

        // 2. Multipie devices
        // 3. non buttons
    
        [STAThread]
        static void Main()
        {
            string saved_log_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), LogFileName);
            _statistics = LoadStatistics(saved_log_path);

            new System.Threading.Timer(CreateLock((e) =>
            {
                SaveStatistics(saved_log_path);
            }, _locker, (exp) =>
            {
                _mainForm.errorLabel.BeginInvoke((MethodInvoker)delegate {
                    _mainForm.errorLabel.Text = $"Log saving error: {exp.ToString()}";
                });
            }), null, 0, 1000 * 60 * 10);
            
            _gamepad = new();
            _gamepad.ButtonReleased += GamepadButtonReleasedLock;
            _mainLoop = true;

            //foreach (var userIndex in XInputDevice.GetConnectedDeviceIndexes())
            //{
            //  MessageBox.Show($"Device {gamepad.Buttons.A.IsPressed} is connected.");
            // }


            Thread newThread = new Thread(DoMoreWork);
            newThread.Start("The answer.");

            ApplicationConfiguration.Initialize();
            _mainForm = new MainWindow();
            _mainForm.buttonsStatisticsLabel.Text = _statistics.ToString();
            _mainForm.appManuallyExitEvent += () =>
            {
                CreateLock((t) =>
                {
                    SaveStatistics(saved_log_path);
                    i7llyvmWindowWorker.KeyboardUnhook();                    
                }, _locker, (exp) =>
                {                  
                }).Invoke(null);

                Environment.Exit(Environment.ExitCode);
            };

            _mainForm.clearStatisticsEvent += () =>
            {
                CreateLock((t) =>
                {
                    _statistics = new();
                    SaveStatistics(saved_log_path);
                }, _locker, (exp) =>
                {
                }).Invoke(null);
            };

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