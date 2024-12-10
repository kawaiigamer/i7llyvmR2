using XInputium;
using XInputium.XInput;
using Newtonsoft.Json;
using System.Threading;

namespace i7llyvmR2
{
    internal static class i7llyvmMain
    {
        private static readonly object _locker = new();
        private static volatile bool _mainLoopFlag = false;
        private static MainWindow _mainForm;
        private static XGamepad _gamepad;
        private static GamepadStatistics _statistics;
        private static Thread _mainLoopThread;
        private static float _lastPositionLT = 0;
        private static float _lastPositionRT = 0;
        private const string LogFileName = "i7Log.txt";
        private const int UpdateTimeMsec = 10;
        private static TimerCallback CreateLock(Action<object?> f, object l, Action<Exception> exceptionCallback, int timeout = 100) => (e) =>
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
                exceptionCallback(exp);
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
                UpdateErrorLabel(exp.ToString());              
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

        private static void UpdateButtonsLabel(bool uiThread = false)
        {
            _mainForm.SetButtonsLabel(_statistics.ToStringButtons(), uiThread);
        }

        private static void UpdateTriggersLabel(bool uiThread = false)
        {
            _mainForm.SetTriggersLabel(_statistics.ToStringTriggers(), uiThread);
        }

        private static void UpdateErrorLabel(string txt, bool uiThread = false)
        {
            _mainForm.SetErrorLabel(txt, uiThread);
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

        // TODO:
        // 1. Multipie devices
        // 2. Triggers lock


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
                UpdateErrorLabel($"Log saving error: {exp}");
            }), null, 0, 1000 * 60 * 10);
            
            _gamepad = new();
            _gamepad.ButtonReleased += GamepadButtonReleasedLock;
            
            
            _gamepad.LeftTrigger.IsMovingChanged += (s, e) => {
                
                    if (_lastPositionLT != 1 && _gamepad.LeftTrigger.Value == 1)
                    {
                        _statistics.LT++; // lock
                        UpdateTriggersLabel();

                   }
                _lastPositionLT = _gamepad.LeftTrigger.Value;
            };

            _gamepad.RightTrigger.IsMovingChanged += (s, e) => {

                if (_lastPositionRT != 1 && _gamepad.RightTrigger.Value == 1)
                {
                    _statistics.RT++; // lock
                    UpdateTriggersLabel();
                }
                _lastPositionRT = _gamepad.RightTrigger.Value;
            };

            ApplicationConfiguration.Initialize();
            _mainForm = new MainWindow();
            UpdateButtonsLabel(true);
            UpdateTriggersLabel(true);
            _mainForm.SetUpdateTimeLabel($"{UpdateTimeMsec} msec", true);

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
                    UpdateButtonsLabel();
                    UpdateTriggersLabel();
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
                Thread.Sleep(UpdateTimeMsec);
            }           
        }
    }
}