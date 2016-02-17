using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Documents;
using System.Xml.Serialization;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;
using UIElement = Zeta.Game.Internals.UIElement;

namespace YARPLUGIN
{
    public class YARPLUGIN : IPlugin
    {
        // Plugin version
        public Version Version { get { return new Version(0, 3, 1, 2); } }

        private const bool _debug = true;
        private static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

        // Compatibility
        private static readonly Regex[] ReCompatibility =
            {
                /* BuddyStats Remote control action */
                new Regex(@"Stop command from BuddyStats", RegexOptions.Compiled), // stop command
                /* Emergency Stop: You need to stash an item but no valid space could be found. Stash is full? Stopping the bot to prevent infinite town-run loop. */
                new Regex(@".+Emergency Stop: .+", RegexOptions.Compiled), // Emergency stop
                /* Atom 2.0.15+ "Take a break" */
                new Regex(@".*Atom.*Will Stop the bot for .+ minutes\.$", RegexOptions.Compiled), // Take a break
                /* RadsAtom "Take a break" */
                new Regex(@"\[RadsAtom\].+ minutes to next break, the break will last for .+ minutes.", RegexOptions.Compiled), 
                /* Take A Break by Ghaleon */
                new Regex(@"\[TakeABreak.*\] It's time to take a break.*", RegexOptions.Compiled), 
            };

