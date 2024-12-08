using System.Threading;
using XInputium;
using XInputium.XInput;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Windows.Gaming.Input;

namespace i7llyvmR2
{
    internal static class i7llyvmMain
    {
        private static volatile bool _mainLoopFlag = false;
        private static MainWindow _mainForm;
        private static XGamepad _gamepad;
        private static GamepadStatistics _statistics;
        public static Thread _mainLoopThread; 
        private static object _locker = new();
        private const string LogFileName = "i7Log.txt";
        private static float _lastPositionLT = 0;
        private static float _lastPositionRT = 0;
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
                _mainForm.buttonsStatisticsLabel.Text = _statistics.ToStringButtons();
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
                
        //private static EventHandler CreateTriggerCallback(Trigger t, ref ulong s, ref float lv, Label l)
        //                                                                        => (sender, eventArgs) =>
        //{
        //    t.IsMovingChanged += (s, e) =>
        //    {
        //        if (lv != 1 && t.Value == 1)
        //        {
        //            CreateLock((e) =>
        //            {
        //                s++;
        //                l.BeginInvoke((MethodInvoker)delegate
        //                {
        //                    l.Text = _statistics.ToStringTriggers();
        //                });
        //            }, _locker, exp =>
        //            { }).Invoke(null);


        //            lv = _gamepad.RightTrigger.Value;



        //        }
        //        lv = _gamepad.RightTrigger.Value;
        //    };
        //}
            
        //);    

        // 2. Multipie devices
        // loak
        // Clear button!

        [STAThread]
        static void Main()
        {
            string saved_log_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), LogFileName);
            _statistics = LoadStatistics(saved_log_path);

            System.Threading.Timer saveTimer = new System.Threading.Timer(CreateLock((e) =>
            {
                SaveStatistics(saved_log_path);
            }, _locker, (exp) =>
            {
                _mainForm.errorLabel.BeginInvoke((MethodInvoker)delegate {
                    _mainForm.errorLabel.Text = $"Log saving error: {exp}";
                });
            }), null, 0, 1000 * 60 * 10);
            
            _gamepad = new();
            _gamepad.ButtonReleased += GamepadButtonReleasedLock;
            
            
            _gamepad.LeftTrigger.IsMovingChanged += (s, e) => {
                
                    if (_lastPositionLT != 1 && _gamepad.LeftTrigger.Value == 1)
                    {
                        _mainForm.triggerStatisticsLabel.BeginInvoke((MethodInvoker)delegate
                        {
                            _statistics.LT++; // lock
                            _mainForm.triggerStatisticsLabel.Text = _statistics.ToStringTriggers();
                        });
                    }
                _lastPositionLT = _gamepad.LeftTrigger.Value;
            };

            _gamepad.RightTrigger.IsMovingChanged += (s, e) => {

                if (_lastPositionRT != 1 && _gamepad.RightTrigger.Value == 1)
                {
                    _mainForm.triggerStatisticsLabel.BeginInvoke((MethodInvoker)delegate
                    {
                        _statistics.RT++; // lock
                        _mainForm.triggerStatisticsLabel.Text = _statistics.ToStringTriggers();
                    });
                }
                _lastPositionRT = _gamepad.RightTrigger.Value;
            };

            ApplicationConfiguration.Initialize();
            _mainForm = new MainWindow();

            _mainForm.buttonsStatisticsLabel.Text = _statistics.ToStringButtons();
            _mainForm.triggerStatisticsLabel.Text = _statistics.ToStringTriggers();

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

            _mainLoopThread = new Thread(MainLoopThread);
            _mainLoopThread.Start();
            _mainLoopFlag = true;

            Application.Run(_mainForm);            
            i7llyvmWindowWorker.KeyboardUnhook(); 
        }
        
        private static void MainLoopThread(object? obj)
        {
            while (_mainLoopFlag)
            {
                _gamepad.Update();
                Thread.Sleep(10);
            }           
        }
    }
}