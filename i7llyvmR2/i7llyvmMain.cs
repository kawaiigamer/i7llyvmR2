using Newtonsoft.Json;
using System.Runtime.InteropServices;
using XInputium;
using XInputium.XInput;

namespace i7llyvmR2
{
    internal static class i7llyvmMain
    {
        private static MainWindow _main_form;
        private static XGamepad _gamepad;
        private static CancellationTokenSource _cts;
        private static System.Threading.Timer _save_timer;
        private static GamepadStatistics _statistics;
        private static float _lastPositionLT = 0;
        private static float _lastPositionRT = 0;
        private static bool _apps_pressed = false;

        private const string LogFileName = "XboxControllerStatistics.json";
        private static readonly string saved_log_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), LogFileName);
        private static readonly TimeSpan LogFileUpdateTick = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan MainUpdateLoopTick = TimeSpan.FromMilliseconds(10);
        private static readonly object _locker = new();
        private static bool IsMainFormInactive => Form.ActiveForm != _main_form;
        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            const int apps_key = 0x5D;
            const int slash_key = 0xBF;
            const int quotes_key = 0xDE;
            const int backslash_key = 0xDC;
            const int plus_key = 0xBB;

            int vkCode = Marshal.ReadInt32(lParam);
            switch (vkCode)
            {
                case apps_key:
                    Interlocked.Exchange(ref _apps_pressed, true);
                    break;

                case slash_key:
                    if (_apps_pressed)
                    {
                        if (IsMainFormInactive)
                        {
                            if (_main_form.Visible)
                            {
                                _main_form.Activate();
                            }
                            else
                            {
                                _main_form.Show();
                            }
                        }
                        else
                        {
                            _main_form.Hide();
                        }
                    }
                    break;

                case quotes_key:
                    if (_apps_pressed && _main_form.Visible)
                    {
                        _main_form.WindowState = _main_form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    }
                    break;

                case backslash_key:
                    if (_apps_pressed && _main_form.Visible)
                    {
                        _main_form.ManuallyExit();
                    }
                    break;

                case plus_key:
                    if (_apps_pressed)
                    {
                        _main_form.ClearStatistics();
                    }
                    break;

                default:
                    Interlocked.Exchange(ref _apps_pressed, false);
                    return IntPtr.Zero;
            }
            return 1;
        }

        private static void StandartExceptionHandler(Exception exp)
        {
            UpdateErrorLabel(exp.Message);
        }

        private static TimerCallback CreateLock(Action<object?> f, object l, Action<Exception> exceptionCallback, int timeout = 1000) => (e) =>
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

        private static void RunLocked(Action a) => CreateLock(_ => a(), _locker, StandartExceptionHandler).Invoke(null);

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
            _main_form.SetButtonsLabel(_statistics.ToStringButtons(), uiThread);
        }

        private static void UpdateTriggersLabel(bool uiThread = false)
        {
            _main_form.SetTriggersLabel(_statistics.ToStringTriggers(), uiThread);
        }

        private static void UpdateErrorLabel(string txt, bool uiThread = false)
        {
            _main_form.SetErrorLabel(txt, uiThread);
        }

        private static GamepadStatistics LoadStatistics()
        {
            try
            {
                return JsonConvert.DeserializeObject<GamepadStatistics>(File.ReadAllText(saved_log_path)) ?? new();
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is JsonException)
            {
                UpdateErrorLabel($"Exception while loading json statistics: {ex.Message}", true);
                return new();
            }
        }

        private static void SaveStatistics()
        {
            string jsonString = JsonConvert.SerializeObject(_statistics);
            File.WriteAllText(saved_log_path, jsonString);
        }

        private static void ReleaseResources()
        {
            RunLocked(() =>
            {
                InterceptKeys.UnregisterKeyboardHook();
                _cts.Cancel();
                _save_timer.Dispose();
                SaveStatistics();
            });
        }

        [STAThread]
        static async Task Main()
        {
            _save_timer = new System.Threading.Timer(CreateLock((e) =>
            {
                SaveStatistics();
            }, _locker, StandartExceptionHandler), null, LogFileUpdateTick, LogFileUpdateTick);

            ApplicationConfiguration.Initialize();
            _main_form = new MainWindow();
            _statistics = LoadStatistics();
            UpdateButtonsLabel(true);
            UpdateTriggersLabel(true);

            _gamepad = new();
            _main_form.SetUpdateTimeLabel($"IDX: {_gamepad.Device.UserIndex.ToString()} {MainUpdateLoopTick.Milliseconds} msec", true);

            _gamepad.ButtonReleased += (s, e) => RunLocked(() => GamepadButtonReleased(s, e));
            _gamepad.LeftTrigger.IsMovingChanged += (s, e) => RunLocked(() =>
            {
                if (_lastPositionLT != 1 && _gamepad.LeftTrigger.Value == 1)
                {
                    _statistics.LT++;
                    UpdateTriggersLabel();
                }
                _lastPositionLT = _gamepad.LeftTrigger.Value;
            });

            _gamepad.RightTrigger.IsMovingChanged += (s, e) => RunLocked(() =>
            {
                if (_lastPositionRT != 1 && _gamepad.RightTrigger.Value == 1)
                {
                    _statistics.RT++;
                    UpdateTriggersLabel();
                }
                _lastPositionRT = _gamepad.RightTrigger.Value;
            });
            _main_form.appManuallyExitEvent += () =>
            {
                ReleaseResources();
                Environment.Exit(Environment.ExitCode);
            };
            _main_form.clearStatisticsEvent += () => RunLocked(() =>
            {
                _statistics = new();
                SaveStatistics();
                UpdateButtonsLabel();
                UpdateTriggersLabel();
            });

            InterceptKeys.RegisterKeyboardHook(KeyboardHookCallback);
            _cts = new CancellationTokenSource();
            Task.Run(async () => await MainUpdateLoop(_cts.Token));

            try
            {
                Application.Run(_main_form);
            }
            finally
            {
                ReleaseResources();
            }
        }

        private static async Task MainUpdateLoop(CancellationToken token)
        {
            using PeriodicTimer timer = new(MainUpdateLoopTick);
            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                   _gamepad.Update(); 
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}