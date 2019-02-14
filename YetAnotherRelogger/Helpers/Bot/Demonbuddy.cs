using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using YetAnotherRelogger.Helpers.Attributes;
using YetAnotherRelogger.Helpers.Enums;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class DemonbuddyClass
    {
        private static readonly ILogger s_logger = Logger.Instance.GetLogger<DemonbuddyClass>();

        [XmlIgnore]
        public Rectangle AutoPos;
        [XmlIgnore]
        public IntPtr MainWindowHandle;
        [XmlIgnore]
        private bool _crashTenderRestart;
        [XmlIgnore]
        private bool _isStopped;
        private DateTime _lastResponse;
        [XmlIgnore]
        private Process _proc;

        public DemonbuddyClass()
        {
            CpuCount = Environment.ProcessorCount;
            ProcessorAffinity = AllProcessors;
        }

        [XmlIgnore]
        [NoCopy]
        public BotClass Parent { get; set; }

        [XmlIgnore]
        [NoCopy]
        public Process Proc
        {
            get => _proc;
            set
            {
                if (value != null)
                    Parent.DemonbuddyPid = value.Id.ToString();
                _proc = value;
            }
        }

        [XmlIgnore]
        [NoCopy]
        public bool IsRunning => (Proc != null && !Proc.HasExited && !_isStopped);

        // Buddy Auth
        public string BuddyAuthUsername { get; set; }
        public string BuddyAuthPassword { get; set; }

        [XmlIgnore]
        [NoCopy]
        public DateTime LoginTime { get; set; }

        // Demonbuddy
        public string Location { get; set; }
        public string Key { get; set; }
        public string CombatRoutine { get; set; }
        public bool NoFlash { get; set; }
        public bool AutoUpdate { get; set; }
        public bool NoUpdate { get; set; }
        public int Priority { get; set; }

        // Affinity
        // If CpuCount does not match current machines CpuCount,
        // the affinity is set to all processor
        public int CpuCount { get; set; }
        public int ProcessorAffinity { get; set; }

        [XmlIgnore]
        public int AllProcessors
        {
            get
            {
                var intProcessorAffinity = 0;
                for (var i = 0; i < Environment.ProcessorCount; i++)
                    intProcessorAffinity |= (1 << i);
                return intProcessorAffinity;
            }
        }

        // Position
        public bool ManualPosSize { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public bool ForceEnableAllPlugins { get; set; }

        private const int MaxInits = 15;

        [NoCopy]
        public bool IsInitialized
        {
            get
            {
                // If diablo is no longer running close db and return false
                if (!Parent.Diablo.IsRunning)
                {
                    Parent.Demonbuddy.Stop(true);
                    return false;
                }

                if ((!Parent.AntiIdle.IsInitialized && General.DateSubtract(Parent.AntiIdle.InitTime) > 180) ||
                    !IsRunning)
                {
                    Parent.AntiIdle.FailedInitCount++;

                    if (Parent.AntiIdle.FailedInitCount >= (Parent.AntiIdle.InitAttempts > 0 ? 1 : MaxInits))
                    {
                        Parent.AntiIdle.InitAttempts++;
                        s_logger.Warning("Demonbuddy:{Id}: Failed to initialize more than {MaxInits} times", Parent.Demonbuddy.Proc.Id, MaxInits);
                        Parent.Standby();
                    }
                    else
                    {
                        s_logger.Warning("Demonbuddy:{Id}: Failed to initialize {FailedInitCount}/{MaxInits}", Parent.Demonbuddy.Proc.Id, Parent.AntiIdle.FailedInitCount, MaxInits);
                        Parent.Demonbuddy.Stop(true);
                    }
                    return false;
                }
                return Parent.AntiIdle.IsInitialized;
            }
        }

        [NoCopy]
        private bool GetLastLoginTime
        {
            get
            {
                // No info to get from any process
                if (Proc == null)
                    return false;

                // get log dir
                var logdir = Path.Combine(Path.GetDirectoryName(Location), "Logs");
                if (logdir.Length == 0 || !Directory.Exists(logdir))
                {
                    // Failed to get log dir so exit here
                    s_logger.Warning("Demonbuddy:{Id}: Failed to find logdir", Proc.Id);
                    return false;
                }
                // get log file
                var logfile = string.Empty;
                var success = false;
                DateTime starttime = Proc.StartTime;
                // Loop a few times if log is not found on first attempt and add a minute for each loop
                for (var i = 0; i <= 3; i++)
                {
                    // Test if logfile exists for current process starttime + 1 minute
                    logfile = $"{logdir}\\{Proc.Id} {starttime.AddMinutes(i):yyyy-MM-dd HH.mm}.txt";
                    if (File.Exists(logfile))
                    {
                        success = true;
                        break;
                    }
                }

                if (success)
                {
                    s_logger.Information("Demonbuddy:{Id}: Found matching log: {logfile}", Proc.Id, logfile);

                    // Read Log file
                    // [11:03:21.173 N] Logging in...
                    try
                    {
                        var lineNumber = -1;
                        using (var fs = new FileStream(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var reader = new StreamReader(fs);
                            var time = new TimeSpan();
                            var logging = false;
                            while (!reader.EndOfStream)
                            {
                                // only read 1000 lines from log file, so we don't spend all day looking through the log.
                                lineNumber++;

                                if (lineNumber > 1000)
                                    break;

                                var line = reader.ReadLine();
                                if (line == null)
                                    continue;

                                if (logging && line.Contains("Attached to Diablo III with pid"))
                                {
                                    LoginTime = DateTime.Parse($"{starttime.ToUniversalTime():yyyy-MM-dd} {time}");
                                    s_logger.Information("Found login time: {LoginTime}", LoginTime);
                                    return true;
                                }
                                Match m = new Regex(@"^\[(.+) .\] Logging in\.\.\.$", RegexOptions.Compiled).Match(line);
                                if (m.Success)
                                {
                                    time = TimeSpan.Parse(m.Groups[1].Value);
                                    logging = true;
                                }

                                Thread.Sleep(5); // Be nice for CPU
                            }
                            s_logger.Warning("Demonbuddy:{Id}: Failed to find login time", Proc.Id);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        s_logger.Error(ex, "Demonbuddy:{Id}: Error accured while reading log", Proc.Id);
                    }
                }
                // Else print error + return false
                s_logger.Warning("Demonbuddy:{Id}: Failed to find matching log", Proc.Id);
                return false;
            }
        }

        public void CrashCheck()
        {
            if (Proc.HasExited)
                return;

            if (Proc.Responding)
                _lastResponse = DateTime.UtcNow;

            if (DateTime.UtcNow.Subtract(Proc.StartTime).TotalMilliseconds < (90 * 1000))
                return;

            if (Settings.Default.AllowKillDemonbuddy && _lastResponse.AddSeconds(120) < DateTime.UtcNow)
            {
                s_logger.Information("Demonbuddy:{Id}: Is unresponsive for more than 120 seconds", Proc.Id);
                s_logger.Warning("Demonbuddy:{Id}: Killing process", Proc.Id);
                try
                {
                    Proc.Kill();
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, "Failed to kill process");
                }
            }
            else if (Settings.Default.AllowKillDemonbuddy && _lastResponse.AddSeconds(90) < DateTime.UtcNow)
            {
                s_logger.Information("Demonbuddy:{Id}: Is unresponsive for more than 90 seconds", Proc.Id);
                s_logger.Warning("Demonbuddy:{Id}: Closing process", Proc.Id);
                try
                {
                    if (Proc != null && !Proc.HasExited)
                        Proc.CloseMainWindow();
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, "Failed to close process");
                }
            }
        }

        public void Start(bool noprofile = false, string profilepath = null, bool crashtenderstart = false)
        {
            if (!Parent.IsStarted || !Parent.Diablo.IsRunning || (_crashTenderRestart && !crashtenderstart))
                return;

            if (!File.Exists(Location))
            {
                s_logger.Error("File not found: {Location}", Location);
                return;
            }

            while (Parent.IsStarted && Parent.Diablo.IsRunning)
            {
                // Get Last login time and kill old session
                if (GetLastLoginTime)
                    BuddyAuth.Instance.KillSession(Parent);

                _isStopped = false;

                // Reset AntiIdle;
                Parent.AntiIdle.Reset(true);

                var arguments = "-pid=" + Parent.Diablo.Proc.Id;
                arguments += " -key=" + Key;
                arguments += " -autostart";
                //arguments += $" -routine=TrinityRoutine";
                //arguments += $" -routine=\"{CombatRoutine}\"";

                arguments += $" -logformat=Json -logport={Settings.Default.LogListenerPort}";

                arguments += $" -bnetaccount=\"{Parent.Diablo.Username}\"";
                arguments += $" -bnetpassword=\"{Parent.Diablo.Password}\"";

                if (Parent.Diablo.UseAuthenticator)
                {
                    //-bnetaccount="blah@blah.com" -bnetpassword="LOL" -authenticatorrestorecode="..." -authenticatorserial="EU-..."
                    arguments += $" -authenticatorrestorecode=\"{Parent.Diablo.RestoreCode}\"";
                    arguments += $" -authenticatorserial=\"{Parent.Diablo.Serial}\"";
                }

                //if (profilepath != null)
                //{
                //    // Check if current profile path is Kickstart
                //    string file = Path.GetFileName(profilepath);
                //    if (file == null || (file.Equals("YAR_Kickstart.xml") || file.Equals("YAR_TMP_Kickstart.xml")))
                //        profilepath = Parent.ProfileSchedule.Current.Location;

                //    var profile = new Profile { Location = profilepath };
                //    string path = ProfileKickstart.GenerateKickstart(profile);
                //    Logger.Instance.Write("Using Profile {0}", path);
                //    arguments += string.Format(" -profile=\"{0}\"", string.Empty);
                //}
                //else if (Parent.ProfileSchedule.Profiles.Count > 0 && !noprofile)
                //{
                //    string path = Parent.ProfileSchedule.GetProfile;
                //    Logger.Instance.Write("Using Scheduled Profile {0}", path);
                //    if (File.Exists(path))
                //        arguments += string.Format(" -profile=\"{0}\"", path);
                //}
                //else if (!noprofile)
                //    Logger.Instance.Write(
                //        "Warning: Launching Demonbuddy without a starting profile (Add a profile to the profilescheduler for this bot)");

                if (NoFlash)
                    arguments += " -noflash";
                if (AutoUpdate)
                    arguments += " -autoupdate";
                if (NoUpdate)
                    arguments += " -noupdate";

                //var kickstart = ProfileKickstart.GenerateKickstart(Parent.Demonbuddy.Location);

                // Kickstarter is required for two reasons
                //
                // 1) For DB to 'start' automatically - like clicking the start button - it needs a 'current profile'.
                // 2) The bot will not log in at D3 login page if it doesnt have a 'current profile'.
                //
                // Once logged in DB will automatically use the last profile listed in Settings\GlobalSettings.xml <LastProfile>. 
                // Using a -profile argument ensures that the current profile won't be empty / a null when DB initializes.
                //
                // If D3 is in game or in the hero selection screen. Kickstarter is not required.

                //if (!Parent.Diablo.IsLoggedIn)
                //{ 
                //    arguments += string.Format(" -profile=\"{0}\"", kickstart);
                //}

                if (ForceEnableAllPlugins)
                    arguments += " -YarEnableAll";

                // Don't expose arguments in release builds.
#if DEBUG
                s_logger.Verbose("DB Arguments: {arguments}", arguments);
#endif
                var p = new ProcessStartInfo(Location, arguments) { WorkingDirectory = Path.GetDirectoryName(Location), UseShellExecute = false };
                p = UserAccount.ImpersonateStartInfo(p, Parent);

                DateTime timeout;
                try // Try to start Demonbuddy
                {
                    Parent.Status = "Starting Demonbuddy"; // Update Status
                    Proc = Process.Start(p);

                    if (Program.IsRunAsAdmin)
                        Proc.PriorityClass = General.GetPriorityClass(Priority);
                    else
                        s_logger.Error("Failed to change priority (No admin rights)");

                    // Set affinity
                    if (CpuCount != Environment.ProcessorCount)
                    {
                        ProcessorAffinity = AllProcessors; // set it to all ones
                        CpuCount = Environment.ProcessorCount;
                    }
                    Proc.ProcessorAffinity = (IntPtr)ProcessorAffinity;

                    s_logger.Information("Demonbuddy:{Id}: Waiting for process to become ready", Proc.Id);

                    timeout = DateTime.UtcNow;
                    while (true)
                    {
                        if (Program.Pause)
                        {
                            return;
                        }
                        if (General.DateSubtract(timeout) > 60)
                        {
                            s_logger.Warning("Demonbuddy:{Id}: Failed to start!", Proc.Id);
                            Parent.Restart();
                            return;
                        }
                        Thread.Sleep(500);
                        try
                        {
                            Proc.Refresh();

                            if (Proc != null && Proc.HasExited && Proc.ExitCode == 12)
                            {
                                Proc.WaitForExit();
                                s_logger.Fatal("Closing YAR due to Tripwire event. Please check the forums for more information.");
                                Application.Exit();
                            }

                            if (Proc.WaitForInputIdle(100) || CrashChecker.IsResponding(MainWindowHandle))
                                break;
                        }
                        catch (Exception ex)
                        {
                            s_logger.Error(ex, "Error while waiting for idle input");
                        }
                    }

                    if (_isStopped)
                        return;
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, "Error during startup");
                    Parent.Stop();
                }

                timeout = DateTime.UtcNow;
                while (!FindMainWindow())
                {
                    if (General.DateSubtract(timeout) > 30)
                    {
                        MainWindowHandle = Proc.MainWindowHandle;
                        break;
                    }
                    Thread.Sleep(500);
                }

                //// When DB starts and creates a new .exe file it renames the associated .config file
                //// The original Demonbuddy.exe.config is required for plugins using SharpSVN.dll
                //// So until this issue is fixed, copy and rename the config.
                //var dbfolder = Path.GetDirectoryName(Parent.Demonbuddy.Location);
                //var configs = System.IO.Directory.GetFiles(dbfolder, "*.config");
                //var targetFileName = "Demonbuddy.exe.config";
                //if (configs.Any())
                //{
                //    var sourcePath = configs.FirstOrDefault(c => c != targetFileName);
                //    if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
                //    {
                //        var targetPath = Path.Combine(dbfolder, targetFileName);
                //        File.Copy(sourcePath, targetPath);
                //    }
                //}            

                // Window postion & resizing
                if (ManualPosSize)
                    AutoPosition.ManualPositionWindow(MainWindowHandle, X, Y, W, H, Parent);
                s_logger.Information("Demonbuddy:{Id}: Process is ready", Proc.Id);

                // Wait for demonbuddy to be Initialized (this means we are logged in)
                // If we don't wait here the Region changeing for diablo fails!
                s_logger.Information("Demonbuddy:{Id}: Waiting for demonbuddy to log into Diablo", Proc.Id);
                while (!IsInitialized && !_isStopped)
                    Thread.Sleep(1000);
                // We made to many attempts break here
                if (Parent.AntiIdle.FailedInitCount > 3)
                    break;
                if (!Parent.AntiIdle.IsInitialized)
                    continue; // Retry

                /*
                //!!!
                //Parent.Diablo.Proc.Id

                if (Parent.Diablo.UseAuthenticator)
                {

                    Thread.Sleep(1000);

                    Logger.Instance.Write("Diablo:{0}: Trying to authentificate", Parent.Diablo.Proc.Id);
                    BattleNetAuthenticator auth = new BattleNetAuthenticator();
                    auth.Restore(Parent.Diablo.Serial2, Parent.Diablo.RestoreCode);
                    string authcode = Convert.ToString(auth.CurrentCode);

                    try
                    {
                        SetForegroundWindow(hControl);
                        SendKeys.SendWait(authcode);
                        SendKeys.SendWait("~");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }

                //!!!
                */

                // We are ready to go
                s_logger.Information("Demonbuddy:{Id}: Initialized! We are ready to go", Proc.Id);
                Parent.AntiIdle.FailedInitCount = 0; // only reset counter
                break;
            } // while (Parent.IsStarted && Parent.Diablo.IsRunning)
        }

        private bool FindMainWindow()
        {
            IntPtr handle = FindWindow.EqualsWindowCaption("Demonbuddy", Proc.Id);
            if (handle != IntPtr.Zero)
            {
                MainWindowHandle = handle;
                s_logger.Verbose("Found Demonbuddy: MainWindow ({handle})", handle);
                return true;
            }
            handle = FindWindow.EqualsWindowCaption("Demonbuddy - BETA", Proc.Id);
            if (handle != IntPtr.Zero)
            {
                MainWindowHandle = handle;
                s_logger.Verbose("Found Demonbuddy - BETA: MainWindow ({handle})", handle);
                return true;
            }
            return false;
        }

        public void Stop(bool force = false)
        {
            _isStopped = true;

            if (Proc == null || Proc.HasExited)
                return;

            // Force close
            if (force)
            {
                s_logger.Warning("Demonbuddy:{Id}: Process is not responding, killing!", Proc.Id);
                Proc.Kill();
                return;
            }

            if (Proc != null && !Proc.HasExited)
            {
                s_logger.Information("Demonbuddy:{Id}: Closing window", Proc.Id);
                Proc.CloseMainWindow();
            }
            if (Parent.Diablo.Proc == null || Parent.Diablo.Proc.HasExited)
            {
                s_logger.Verbose("Demonbuddy:{Id}: Waiting to close", Proc.Id);
                Proc.CloseMainWindow();
                Parent.AntiIdle.State = IdleState.Terminate;
                Proc.WaitForExit(60000);
                if (Proc == null || Proc.HasExited)
                {
                    s_logger.Information("Demonbuddy:{Id}: Closed.", Proc.Id);
                    return;
                }
            }

            if (Proc.HasExited)
                s_logger.Information("Demonbuddy:{Id}: Closed.", Proc.Id);
            else if (!Proc.Responding)
            {
                s_logger.Error("Demonbuddy:{Id}: Failed to close! kill process", Proc.Id);
                Proc.Kill();
            }
        }

        public void CrashTender(string profilepath = null)
        {
            _crashTenderRestart = true;
            s_logger.Information("CrashTender: Stopping Demonbuddy:{Id}", Proc.Id);
            Stop(true); // Force DB to stop
            s_logger.Information("CrashTender: Starting Demonbuddy without a starting profile");

            if (profilepath != null)
                Start(profilepath: profilepath, crashtenderstart: true);
            else
                Start(true, crashtenderstart: true);
            _crashTenderRestart = false;
        }
    }
}
