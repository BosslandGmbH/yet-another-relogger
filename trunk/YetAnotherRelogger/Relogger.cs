using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger
{
    public sealed class Relogger
    {
        #region singleton

        private static readonly Relogger instance = new Relogger();

        static Relogger()
        {
        }

        private Relogger()
        {
        }

        public static Relogger Instance
        {
            get { return instance; }
        }

        #endregion

        public BotClass CurrentBot;
        private bool _autoStartDone;
        private bool _isStopped;
        private Thread _threadRelogger;

        public void Start()
        {
            if (_threadRelogger != null && (_threadRelogger == null || _threadRelogger.IsAlive))
                return;

            _isStopped = false;
            _threadRelogger = new Thread(ReloggerWorker) { IsBackground = true, Name = "ReloggerWorker" };
            _threadRelogger.Start();
        }


        public void Stop()
        {
            _isStopped = true;
            _threadRelogger.Abort();
            _threadRelogger = null;
        }

        private DateTime _lastSaveSettings = DateTime.MinValue;

        private void ReloggerWorker()
        {
            // Check if we are launched by windows RUN
            if (CommandLineArgs.WindowsAutoStart && !_autoStartDone)
            {
                _autoStartDone = true;
                Logger.Instance.WriteGlobal("Windows auto start delaying with {0} seconds", Settings.Default.StartDelay);
                Thread.Sleep((int)Settings.Default.StartDelay * 1000);
                foreach (BotClass bot in BotSettings.Instance.Bots.Where(c => c.IsEnabled))
                {
                    bot.AntiIdle.Reset(freshstart: true); // Reset AntiIdle
                    bot.IsStarted = true;
                    bot.Status = "Auto Start...";
                }
            }
            // Check if we are launched with the autostart
            if (CommandLineArgs.AutoStart && !_autoStartDone)
            {
                _autoStartDone = true;
                foreach (BotClass bot in BotSettings.Instance.Bots.Where(c => c.IsEnabled))
                {
                    bot.AntiIdle.Reset(freshstart: true); // Reset AntiIdle
                    bot.IsStarted = true;
                    bot.Status = "Auto Start...";
                }
            }

            DebugHelper.Write("Relogger Thread Starting!");
            while (true)
            {
                try
                {
                    if (_isStopped)
                        return;

                    // Paused
                    if (Program.Pause)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    List<Process> blizzardErrorProcs =
                         (from p in Process.GetProcessesByName("BlizzardError.exe")
                          select p).ToList();
                    if (blizzardErrorProcs.Any())
                    {
                        try
                        {
                            foreach (var p in blizzardErrorProcs)
                            {
                                Logger.Instance.Write("Killing BlizzardError.exe with PID {0}", p.Id);
                                p.Kill();
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write("Exception killing BlizzardError.exe: " + ex);
                        }
                    }

                    if (DateTime.UtcNow.Subtract(_lastSaveSettings).TotalSeconds > 10)
                    {
                        _lastSaveSettings = DateTime.UtcNow;
                        Settings.Default.Save();
                        BotSettings.Instance.Save();
                    }

                    // Check / validate internet connection
                    if (!ConnectionCheck.IsConnected || !ConnectionCheck.ValidConnection)
                    {
                        Debug.WriteLine("Internet validation failed looping until success");
                        Thread.Sleep(1000);
                        continue;
                    }

                    foreach (BotClass bot in BotSettings.Instance.Bots.Where(bot => bot != null).ToList())
                    {
                        if (Program.Pause)
                            break;

                        DateTime time = DateTime.UtcNow; // set current time to calculate sleep time at end of loop
                        CurrentBot = bot;
                        //Debug.WriteLine(bot.Name + ":" + ":" + bot.IsRunning);
                        //Debug.WriteLine("State=" + bot.AntiIdle.State);
                        if (bot.IsRunning && bot.IsStarted && !bot.Week.ShouldRun(bot.IsRunning))
                        {
                            // We need to stop
                            Logger.Instance.Write("We are scheduled to stop");
                            bot.Week.NextSchedule(true);
                            bot.IsRunning = false;
                            bot.Demonbuddy.Stop();
                            bot.Diablo.Stop();
                            bot.Status = "Scheduled stop!";
                        }
                        else if (!bot.IsRunning && bot.IsStarted && bot.Week.ShouldRun(bot.IsRunning))
                        {
                            // we need to start
                            Logger.Instance.Write("We are scheduled to start");
                            bot.Week.NextSchedule(false);
                            bot.IsRunning = true;
                            bot.StartTime = DateTime.UtcNow;
                            StartBoth(bot);
                        }
                        else if (!bot.IsStandby && bot.IsRunning)
                        {
                            // Check if process is responding
                            bot.Diablo.CrashCheck();
                            if (bot.AntiIdle.IsInitialized)
                                bot.Demonbuddy.CrashCheck();

                            if (!bot.Diablo.IsRunning)
                            {
                                if (bot.Diablo.Proc != null)
                                    Logger.Instance.Write("Diablo:{0}: Process is not running", bot.Diablo.Proc.Id);
                                //if (bot.Demonbuddy.IsRunning && bot.Demonbuddy.Proc != null)
                                //{
                                //    Logger.Instance.Write("Demonbuddy:{0}: Closing db", bot.Demonbuddy.Proc.Id);
                                //    bot.Demonbuddy.Stop();
                                //}
                                if (bot.Demonbuddy.IsRunning && bot.Demonbuddy.Proc != null)
                                {
                                    Logger.Instance.Write("Demonbuddy:{0}: Waiting for Demonbuddy to self close", bot.Demonbuddy.Proc.Id);
                                }
                                else
                                {
                                    StartBoth(bot);
                                }
                            }
                            else if (!bot.Demonbuddy.IsRunning)
                            {
                                Logger.Instance.Write("Demonbuddy: Process is not running");
                                bot.Demonbuddy.Start();
                            }
                            else if (Settings.Default.AntiIdleStatsDuration > 0 && bot.AntiIdle.State != IdleState.Initialize && General.DateSubtract(bot.AntiIdle.LastStats) >
                                (double)Settings.Default.AntiIdleStatsDuration)
                            {
                                Logger.Instance.Write("We did not recieve any stats during 300 seconds!");
                                bot.Restart();
                            }
                            else if (bot.AntiIdle.IsInitialized)
                            {
                                if (bot.ProfileSchedule.IsDone)
                                {
                                    Logger.Instance.Write("Profile: \"{0}\" Finished!", bot.ProfileSchedule.Current.Name);
                                    bot.AntiIdle.State = IdleState.NewProfile;
                                }
                            }
                        }
                        else
                        {
                            //Logger.Instance.Write("Bot Standby={0} Running={1} D3Running={2} DBRunning={3}", bot.IsStandby, bot.IsRunning, bot.Diablo.IsRunning, bot.Demonbuddy.IsRunning);
                            bot.StartTime = DateTime.UtcNow;
                        }
                        // calculate sleeptime
                        var sleep = (int)(Program.Sleeptime - DateTime.UtcNow.Subtract(time).TotalMilliseconds);
                        if (sleep > 0)
                            Thread.Sleep(sleep);
                    }
                } // try
                catch (InvalidOperationException)
                {
                    // Catch error when bot is edited while in a loop
                    //Logger.Instance.WriteGlobal(iox.Message);
                    continue;
                }
                catch (Exception ex)
                {
                    if (_isStopped)
                        return;
                    Logger.Instance.WriteGlobal("Relogger Crashed! with message {0}", ex.Message);
                    DebugHelper.Exception(ex);
                    Logger.Instance.WriteGlobal("Waiting 10 seconds and try again!");
                    Thread.Sleep(10000);
                    continue;
                }
                Thread.Sleep(1000);
            } // while
        } // private void reloggerWorker()

        private bool StartBoth(BotClass bot)
        {
            bot.Diablo.Start();
            if (!bot.Diablo.IsRunning)
                return false;

            bot.Demonbuddy.Start();
            if (!bot.Demonbuddy.IsRunning)
                return false;

            bot.Status = "Monitoring";
            return true;
        }
    }
}