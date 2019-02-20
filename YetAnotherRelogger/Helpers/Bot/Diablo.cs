using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using Serilog;
using YetAnotherRelogger.Helpers.Attributes;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class Diablo
    {
        private ILogger _logger = Logger.Instance.GetLogger<Diablo>();

        [XmlIgnore]
        public Rectangle AutoPos;
        [XmlIgnore]
        public IntPtr MainWindowHandle;
        [XmlIgnore]
        public Process Proc;
        [XmlIgnore]
        private bool _isStopped;
        [XmlIgnore]
        private DateTime _lastRepsonse;
        [XmlIgnore]
        private DateTime _timeStartTime;

        private bool _isLoggedIn;

        public Diablo()
        {
            CpuCount = Environment.ProcessorCount;
            ProcessorAffinity = AllProcessors;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [XmlIgnore]
        [NoCopy]
        public Bot Parent { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }
        /// <summary>
        /// Gets the location2.
        /// </summary>
        /// <value>
        /// The location2.
        /// </value>
        public string Location2
        {
            get
            {
                var ret = Parent.UseDiabloClone
                    ? Path.Combine(Parent.DiabloCloneLocation, Path.GetFileName(Path.GetDirectoryName(Location)),
                        Path.GetFileName(Location))
                    : Location;
                Debug.WriteLine("File Location: {0}", ret);
                return ret;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use is boxer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use is boxer]; otherwise, <c>false</c>.
        /// </value>
        public bool UseIsBoxer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [reused window].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [reused window]; otherwise, <c>false</c>.
        /// </value>
        public bool ReusedWindow { get; set; }
        /// <summary>
        /// Gets or sets the display slot.
        /// </summary>
        /// <value>
        /// The display slot.
        /// </value>
        public string DisplaySlot { get; set; }
        /// <summary>
        /// Gets or sets the character set.
        /// </summary>
        /// <value>
        /// The character set.
        /// </value>
        public string CharacterSet { get; set; }
        /// <summary>
        /// Gets or sets if we launch the entire character set from ISBoxer
        /// </summary>
        public bool IsBoxerLaunchCharacterSet { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [manual position size].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [manual position size]; otherwise, <c>false</c>.
        /// </value>
        public bool ManualPosSize { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public bool NoFrame { get; set; }
        public bool UseAuthenticator { get; set; }
        /// <summary>
        /// Gets or sets the serial. (US-1234-4567-8901)
        /// </summary>
        /// <value>
        /// The serial.
        /// </value>
        public string Serial { get; set; }
        /// <summary>
        /// Gets or sets the serial2. (US123445678901)
        /// </summary>
        /// <value>
        /// The serial2.
        /// </value>
        public string Serial2 { get; set; }
        /// <summary>
        /// Gets or sets the restore code.
        /// </summary>
        /// <value>
        /// The restore code.
        /// </value>
        public string RestoreCode { get; set; }
        /// <summary>
        /// Gets or sets the cpu count.
        /// </summary>
        /// <value>
        /// The cpu count.
        /// </value>
        public int CpuCount { get; set; }
        /// <summary>
        /// Gets or sets the processor affinity.
        /// </summary>
        /// <value>
        /// The processor affinity.
        /// </value>
        public int ProcessorAffinity { get; set; }

        /// <summary>
        /// Gets all processors.
        /// </summary>
        /// <value>
        /// All processors.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning => (Proc != null && !Proc.HasExited && !_isStopped);

        /// <summary>
        /// If Diablo3 is on the lobby screen or in game.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                if (!IsRunning) _isLoggedIn = false;
                return _isLoggedIn;
            }
            set => _isLoggedIn = value;
        }

        /// <summary>
        /// Crashes the check.
        /// </summary>
        public void CrashCheck()
        {
            if (Proc == null)
            {
                IsLoggedIn = false;
                return;
            }

            if (Proc.HasExited)
            {
                IsLoggedIn = false;
                return;
            }

            if (Proc.Responding)
            {
                _lastRepsonse = DateTime.UtcNow;
                Parent.Status = "Monitoring";
            }
            else
                Parent.Status = $"Diablo is unresponsive ({DateTime.UtcNow.Subtract(_lastRepsonse).TotalSeconds} secs)";

            if (Settings.Default.AllowKillGame && DateTime.UtcNow.Subtract(_lastRepsonse).TotalSeconds > 120)
            {
                _logger.Information("Diablo: Is unresponsive for more than 120 seconds");
                _logger.Warning("Diablo: Killing process");
                try
                {
                    if (Proc != null && !Proc.HasExited)
                    {
                        Proc.Kill();
                        IsLoggedIn = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Exception during CrashCheck");
                }
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (!Parent.IsStarted)
                return;

            if (!File.Exists(Location))
            {
                _logger.Warning("File not found: {Location}", Location);
                return;
            }

            _isStopped = false;
            // Ping check
            while (Settings.Default.ConnectionCheckPing && !ConnectionCheck.PingCheck() && !_isStopped)
            {
                Parent.Status = "Wait on internet connection";
                _logger.Warning("PingCheck: Waiting 10 seconds and trying again!");
                Thread.Sleep(10000);
            }

            // Check valid host
            while (Settings.Default.ConnectionCheckHostCheck && Settings.Default.ConnectionCheckIpHost &&
                   !ConnectionCheck.CheckValidConnection() && !_isStopped)
            {
                Parent.Status = "Wait on host validation";
                _logger.Warning("ConnectionValidation: Waiting 10 seconds and trying again!");
                Thread.Sleep(10000);
            }

            // Check if we need to create a Diablo clone
            if (Parent.UseDiabloClone)
                DiabloClone.Create(Parent);

            Parent.Status = "Prepare Diablo"; // Update Status

            //General.AgentKiller(); // Kill all Agent.exe processes

            // Prepare D3 for launch
            //string agentDBPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            //                     @"\Battle.net\Agent\agent.db";
            //if (File.Exists(agentDBPath))
            //{
            //    Logger.Instance.Write("Deleting: {0}", agentDBPath);
            //    try
            //    {
            //        File.Delete(agentDBPath);
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Instance.Write("Failed to delete! Exception: {0}", ex.Message);
            //        DebugHelper.Exception(ex);
            //    }
            //}

            // Copy D3Prefs
            if (!string.IsNullOrEmpty(Parent.D3PrefsLocation))
                D3Prefs();

            // Registry Changes
            RegistryClass.ChangeLocale(Parent.Diablo.Language); // change language
            RegistryClass.ChangeRegion(Parent.Diablo.Region); // change region

            if (UseIsBoxer)
            {
                IsBoxerStarter();
                if (Proc == null)
                {
                    return;
                }
            }
            else if (Proc == null || (Proc != null && Proc.HasExited))
            {
                try
                {
                    var arguments = "-launch";

                    if (Region == "Beta")
                    {
                        arguments += " OnlineService.PTR=true";
                    }

                    var pi = new ProcessStartInfo(Location2, arguments)
                    {
                        WorkingDirectory = Path.GetDirectoryName(Location2),
                        UseShellExecute = false
                    };
                    pi = UserAccount.ImpersonateStartInfo(pi, Parent);
                    // Set working directory to executable location
                    Parent.Status = "Starting Diablo"; // Update Status
                    Proc = Process.Start(pi);
                    if(Proc == null)
                        throw new ApplicationException("Failed to start process");

                    _logger = _logger.ForContext("PID", Proc.Id);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Exception during Start");
                    Parent.Stop();
                    return;
                }
            }

            if (!UseIsBoxer) // Don't want to fight with isboxer
            {
                if (CpuCount != Environment.ProcessorCount)
                {
                    ProcessorAffinity = AllProcessors; // set it to all ones
                    CpuCount = Environment.ProcessorCount;
                }
                Proc.ProcessorAffinity = (IntPtr)ProcessorAffinity;
            }

            if (_isStopped)
                return; // Halt here when bot is stopped while we where waiting for it to become active

            // Wait for d3 to fully load
            var state = (Settings.Default.UseD3Starter || UseIsBoxer ? 0 : 2);
            if (ReusedWindow)
                state = 2;
            var handle = IntPtr.Zero;
            var timedout = false;
            LimitStartTime(true); // reset startup time
            while (!Proc.HasExited && state < 4)
            {
                if (timedout)
                    return;
                //Debug.WriteLine("Splash: " + FindWindow.FindWindowClass("D3 Splash Window Class", Proc.Id) + " Main:" + FindWindow.FindWindowClass("D3 Main Window Class", Proc.Id));
                switch (state)
                {
                    case 0:
                        handle = FindWindow.FindWindowClass("D3 Splash Window Class", Proc.Id);
                        if (handle != IntPtr.Zero)
                        {
                            _logger.Information("Diablo: Found D3 Splash Window ({handle})", handle);
                            state++;
                            LimitStartTime(true); // reset startup time
                        }
                        timedout = LimitStartTime();
                        break;
                    case 1:
                        handle = FindWindow.FindWindowClass("D3 Splash Window Class", Proc.Id);
                        if (handle == IntPtr.Zero)
                        {
                            _logger.Information("Diablo: D3 Splash Window Closed ({handle})", handle);
                            state++;
                            LimitStartTime(true); // reset startup time
                        }
                        timedout = LimitStartTime();
                        break;
                    case 2:
                        handle = FindWindow.FindWindowClass("D3 Main Window Class", Proc.Id);
                        if (handle != IntPtr.Zero)
                        {
                            _logger.Information("Diablo: Found D3 Main Window ({handle})", handle);
                            state++;
                            LimitStartTime(true); // reset startup time
                        }
                        timedout = LimitStartTime();
                        break;
                    case 3:
                        if (CrashChecker.IsResponding(handle))
                        {
                            MainWindowHandle = handle;
                            state++;
                            LimitStartTime(true); // reset startup time
                        }
                        timedout = LimitStartTime();
                        break;
                }
                Thread.Sleep(500);
            }
            if (timedout)
                return;

            if (Program.IsRunAsAdmin)
                Proc.PriorityClass = General.GetPriorityClass(Priority);
            else
                _logger.Information("Diablo: Failed to change priority (No admin rights)");
            // Continue after launching stuff
            _logger.Information("Diablo: Waiting for process to become ready");

            var timeout = DateTime.UtcNow;
            while (true)
            {
                if (Program.Pause)
                {
                    timeout = DateTime.UtcNow;
                    return;
                }
                if (General.DateSubtract(timeout) > 30 || Proc.HasExited)
                {
                    _logger.Information("Diablo: Failed to start!");
                    Parent.Restart();
                    return;
                }
                Thread.Sleep(100);
                try
                {
                    Proc.Refresh();
                    if (Proc.WaitForInputIdle(100) || CrashChecker.IsResponding(MainWindowHandle))
                        break;
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Diablo: Exception during start");
                }
            }

            if (!IsRunning)
                return;

            _lastRepsonse = DateTime.UtcNow;

            Thread.Sleep(1500);
            if (NoFrame)
                AutoPosition.RemoveWindowFrame(MainWindowHandle, true); // Force remove window frame
            if (ManualPosSize)
                AutoPosition.ManualPositionWindow(MainWindowHandle, X, Y, W, H, Parent);
            else if (Settings.Default.UseAutoPos)
                AutoPosition.PositionWindows();

            _logger.Information("Diablo: Process is ready");

            // Demonbuddy start delay
            if (Settings.Default.DemonbuddyStartDelay > 0)
            {
                _logger.Information("Diablo: Demonbuddy start delay, waiting {delay} seconds",
                    Settings.Default.DemonbuddyStartDelay);
                Thread.Sleep((int)Settings.Default.DemonbuddyStartDelay * 1000);
            }
        }

        private bool LimitStartTime(bool reset = false)
        {
            if (Program.Pause)
            {
                _timeStartTime = DateTime.UtcNow;
                return false;
            }
            if (reset)
                _timeStartTime = DateTime.UtcNow;
            else if (General.DateSubtract(_timeStartTime) > (int)Settings.Default.DiabloStartTimeLimit)
            {
                _logger.Information("Diablo: Starting diablo timed out!");
                Parent.Restart();
                return true;
            }
            return false;
        }

        private void ApocD3Starter()
        {
            Parent.Status = "D3Starter: Starting Diablo"; // Update Status
            var d3StarterSuccess = false;
            try
            {
                var starter = new Process
                {
                    StartInfo =
                    {
                        FileName = Settings.Default.D3StarterPath,
                        WorkingDirectory = Path.GetDirectoryName(Settings.Default.D3StarterPath),
                        Arguments = $"\"{Location2}\" 1",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                starter.StartInfo = UserAccount.ImpersonateStartInfo(starter.StartInfo, Parent);
                starter.Start();

                while (!starter.HasExited)
                {
                    var l = starter.StandardOutput.ReadLine();
                    if (l == null)
                        continue;

                    _logger.Information("D3Starter: {l}" + l);
                    Match m;
                    if ((m = Regex.Match(l, @"Process ID (\d+) started.")).Success)
                        Proc = Process.GetProcessById(Convert.ToInt32(m.Groups[1].Value));
                    if (Regex.Match(l, @"\d game instances started! All done!").Success)
                    {
                        d3StarterSuccess = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "D3Starter Error");
            }

            if (!d3StarterSuccess)
            {
                _logger.Information("D3Starter failed!");
                Parent.Stop();
                Parent.Status = "D3Starter Failed!";
            }
        }

        private void IsBoxerStarter()
        {
            if (string.IsNullOrEmpty(Settings.Default.ISBoxerPath) || !File.Exists(Settings.Default.ISBoxerPath))
            {
                _logger.Warning("Diablo: Can't find InnerSpace executable!");
                Parent.Stop();
                return;
            }
            if (string.IsNullOrEmpty(CharacterSet))
            {
                _logger.Warning("Diablo: Is boxer is not configured!");
                Parent.Stop();
                return;
            }

            // Find running ISBoxer
            foreach (var proc in Process.GetProcesses())
            {
                var windowTitle = $"is{DisplaySlot} {DisplaySlot} - {CharacterSet}";

                if (proc.MainWindowTitle == windowTitle)
                {
                    Proc = proc;
                    ReusedWindow = true;
                    _logger.Information("Diablo: Re-using ISBoxer Created Game Window {0} with PID {1}", Proc.MainWindowTitle, Proc.Id);
                    return;
                }
            }
            
            if (Proc == null || (Proc != null && Proc.HasExited))
            {
                ReusedWindow = false;

                var args = IsBoxerLaunchCharacterSet
                    ? $"run isboxer -launch \"{CharacterSet}\""
                    : $"run isboxer -launchslot \"{CharacterSet}\" {DisplaySlot}";

                var isboxer = new Process
                {
                    StartInfo =
                    {
                        FileName = Settings.Default.ISBoxerPath,
                        WorkingDirectory = Path.GetDirectoryName(Settings.Default.ISBoxerPath),
                        Arguments = args,
                    }
                };
                _logger.Information("Diablo: Starting InnerSpace: {0}", Settings.Default.ISBoxerPath);
                _logger.Information("Diablo: With arguments: {0}", isboxer.StartInfo.Arguments);
                //isboxer.StartInfo = UserAccount.ImpersonateStartInfo(isboxer.StartInfo, Parent);
                isboxer.Start();
            }

            // Find diablo process
            var exeName = Path.GetFileNameWithoutExtension(Location);
            _logger.Information("Diablo: Searching for new process: {0}", exeName);
            if (string.IsNullOrEmpty(exeName))
            {
                _logger.Information("Diablo: Failed GetFileNameWithoutExtension!");
                Parent.Stop();
                return;
            }

            // Create snapshot from all running processes
            var currProcesses = Process.GetProcesses();
            var timeout = DateTime.UtcNow;
            while (General.DateSubtract(timeout) < 20)
            {
                Thread.Sleep(250);
                var p = Process.GetProcesses().FirstOrDefault(x =>
                    x.ProcessName.Equals(exeName) &&
                    // Find Diablo inside relogger
                    BotSettings.Instance.Bots.FirstOrDefault(
                        z =>
                            z.Diablo.Proc != null &&
                            !z.Diablo.Proc.HasExited &&
                            z.Diablo.Proc.Id == x.Id) == null &&
                    // Find Diablo in all processes
                    currProcesses.FirstOrDefault(y => y.Id == x.Id) == null);

                if (p == null)
                    continue;
                Proc = p;
                _logger = _logger.ForContext("PID", Proc.Id);
                _logger.Information("Found new Diablo III Name: {ProcessName}", Proc.ProcessName);
                return;
            }

            Logger.Instance.Write(Parent, "Failed to find new Diablo III");
            //Parent.Stop();
        }

        private void D3Prefs()
        {
            using (var imp = new Impersonator())
            {
                if (Parent.UseWindowsUser)
                    imp.Impersonate(Parent.WindowsUserName, "localhost", Parent.WindowsUserPassword);
                // Copy D3Prefs
                _logger.Information("Diablo: Replacing D3Prefs for user: {0}", Environment.UserName);
                var currentprefs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                      @"\Diablo III\D3Prefs.txt";
                if (Directory.Exists(Path.GetDirectoryName(currentprefs)))
                {
                    _logger.Information("Diablo: Copy custom D3Prefs file to: {0}", currentprefs);
                    try
                    {
                        File.Copy(Parent.D3PrefsLocation, currentprefs, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.Information("Diablo: Failed to copy D3Prefs file: {0}", ex);
                    }
                }
                else
                    _logger.Information("Diablo: D3Prefs Failed: Path to \"{0}\" does not exist!", currentprefs);
            }

            // Also replace Default User D3Prefs
            var defaultprefs =
                Regex.Match(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    $@"(.+)\\{Environment.UserName}.*").Groups[1].Value;
            if (Directory.Exists(defaultprefs + "\\Default"))
                defaultprefs += "\\Default";
            else if (Directory.Exists(defaultprefs + "\\Default User"))
                defaultprefs += "\\Default User";
            else
                return;
            defaultprefs += @"\Diablo III\D3Prefs.txt";
            if (Directory.Exists(Path.GetDirectoryName(defaultprefs)))
            {
                _logger.Information("Diablo: Copy custom D3Prefs file to: {0}", defaultprefs);
                try
                {
                    File.Copy(Parent.D3PrefsLocation, defaultprefs, true);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Diablo: Failed to copy d3prefs file: {0}");
                }
            }
            Thread.Sleep(1000);
        }

        public void Stop()
        {
            _isStopped = true;

            if (Proc == null || Proc.HasExited)
                return;

            _logger.Warning("Diablo: Kill process");
            Proc.Kill();
        }
    }
}