        // CrashTender
        private static readonly Regex[] ReCrashTender =
            {
                /* Invalid Session */
                new Regex(@"Session is invalid!", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                /* Session expired */
                new Regex(@"Session is expired", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                /* Failed to attach to D3*/
                new Regex(@"Was not able to attach to any running Diablo III process, are you running the bot already\?", RegexOptions.Compiled), 
                new Regex(@"Traceback (most recent call last):", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            };

        private static readonly Regex[] CrashExceptionRegexes = 
        {
                new Regex(@"Exception during bot tick.*", RegexOptions.Compiled)
        };
        private static int crashExceptionCounter;

        private static readonly Regex waitingBeforeGame = new Regex(@"Waiting (.+) seconds before next game", RegexOptions.Compiled);
        private static readonly Regex pluginsCompiled = new Regex(@"There are \d+ plugins", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex logMessageCapture = new Regex(@"^\[(?<Timestamp>[\d:\.]+) (?<LogLevel>[NDVE])\] (?<Message>.*)$", RegexOptions.Compiled);
        private static readonly Regex yarRegex = new Regex(@"^\[YetAnotherRelogger\].*", RegexOptions.Compiled);
        private static readonly string d3Exit = "Diablo III Exited";
        private static readonly string getCellWeightsException = "Zeta.Navigation.MainGridProvider.GetCellWeights";

        private log4net.Core.Level initialLogLevel = null;

        public class BotStats
        {
            public int Pid;
            public long LastRun;
            public long LastPulse;
            public long PluginPulse;
            public long LastGame;
            public bool IsPaused;
            public bool IsRunning;
            public bool IsInGame;
            public bool IsLoadingWorld;
            public long Coinage;
            public long Experience;

            public override string ToString()
            {
                return string.Format("Pid={0} LastRun={1} LastPulse={2} PluginPulse={3} LastGame={4} IsPaused={5} IsRunning={6} IsInGame={7} IsLoadingWorld={8} Coinage={9} Experience={10}",
                    Pid, LastRun, LastPulse, PluginPulse, LastGame, IsPaused, IsRunning, IsInGame, IsLoadingWorld, Coinage, Experience);
            }
        }
        #region Plugin information
        public string Author { get { return "rrrix and sinterlkaas"; } }
        public string Description { get { return "Communication plugin for YetAnotherRelogger"; } }
        public string Name { get { return "YAR Comms"; } }
        public bool Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }
        #endregion

        public Window DisplayWindow { get { return null; } }
        private bool _allPluginsCompiled;
        private Thread _yarThread = null;

        private BotStats _bs = new BotStats();
        private bool _pulseFix;
        private YARAppender YARAppender = new YARAppender();

        public bool IsEnabled { get; set; }

        public static void Log(string str)
        {
            Log(str, 0);
        }
        public static void Log(Exception ex)
        {
            Log(ex.ToString(), 0);
        }
        public static void Log(string str, params object[] args)
        {
            DBLog.InfoFormat("[YetAnotherRelogger] " + str, args);
        }
        public static void LogException(Exception ex)
        {
            Log(ex.ToString());
        }

        #region Plugin Events
        public YARPLUGIN()
        {
            _bs.Pid = Process.GetCurrentProcess().Id;
        }

        private bool _appenderAdded;
        private object _appenderLock = 0;
        public void OnInitialize()
        {
            Log("YAR Plugin Initialized with PID: {0}", _bs.Pid);

            // Force enable YAR
            var enabledPluginsList = PluginManager.Plugins.Where(p => p.Enabled).Select(p => p.Plugin.Name).ToList();
            if (!enabledPluginsList.Contains(Name))
                enabledPluginsList.Add(Name);
            PluginManager.SetEnabledPlugins(enabledPluginsList.ToArray());

            _bs = new BotStats();
            _bs.LastPulse = DateTime.UtcNow.Ticks;

            lock (_appenderLock)
            {
                if (!_appenderAdded)
                {
                    Hierarchy loggingHierarchy = (Hierarchy)LogManager.GetRepository();
                    loggingHierarchy.Root.AddAppender(YARAppender);
                    _appenderAdded = true;
                }
            }

            Reset();

            StartYarWorker();

            //Pulsator.OnPulse += Pulsator_OnPulse;
            //TreeHooks.Instance.OnHooksCleared += Instance_OnHooksCleared;

            //if (ProfileUtils.IsProfileYarKickstart)
            //{
            //    Log("[YAR] Kickstart Profile detected");

            //    if (ZetaDia.Service.Hero.IsValid && ZetaDia.Service.Hero.HeroId > 0)
            //    {
            //        Log("[YAR] Logged in and hero is valid");

            //        var realProfile = ProfileUtils.GetProfileAttribute("LoadProfile", "profile");
            //        if (!string.IsNullOrEmpty(realProfile))
            //        {
            //            Log("[YAR] Loading profile: {0}", realProfile);
            //            ProfileManager.Load(ProfileManager.CurrentProfile.Path);
            //        }
            //    }
            //}

            Log("Requesting Profile (Current={0})", ProfileManager.CurrentProfile != null ? ProfileManager.CurrentProfile.Path : "Null");

            Send("RequestProfile");

            Send("Initialized");
        }

        public class ProfileUtils
        {
            public static bool ProfileHasTag(string tagName)
            {
                var profile = ProfileManager.CurrentProfile;
                if (profile != null && profile.Element != null)
                {
                    return profile.Element.XPathSelectElement("descendant::" + tagName) != null;
                }
                return false;
            }

            public static XElement GetProfileTag(string tagName, XElement element = null)
            {
                if (element == null)
                {
                    var profile = ProfileManager.CurrentProfile;
                    if (profile != null && profile.Element != null)
                    {
                        element = profile.Element;
                    }
                    else
                    {
                        return null;
                    }
                }

                return element.XPathSelectElement("descendant::" + tagName);
            }

            public static bool IsProfileYarKickstart
            {
                get
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ProfileManager.CurrentProfile.Path);
                    return fileNameWithoutExtension != null && fileNameWithoutExtension.ToLower().StartsWith("yar_kickstart");
                }
            }

            public static string GetProfileAttribute(string tagName, string attrName, XElement element = null)
            {
                var profileTag = GetProfileTag(tagName, element);
                if (profileTag != null)
                {
                    var behaviorAttr = profileTag.Attribute(attrName);
                    if (behaviorAttr != null && !string.IsNullOrEmpty(behaviorAttr.Value))
                    {
                        return behaviorAttr.Value;
                    }
                }
                return string.Empty;
            }
        }

        void BotMain_OnStart(IBot bot)
        {
            InsertOrRemoveHook();
        }

        void Instance_OnHooksCleared(object sender, EventArgs e)
        {
            InsertOrRemoveHook();
        }

        public void OnEnabled()
        {
            IsEnabled = true;
            Log("YAR Plugin Enabled with PID: {0}", _bs.Pid);
            BotMain.OnStart += BotMain_OnStart;

            StartYarWorker();
            Send("NewDifficultyLevel", true); // Request Difficulty level
            Reset();
        }

        public void OnDisabled()
        {
            IsEnabled = false;
            Pulsator.OnPulse -= Pulsator_OnPulse;
            BotMain.OnStart -= BotMain_OnStart;
            TreeHooks.Instance.OnHooksCleared -= Instance_OnHooksCleared;

            lock (_appenderLock)
            {
                if (_appenderAdded)
                {
                    Hierarchy loggingHierarchy = (Hierarchy)LogManager.GetRepository();
                    _appenderAdded = false;
                    loggingHierarchy.Root.RemoveAppender(YARAppender);
                }
            }

            Log("YAR Plugin Disabled!");

            // Pulsefix disabled plugin
            if (_pulseFix)
            {
                _pulseFix = false;
                return; // Stop here to prevent Thread abort
            }
            try
            {
                if (_yarThread.IsAlive)
                {
                    // user disabled plugin abort Thread
                    _yarThread.Abort();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public void OnPulse()
        {
            _bs.IsInGame = ZetaDia.IsInGame;
            _bs.IsLoadingWorld = ZetaDia.IsLoadingWorld;

            if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return;

            Pulse();
        }

        private void OnProfileLoaded(object sender, object e)
        {
            Send("NewDifficultyLevel", true); // Request Difficulty level
        }

        private void StartYarWorker()
        {
            if (_yarThread == null || (_yarThread != null && !_yarThread.IsAlive))
            {
                Log("Starting YAR Thread");
                _yarThread = new Thread(YarWorker) { Name = "YARWorker", IsBackground = true };
                _yarThread.Start();
            }
        }

        /// <summary>
        /// Just to make sure our ticks are ALWAYS updated!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pulsator_OnPulse(object sender, EventArgs e)
        {
            _bs.LastPulse = DateTime.UtcNow.Ticks;

            ErrorHandling();

            if (IsEnabled)
                StartYarWorker();
        }

        public void OnShutdown()
        {
            _yarThread.Abort();
        }


        LoggingEvent[] _logBuffer;

        /// <summary>
        /// Reads through the LogMessage queue and sends updates to YAR
        /// </summary>
        private void LogWorker()
        {
            try
            {
                _bs.IsRunning = BotMain.IsRunning;

                if (BotMain.IsRunning)
                {
                    _bs.IsPaused = false;
                    _bs.LastRun = DateTime.UtcNow.Ticks;
                }

                Queue<LoggingEvent> localQueue = new Queue<LoggingEvent>();
                while (YARAppender.Messages.Any())
                {
                    LoggingEvent loggingEvent;
                    if (YARAppender.Messages.TryDequeue(out loggingEvent))
                        localQueue.Enqueue(loggingEvent);
                }


                if (_logBuffer == null)
                {
                    _logBuffer = localQueue.ToArray();
                }
                else
                {
                    lock (_logBuffer)
                    {
                        var newbuffer = _logBuffer.Concat(localQueue.ToArray()).ToArray();
                        _logBuffer = newbuffer;
                    }
                }

                // Keep Thread alive while log buffer is not empty
                while (_logBuffer != null)
                {
                    try
                    {
                        var duration = DateTime.UtcNow;
                        LoggingEvent[] buffer;
                        // Lock buffer and copy to local variable for scanning
                        lock (_logBuffer)
                        {
                            buffer = new LoggingEvent[_logBuffer.Length + 1]; // set log new local log buffer size
                            _logBuffer.CopyTo(buffer, 0); // copy to local
                            _logBuffer = null; // clear buffer
                        }


                        var count = 0; // Scan counter
                        var breakloop = false;
                        // Scan log items
                        foreach (LoggingEvent lm in buffer.Where(x => x != null))
                        {
                            string msg = lm.RenderedMessage;
                            if (yarRegex.IsMatch(msg))
                                continue;

                            count++; // add to counter
                            var m = pluginsCompiled.Match(msg);
                            if (m.Success)
                            {
                                Log("Plugins Compiled matched");
                                _allPluginsCompiled = true;
                                Send("AllCompiled"); // tell relogger about all plugin compile so the relogger can tell what to do next
                                continue;
                            }

                            // Find Start stop button click
                            if (msg.Equals("Start/Stop Button Clicked!") && !BotMain.IsRunning)
                            {
                                Send("UserStop");
                                crashExceptionCounter = 0;
                            }

                            try
                            {
                                if (!ZetaDia.IsInGame && FindStartDelay(msg)) continue; // Find new start delay
                            }
                            catch (AccessViolationException)
                            {
                                if (IsGameRunning())
                                {
                                    Send("D3Exit"); // Process has exited
                                    breakloop = true; // break out of loop
                                }
                            }
                            // Crash Tender check
                            if (ReCrashTender.Any(re => re.IsMatch(msg)))
                            {
                                Log("Crash message detected");
                                Send("D3Exit"); // Restart D3
                                breakloop = true; // break out of loop
                            }

                            if (CrashExceptionRegexes.Any(re => re.IsMatch(msg)))
                            {
                                Log("Crash Exception detected");
                                crashExceptionCounter++;
                            }
                            if (crashExceptionCounter > 1000)
                            {
                                Log("Detected 1000 unhandled bot tick exceptions, restarting everything");
                                Send("D3Exit"); // Restart D3
                            }

                            // YAR compatibility with other plugins
                            if (ReCompatibility.Any(re => re.IsMatch(msg)))
                                Send("ThirdpartyStop");
                            if (breakloop) break; // Check if we need to break out of loop
                        }
                        //if (count > 1) Log("Scanned {0} log items in {1}ms", count, DateTime.UtcNow.Subtract(duration).TotalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Log("Exception in LogWorker: {0}", ex);
            }
        }

        private static Composite _yarHook;
        private void InsertOrRemoveHook(bool forceInsert = false)
        {
            try
            {
                if (IsEnabled || forceInsert)
                {
                    if (_yarHook == null)
                        _yarHook = CreateYarHook();

                    Log("Inserting YAR Hook");
                    TreeHooks.Instance.InsertHook("OutOfgame", 0, _yarHook);
                }
                else
                {
                    if (_yarHook != null)
                    {
                        Log("Removing YAR Hook");
                        TreeHooks.Instance.RemoveHook("OutOfgame", _yarHook);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        internal Composite CreateYarHook()
        {
            return new Action(ret => Pulse());
        }

        /// <summary>
        /// This is called from TreeStart
        /// </summary>
        /// <returns></returns>
        public RunStatus Pulse()
        {
            try
            {
                // YAR Health Check
                _pulseCheck = true;
                _bs.LastPulse = DateTime.UtcNow.Ticks;

                _bs.PluginPulse = DateTime.UtcNow.Ticks;

                if (!ZetaDia.Service.IsValid || !ZetaDia.Service.Platform.IsConnected)
                {
                    ErrorHandling();

                    // We handled an error, we should 
                    return RunStatus.Failure;
                }

                if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld)
                {
                    return RunStatus.Failure;
                }

                LogWorker();

                // in-game / character data 
                _bs.IsLoadingWorld = ZetaDia.IsLoadingWorld;
                _bs.Coinage = 0;
                _bs.Experience = 0;
                try
                {
                    if (ZetaDia.Me != null && ZetaDia.Me.IsValid)
                    {
                        _bs.Coinage = ZetaDia.PlayerData.Coinage;
                        Int64 exp;
                        if (ZetaDia.Me.Level < 60)
                            exp = ZetaDia.Me.CurrentExperience;
                        else
                            exp = ZetaDia.Me.ParagonCurrentExperience;

                        _bs.Experience = exp;
                    }
                }
                catch
                {
                    Log("Exception reading Coinage", 0);
                    _bs.Coinage = -1;
                    _bs.Experience = -1;
                }

                if (ZetaDia.IsInGame)
                {
                    _bs.LastGame = DateTime.UtcNow.Ticks;
                    _bs.IsInGame = true;
                }
                else
                {
                    if (_bs.IsInGame)
                    {
                        Send("GameLeft", true);
                    }
                    _bs.IsInGame = false;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return RunStatus.Failure;
        }
        #endregion

        #region Logging Monitor
        public bool FindStartDelay(string msg)
        {
            // Waiting #.# seconds before next game...
            var m = waitingBeforeGame.Match(msg);
            if (m.Success)
            {
                Send("StartDelay " + DateTime.UtcNow.AddSeconds(double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)).Ticks);
                return true;
            }
            return false;
        }
        #endregion

        #region yarWorker
        public void YarWorker()
        {
            Log("YAR Worker Thread Started");
            while (true)
            {
                try
                {
                    if (BotMain.BotThread != null)
                        _bs.IsRunning = BotMain.BotThread.IsAlive;
                    else
                        _bs.IsRunning = false;

                    bool isInGame = false;
                    try
                    {
                        isInGame = ZetaDia.IsInGame;
                    }
                    catch { }
                    // Calculate game runs
                    if (isInGame)
                    {
                        _bs.LastGame = DateTime.UtcNow.Ticks;
                        _bs.IsInGame = true;
                    }
                    else
                    {
                        if (_bs.IsInGame)
                        {
                            Send("GameLeft", true);
                            Send("NewDifficultyLevel", true); // Request Difficulty level
                        }
                        _bs.IsInGame = false;
                    }

                    // Send stats
                    Send("XML:" + _bs.ToXmlString(), xml: true);

                    LogWorker();

                    Thread.Sleep(750);
                }
                catch (ThreadAbortException)
                {
                    Log("YAR Thread Aborted");
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
        }
        #endregion

        #region Handle Errors and strange situations

        private bool handlederror;
        private bool ErrorHandling()
        {
            bool errorHandled = false;
            if (ErrorDialog.IsVisible)
            {
                // Check if Demonbuddy found errordialog
                if (!handlederror)
                {
                    Send("CheckConnection", pause: true);
                    handlederror = true;
                    errorHandled = true;
                }
                else
                {
                    handlederror = false;
                    ErrorDialog.Click();
                    CheckForLoginScreen();
                    errorHandled = true;
                }
            }

            if (UIElementTester.isValid(_UIElement.errordialog_okbutton))
            {
                // Demonbuddy failed to find error dialog use static hash to find the OK button
                Send("CheckConnection", pause: true);
                UIElement.FromHash(_UIElement.errordialog_okbutton).Click();
                CheckForLoginScreen();
                errorHandled = true;
            }

            handlederror = false;
            if (UIElementTester.isValid(_UIElement.loginscreen_username))
            {
                // We are at loginscreen
                Send("CheckConnection", pause: true);
                errorHandled = true;
            }
            return errorHandled;
        }

        // Detect if we are booted to login screen or character selection screen
        private void CheckForLoginScreen()
        {
            var timeout = DateTime.UtcNow;
            while (DateTime.UtcNow.Subtract(timeout).TotalSeconds <= 15)
            {
                BotMain.PauseFor(TimeSpan.FromMilliseconds(600));
                if (UIElementTester.isValid(_UIElement.startresume_button))
                    break;
                if (UIElementTester.isValid(_UIElement.loginscreen_username))
                { // We are at loginscreen
                    Send("CheckConnection", pause: true);
                    break;
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region PipeClientSend
        private bool Send(string data, bool pause = false, bool xml = false, int retry = 1, int timeout = 3000)
        {

            var success = false;
            var tries = 0;
            if (_bs.Pid == 0)
                _bs.Pid = Process.GetCurrentProcess().Id;

            if (!xml)
                data = _bs.Pid + ":" + data;
            else
                data += "\nEND";

            // Pause bot
            if (pause)
            {
                _recieved = false;
                Func<bool> waitFor = Recieved;
                BotMain.PauseWhile(waitFor, 0, TimeSpan.FromMilliseconds((retry * timeout) + 3000));
            }
            while (!success && tries < retry)
            {
                try
                {
                    tries++;
                    using (var client = new NamedPipeClientStream(".", "YetAnotherRelogger"))
                    {
                        client.Connect(timeout);
                        if (client.IsConnected)
                        {
                            var streamWriter = new StreamWriter(client) { AutoFlush = true };
                            var streamReader = new StreamReader(client);

                            streamWriter.WriteLine(data);

                            var connectionTime = DateTime.UtcNow;

                            if (!client.IsConnected)
                            {
                                Log("Error: client disconnected before response received");
                            }

                            while (!success && client.IsConnected)
                            {
                                if (DateTime.UtcNow.Subtract(connectionTime).TotalSeconds > 3)
                                {
                                    client.Close();
                                    break;
                                }

                                string responseText = string.Empty;
                                if (!streamReader.EndOfStream)
                                {
                                    responseText = streamReader.ReadLine();
                                }
                                if (string.IsNullOrWhiteSpace(responseText))
                                {
                                    Thread.Sleep(10);
                                    continue;
                                }

                                HandleResponse(responseText);
                                success = true;

                            }

                        }
                        else
                        {
                            // Failed to connect
                        }
                    }
                }
                catch (ThreadAbortException) { }
                catch (TimeoutException)
                {
                    if (this.IsEnabled)
                    {
                        // YAR is not running, disable the plugin
                        //Log("TimeoutException - Disabling YAR Plugin");

                        PluginManager.Plugins.Where(p => p.Plugin.Name == this.Name).All(p => p.Enabled = false);
                        _yarThread.Abort();
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    OnShutdown();
                }

            }
            _recieved = true;
            return success;
        }
        #endregion

        #region HandleResponse
        void HandleResponse(string data)
        {
            string cmd = data.Split(' ')[0];
            if (data.Split(' ').Count() > 1)
                data = data.Substring(cmd.Length + 1);
            switch (cmd)
            {
                case "Restart":
                    Log("Restarting bot");
                    try
                    {
                        Log("Stopping Bot");
                        BotMain.Stop();
                        Application.Current.Dispatcher.BeginInvoke((System.Action)(() =>
                        {
                            try
                            {
                                Log("Starting Bot");
                                Thread.Sleep(1000);
                                BotMain.Start();
                            }
                            catch (Exception ex)
                            {
                                LogException(ex);
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                    Reset();
                    break;
                case "LoadProfile":
                    LoadProfile(data);
                    break;
                case "DifficultyLevel":
                    var difficulty_level = Convert.ToInt32(data.Trim());
                    if (difficulty_level >= 0)
                    {
                        var difficulty = (GameDifficulty)System.Enum.Parse(typeof(GameDifficulty), data.Trim(), true);
                        Log("Recieved DifficultyLevel: {0}", difficulty);
                        CharacterSettings.Instance.GameDifficulty = difficulty;
                    }
                    break;
                case "ForceEnableAll":
                    ForceEnableAllPlugins();
                    break;
                case "ForceEnableYar":
                    ForceEnableYar();
                    break;
                case "FixPulse":
                    FixPulse();
                    break;
                case "Shutdown":
                    Log("Received Shutdown command");
                    SafeCloseProcess();
                    break;
                case "Roger!":
                case "Unknown command!":
                    break;
                default:
                    Log("Unknown response! \"{0} {1}\"", cmd, data);
                    break;
            }
            _recieved = true;
        }

        // from Nesox
        private void SafeCloseProcess()
        {
            Log("Attempting to Safely Close Process");
            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(new System.Action(SafeCloseProcess));
                    return;
                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        #region ForceEnable Plugin(s)
        private void ForceEnableYar()
        {
            // Check if plugin is enabled
            var plugin = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name.Equals(Name));
            if (plugin == null || (plugin.Enabled)) return;

            Log("Force enable plugin");
            var plugins = PluginManager.GetEnabledPlugins().ToList();
            plugins.Add(Name);
            PluginManager.SetEnabledPlugins(plugins.ToArray());
        }

        private void ForceEnableAllPlugins()
        {
            PluginContainer test;
            DateTime limit;

            var disabledPlugins = PluginManager.Plugins.Where(p => !p.Enabled && p.Plugin.Name != "BuddyMonitor").ToList();
            if (!disabledPlugins.Any())
                return;

            Log("Disabled plugins found. User requested all plugins be enabled through YAR. Enabling Plugins..");

            foreach (var plugin in disabledPlugins)
            {
                try
                {
                    Log("Force enable: \"{0}\"", plugin.Plugin.Name);
                    plugin.Enabled = true;
                    limit = DateTime.UtcNow;
                    while ((test = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name.Equals(plugin.Plugin.Name))) != null && !test.Enabled)
                    {
                        if (DateTime.UtcNow.Subtract(limit).TotalSeconds > 5)
                        {
                            Log("Failed to enable: Timeout ({0} seconds) \"{1}\"", DateTime.UtcNow.Subtract(limit).TotalSeconds, plugin.Plugin.Name);
                            break;
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Log("Failed to enable: \"{0}\"", plugin.Plugin.Name);
                    LogException(ex);
                }
            }
        }
        #endregion

        #region FixPulse

        private bool _pulseCheck;
        private void FixPulse()
        {
            DateTime timeout;
            Log("############## Pulse Fix ##############");
            // Check if plugin is enabled
            var plugin = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name.Equals(Name));
            if (plugin != null && plugin.Enabled)
            {
                Log("PulseFix: Plugin is already enabled -> Disable it for now");
                _pulseFix = true; // Prevent our thread from begin aborted
                plugin.Enabled = false;
                timeout = DateTime.UtcNow;
                while (plugin.Enabled)
                {
                    if (DateTime.UtcNow.Subtract(timeout).TotalSeconds > 10)
                    {
                        Log("PulseFix: Failed to disable plugin");
                        Application.Current.Shutdown(0);
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
            else
                Log("PulseFix: Plugin is not enabled!");

            // Force enable yar plugin
            ForceEnableYar();

            var attempt = 0;
            while (!BotMain.IsRunning)
            {
                attempt++;
                if (attempt >= 4)
                {
                    Log("PulseFix: Fix attempts failed, closing demonbuddy!");
                    Application.Current.Shutdown();
                }
                if (BotMain.BotThread == null)
                {
                    Log("PulseFix: Mainbot thread is not running");
                    Log("PulseFix: Force start bot");
                    BotMain.Start();
                }
                else if (BotMain.BotThread != null)
                {
                    if (BotMain.IsPaused || BotMain.IsPausedForStateExecution)
                        Log("PulseFix: DB is Paused!");
                    Log("PulseFix: Force stop bot");
                    BotMain.BotThread.Abort();
                    Thread.Sleep(1000);
                    Log("PulseFix: Force start bot");
                    BotMain.Start();
                }
                Thread.Sleep(1000);
            }

            // Check if we get a pulse within 10 seconds
            Log("PulseFix: Waiting for first pulse");
            _pulseCheck = false;
            timeout = DateTime.UtcNow;
            while (!_pulseCheck)
            {
                if (DateTime.UtcNow.Subtract(timeout).TotalSeconds > 10)
                {
                    Log("PulseFix: Failed to recieve a pulse within 10 seconds");
                    SafeCloseProcess();
                    break;
                }
                Thread.Sleep(100);
            }
            Log("############## End Fix ##############");
        }
        #endregion

        bool _recieved;
        bool Recieved()
        {
            return _recieved;
        }
        bool IsGameRunning()
        {
            return ZetaDia.Memory.Process.HasExited && !Process.GetProcesses().Any(p => p.ProcessName.StartsWith("BlizzardError") && DateTime.UtcNow.Subtract(p.StartTime).TotalSeconds <= 30);
        }
        #endregion

        void Reset()
        {
            _bs.LastPulse = DateTime.UtcNow.Ticks;
            _bs.LastRun = DateTime.UtcNow.Ticks;
            _bs.LastGame = DateTime.UtcNow.Ticks;
        }

        private string ParseInnerProfile(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var xml = XDocument.Load(path);

            return ProfileUtils.GetProfileAttribute("LoadProfile", "profile", xml.Root);                        
        }

        private void LoadProfile(string profile)
        {
            var isHardReset = ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Service.Party.CurrentPartyLockReasonFlags != PartyLockReasonFlag.None;

            if (isHardReset)
            {
                BotMain.Stop(false, "-> Hard Stop/Reset and Load new profile");
                if (ZetaDia.IsInGame)
                {
                    ZetaDia.Service.Party.LeaveGame(true);
                    while (ZetaDia.IsInGame)
                        Thread.Sleep(1000);
                }
            }

            //profile = ParseInnerProfile(profile);

            if (isHardReset)
            {
                Thread.Sleep(2000);
                Log("Loading profile: {0}", profile);
            }

            ProfileManager.Load(profile.Trim());

            if (isHardReset)
            {
                Thread.Sleep(5000);
                BotMain.Start();
            }

        }
    }

    #region ElementTester
    public static class _UIElement
    {
        public static ulong leavegame_cancel = 0x3B55BA1E41247F50,
        loginscreen_username = 0xDE8625FCCFFDFC28,
        loginscreen_password = 0xBA2D3316B4BB4104,
        loginscreen_loginbutton = 0x50893593B5DB22A9,
        startresume_button = 0x51A3923949DC80B7,
        errordialog_okbutton = 0xB4433DA3F648A992;
    }
    public static class UIElementTester
    {

        /// <summary>
        /// UIElement validation check
        /// </summary>
        /// <param name="hash">UIElement hash to check</param>
        /// <param name="isEnabled">should be enabled</param>
        /// <param name="isVisible">should be visible</param>
        /// <param name="bisValid">should be a valid UIElement</param>
        /// <returns>true if all requirements are valid</returns>
        public static bool isValid(ulong hash, bool isEnabled = true, bool isVisible = true, bool bisValid = true)
        {
            try
            {
                if (!UIElement.IsValidElement(hash))
                    return false;
                else
                {
                    var element = UIElement.FromHash(hash);

                    if ((isEnabled && !element.IsEnabled) || (!isEnabled && element.IsEnabled))
                        return false;
                    if ((isVisible && !element.IsVisible) || (!isVisible && element.IsVisible))
                        return false;
                    if ((bisValid && !element.IsValid) || (!bisValid && element.IsValid))
                        return false;

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region XmlTools
    public static class XmlTools
    {
        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }
        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }
    #endregion

    public class YARAppender : AppenderSkeleton
    {
        public static ConcurrentQueue<LoggingEvent> Messages = new ConcurrentQueue<LoggingEvent>();

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (Messages)
            {
                Messages.Enqueue(loggingEvent);
            }
        }
    }

}

#region Trinity Support
namespace YARPLUGIN
{
    public static class TrinitySupport
    {
        private static bool _failed;
        private static Type _gilesTrinityType;
        public static bool Initialized { get; private set; }

        public static void Initialize()
        {
            Initialized = true;
            YARPLUGIN.Log("Initialize Trinity Support");
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.ToLower().StartsWith("trinity"));
            if (asm != null)
            {
                try
                {
                    _gilesTrinityType = asm.GetType("GilesTrinity.GilesTrinity");
                    _failed = false;
                    return;
                }
                catch (Exception ex)
                {
                    YARPLUGIN.Log("Failed to initialize Trinity Support:");
                    YARPLUGIN.LogException(ex);
                }
            }
            else
            {
                YARPLUGIN.Log("Trinity is not installed");
            }
            _failed = true;
        }

        public static bool IsEnabled
        {
            get
            {
                var plugin = PluginManager.Plugins.FirstOrDefault(p => p.Plugin.Name.Equals("Trinity"));
                return (plugin != null && plugin.Enabled);
            }
        }

        private static bool bDontMoveMeIAmDoingShit
        {
            get
            {
                try
                {
                    return (bool)_gilesTrinityType.GetField("bDontMoveMeIAmDoingShit", BindingFlags.Static).GetValue(null);
                }
                catch (Exception ex)
                {
                    YARPLUGIN.Log("Failed to get Trinity info:");
                    YARPLUGIN.LogException(ex);
                    return false;
                }
            }
        }
        private static bool MainBotPaused
        {
            get
            {
                try
                {
                    return (bool)_gilesTrinityType.GetField("bMainBotPaused", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                }
                catch (Exception ex)
                {
                    YARPLUGIN.Log("Failed to get Trinity info:");
                    YARPLUGIN.LogException(ex);
                    return false;
                }
            }
        }
        public static bool IsPaused
        {
            get
            {
                if (!Initialized) Initialize();
                return !_failed && MainBotPaused;
            }
        }
        public static bool IsBusy
        {
            get
            {
                if (!Initialized) Initialize();
                return !_failed && bDontMoveMeIAmDoingShit;
            }
        }
    }
}
#endregion