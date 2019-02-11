using System;
using System.Threading;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger
{
    public sealed class ForegroundChecker
    {
        #region singleton
        private static ForegroundChecker _instance;
        public static ForegroundChecker Instance => _instance ?? (_instance = new ForegroundChecker());
        private ForegroundChecker()
        {
        }
        #endregion

        private Thread _fcThread;
        private IntPtr _lastDemonbuddy;
        private IntPtr _lastDiablo;

        public void Start()
        {
            _fcThread?.Abort();
            _fcThread = new Thread(ForegroundCheckerWorker) { IsBackground = true, Name = "ForegroundCheckerWorker" };
            _fcThread.Start();
        }

        public void Stop()
        {
            _fcThread.Abort();
        }

        private void ForegroundCheckerWorker()
        {
            try
            {
                while (true)
                {
                    var bots = BotSettings.Instance.Bots;
                    var hwnd = WinApi.GetForegroundWindow();

                    if (_lastDemonbuddy != hwnd && _lastDiablo != hwnd)
                    {
                        _lastDemonbuddy = _lastDiablo = IntPtr.Zero;
                        foreach (var bot in bots)
                        {
                            var time = DateTime.UtcNow;
                            if (!bot.IsStarted || !bot.IsRunning || !bot.Diablo.IsRunning || !bot.Demonbuddy.IsRunning)
                                continue;
                            if (bot.Diablo.Proc.MainWindowHandle != hwnd)
                                continue;

                            _lastDiablo = bot.Diablo.MainWindowHandle;
                            _lastDemonbuddy = bot.Demonbuddy.MainWindowHandle;
                            Logger.Instance.WriteGlobal(
                                "<{0}> Diablo:{1}: has focus. Bring attached Demonbuddy to front", bot.Name,
                                bot.Diablo.Proc.Id);

                            // Bring demonbuddy to front
                            WinApi.ShowWindow(_lastDemonbuddy, WinApi.WindowShowStyle.ShowNormal);
                            WinApi.SetForegroundWindow(_lastDemonbuddy);
                            var timeout = DateTime.UtcNow;
                            while (WinApi.GetForegroundWindow() != _lastDemonbuddy)
                            {
                                if (General.DateSubtract(timeout, false) > 500)
                                {
                                    WinApi.ShowWindow(_lastDemonbuddy, WinApi.WindowShowStyle.ForceMinimized);
                                    Thread.Sleep(300);
                                    WinApi.ShowWindow(_lastDemonbuddy, WinApi.WindowShowStyle.ShowNormal);
                                    Thread.Sleep(300);
                                    WinApi.SetForegroundWindow(_lastDemonbuddy);
                                    if (WinApi.GetForegroundWindow() != _lastDemonbuddy)
                                        Logger.Instance.WriteGlobal("<{0}> Failed to bring Demonbuddy to front",
                                            bot.Name);
                                    break;
                                }
                                Thread.Sleep(100);
                            }

                            // Switch back to diablo
                            WinApi.ShowWindow(_lastDiablo, WinApi.WindowShowStyle.ShowNormal);
                            WinApi.SetForegroundWindow(_lastDiablo);
                            while (WinApi.GetForegroundWindow() != _lastDiablo)
                            {
                                if (General.DateSubtract(timeout, false) > 500)
                                {
                                    WinApi.ShowWindow(_lastDiablo, WinApi.WindowShowStyle.ForceMinimized);
                                    Thread.Sleep(300);
                                    WinApi.ShowWindow(_lastDiablo, WinApi.WindowShowStyle.ShowNormal);
                                    Thread.Sleep(300);
                                    WinApi.SetForegroundWindow(_lastDiablo);
                                    break;
                                }
                                Thread.Sleep(100);
                            }

                            // calculate sleeptime
                            var sleep = (int)(Program.Sleeptime - DateTime.UtcNow.Subtract(time).TotalMilliseconds);
                            if (sleep > 0)
                                Thread.Sleep(sleep);
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
                Thread.Sleep(5000);
                ForegroundCheckerWorker();
            }
        }
    }
}
