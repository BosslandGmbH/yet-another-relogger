using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace YetAnotherRelogger.Plugin
{
    public static class NetworkTools
    {
        private static readonly Random s_random = new Random();
        public static int GetFreeUdpPort(int start, int end)
        {
            var activeListeners = new HashSet<int>(System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveUdpListeners().Select(u => u.Port));

            int port;
            do
            {
                port = s_random.Next(start, end);
            } while (activeListeners.Contains(port));

            return port;
        }
    }

    public enum BotCommand : byte
    {
        Null = 0,
        Ack = 1,
        Restart = 2,
        Shutdown = 3,
        FixPulse = 4,
        ForceEnableAll = 5,
        ForceEnableYar = 6,
        LoadProfile = 7,
        SwitchDifficultyLevel = 8
    }

    public enum ControlRequest
    {
        Null = 0,
        GameLeft = 1,
        NewDifficultyLevel = 2,
        CheckConnection = 3,
        RequestProfile = 4,
        Initialized = 5,
    }

    public class Plugin : IPlugin
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        
        #region IPlugin implementation
        public string Author => "rrrix and sinterlkaas";
        public Version Version { get; } = new Version(typeof(Plugin).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
        public string Name { get; } = typeof(Plugin).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        public string Description { get; } = typeof(Plugin).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        public Window DisplayWindow => null;

        /// <summary> Executes the pulse action. This is called every "tick" of the bot. </summary>
        public void OnPulse()
        {
            Pulse();
        }

        /// <summary> Executes the initialize action. This is called at initial bot startup. (When the bot itself is started, not when Start() is called) </summary>
        public void OnInitialize()
        {
            ForceEnableYar();
        }

        /// <summary> Executes the shutdown action. This is called when the bot is shutting down. (Not when Stop() is called) </summary>
        public void OnShutdown()
        {
            StopListening();
            _yarThread.Abort();
        }

        /// <summary> Executes the enabled action. This is called when the user has enabled this specific plugin via the GUI. </summary>
        public void OnEnabled()
        {
            // YAR Login Support (YARKickstart IBot will call this from the proper thread)
            if (!Application.Current.CheckAccess()) return;

            IsEnabled = true;

            var port = StartListening();
            s_logger.ForContext("Port", port).Information("YAR Plugin Enabled with PID: {PID} listening to {Port}");

            // Setup events before we actually do anything.
            BotMain.OnStart += OnStart;
            ProfileManager.OnProfileLoaded += OnProfileLoaded;
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;
            
            StartYarWorker();

            s_logger.Information("Requesting Profile (Current={Profile})", ProfileManager.CurrentProfile != null ? ProfileManager.CurrentProfile.Path : "Null");
            Send(ControlRequest.RequestProfile);

            Send(ControlRequest.NewDifficultyLevel); // Request Difficulty level
            Reset();

            Send(ControlRequest.Initialized);
        }

        /// <summary> Executes the disabled action. This is called whent he user has disabled this specific plugin via the GUI. </summary>
        public void OnDisabled()
        {
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
                s_logger.Warning(ex, "Exception while disabling");
            }

            // Clear events no longer need them as we are about to be disabled.
            TreeHooks.Instance.OnHooksCleared -= OnHooksCleared;
            ProfileManager.OnProfileLoaded -= OnProfileLoaded;
            BotMain.OnStart -= OnStart;

            StopListening();

            s_logger.Information("YAR Plugin Disabled!");

            IsEnabled = false;
        }
        #endregion

        #region BotEvents
        private void OnStart(IBot bot)
        {
            InsertOrRemoveOutOfGameHook();
        }

        private void OnHooksCleared(object sender, EventArgs e)
        {
            InsertOrRemoveOutOfGameHook();
        }

        private void OnProfileLoaded(object sender, object e)
        {
            Send(ControlRequest.NewDifficultyLevel); // Request Difficulty level
        }

        /// <summary>
        /// This is called from TreeStart
        /// </summary>
        /// <returns></returns>
        public RunStatus Pulse()
        {
            _bs.LastPulse = DateTime.UtcNow.Ticks;
            _bs.IsInGame = ZetaDia.IsInGame;
            _bs.IsLoadingWorld = ZetaDia.Globals.IsLoadingWorld;

            if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return RunStatus.Failure;

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

                if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.Globals.IsLoadingWorld)
                {
                    return RunStatus.Failure;
                }

                // in-game / character data 
                _bs.IsLoadingWorld = ZetaDia.Globals.IsLoadingWorld;
                _bs.Coinage = 0;
                _bs.Experience = 0;
                try
                {
                    if (ZetaDia.Me != null && ZetaDia.Me.IsValid)
                    {
                        _bs.Coinage = ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage;
                        var exp = ZetaDia.Me.Level < 70 ? ZetaDia.Me.CurrentExperience : ZetaDia.Me.ParagonCurrentExperience;

                        _bs.Experience = exp;
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Warning(ex, "Exception reading coinage");
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
                        Send(ControlRequest.GameLeft);
                    }
                    _bs.IsInGame = false;
                }
            }
            catch (Exception ex)
            {
                s_logger.Warning(ex, "Pulse failed.");
            }
            return RunStatus.Failure;
        }
        #endregion

        #region Command and Control Server
        private bool _acknowledged;
        private UdpClient _listener;

        private int StartListening()
        {
            var port = NetworkTools.GetFreeUdpPort(56000, 57000);
            _listener = new UdpClient(port);
            Listen();
            return port;
        }

        private void StopListening()
        {
            _listener.Dispose();
            _listener = null;
        }
        
        private async void Listen()
        {
            var resp = await _listener.ReceiveAsync();
            PacketReceived(resp);
            if (IsEnabled)
                Listen();
        }

        private async void PacketReceived(UdpReceiveResult packet)
        {
            var cmd = (BotCommand)packet.Buffer[0];
            
            switch (cmd)
            {
                case BotCommand.Ack:
                    s_logger.Information("Acknowledged");
                    _acknowledged = true;
                    break;
                case BotCommand.Restart:
                    s_logger.Information("Restarting bot");
                    try
                    {
                        s_logger.Information("Stopping Bot");
                        BotMain.Stop();
                        await Application.Current.Dispatcher.BeginInvoke((System.Action)(() =>
                        {
                            try
                            {
                                s_logger.Information("Starting Bot");
                                Thread.Sleep(1000);
                                BotMain.Start();
                            }
                            catch (Exception ex)
                            {
                                s_logger.Warning(ex, "Error during start");
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        s_logger.Warning(ex, "Error during restart");
                    }
                    Reset();
                    break;
                case BotCommand.Shutdown:
                    s_logger.Information("Received Shutdown command");
                    SafeCloseProcess();
                    break;
                case BotCommand.ForceEnableAll:
                    ForceEnableAllPlugins();
                    break;
                case BotCommand.ForceEnableYar:
                    ForceEnableYar();
                    break;
                case BotCommand.FixPulse:
                    FixPulse();
                    break;
                case BotCommand.LoadProfile:
                    var profile = Encoding.UTF8.GetString(packet.Buffer.Skip(1).ToArray());
                    s_logger.Information("Received {profile}", profile);
                    LoadProfile(profile);
                    break;
                case BotCommand.SwitchDifficultyLevel:
                    var difficultyLevel = (GameDifficulty)BitConverter.ToInt32(packet.Buffer, 1);
                    if (difficultyLevel >= 0)
                    {
                        s_logger.Information("Recieved DifficultyLevel: {difficultyLevel}", difficultyLevel);
                        CharacterSettings.Instance.GameDifficulty = difficultyLevel;
                    }
                    break;
                default:
                    s_logger.Warning("Unknown command {cmd}", cmd);
                    break;
            }
        }
        
        private void Send(ControlRequest notification, int timeout = 3000)
        {
            _acknowledged = false;
            // Pause bot until the time out is hit.
            BotMain.PauseWhile(() => _acknowledged, 0, TimeSpan.FromMilliseconds(timeout));
            s_logger.Information("Control Request: {notification}", notification);
            while (BotMain.IsPaused)
                Thread.Yield();
        }
        #endregion

        #region Command Handlers
        private void SafeCloseProcess()
        {
            s_logger.Warning("Attempting to safely close process");
            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(SafeCloseProcess);
                    return;
                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                s_logger.Warning(ex, "Error during safely close");
            }
        }

        private void ForceEnableAllPlugins()
        {
            PluginContainer test;
            DateTime limit;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var disabledPlugins = PluginManager.Plugins.Where(p => !p.Enabled && p.Plugin.Name != "BuddyMonitor").ToList();
                if (!disabledPlugins.Any())
                    return;

                s_logger.Information("Disabled plugins found. User requested all plugins be enabled through YAR. Enabling Plugins..");

                foreach (PluginContainer plugin in disabledPlugins)
                {
                    try
                    {
                        s_logger.Information("Force enable: {Name}", plugin.Plugin.Name);
                        plugin.Enabled = true;
                        limit = DateTime.UtcNow;
                        while ((test = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name.Equals(plugin.Plugin.Name))) != null && !test.Enabled)
                        {
                            if (DateTime.UtcNow.Subtract(limit).TotalSeconds > 5)
                            {
                                s_logger.Warning("Failed to enable: Timeout ({TotalSeconds} seconds) {Name}", DateTime.UtcNow.Subtract(limit).TotalSeconds, plugin.Plugin.Name);
                                break;
                            }
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        s_logger.Warning(ex, "Failed to enable: \"{0}\"", plugin.Plugin.Name);
                    }
                }

            });
        }

        private void ForceEnableYar()
        {
            // Force enable YAR
            var enabledPluginsList = PluginManager.Plugins.Where(p => p.Enabled).Select(p => p.Plugin.Name).ToList();
            if (!enabledPluginsList.Contains(Name))
                enabledPluginsList.Add(Name);

            s_logger.Information("Force enable YetAnotherRelogger.Plugin");
            // This call triggers OnEnabled.
            PluginManager.SetEnabledPlugins(enabledPluginsList.ToArray());
        }

        #region FixPulse
        private bool _pulseCheck;
        private void FixPulse()
        {
            DateTime timeout;
            s_logger.Information("############## Pulse Fix ##############");
            // Check if plugin is enabled
            PluginContainer plugin = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name.Equals(Name));
            if (plugin != null && plugin.Enabled)
            {
                s_logger.Information("PulseFix: Plugin is already enabled -> Disable it for now");
                _pulseFix = true; // Prevent our thread from begin aborted
                plugin.Enabled = false;
                timeout = DateTime.UtcNow;
                while (plugin.Enabled)
                {
                    if (DateTime.UtcNow.Subtract(timeout).TotalSeconds > 10)
                    {
                        s_logger.Information("PulseFix: Failed to disable plugin");
                        Application.Current.Shutdown(0);
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
            else
                s_logger.Information("PulseFix: Plugin is not enabled!");

            // Force enable yar plugin
            ForceEnableYar();

            var attempt = 0;
            while (!BotMain.IsRunning)
            {
                attempt++;
                if (attempt >= 4)
                {
                    s_logger.Information("PulseFix: Fix attempts failed, closing demonbuddy!");
                    Application.Current.Shutdown();
                }
                if (BotMain.BotThread == null)
                {
                    s_logger.Information("PulseFix: Mainbot thread is not running");
                    s_logger.Information("PulseFix: Force start bot");
                    BotMain.Start();
                }
                else if (BotMain.BotThread != null)
                {
                    if (BotMain.IsPaused || BotMain.IsPausedForStateExecution)
                        s_logger.Information("PulseFix: DB is Paused!");
                    s_logger.Information("PulseFix: Force stop bot");
                    BotMain.BotThread.Abort();
                    Thread.Sleep(1000);
                    s_logger.Information("PulseFix: Force start bot");
                    BotMain.Start();
                }
                Thread.Sleep(1000);
            }

            // Check if we get a pulse within 10 seconds
            s_logger.Information("PulseFix: Waiting for first pulse");
            _pulseCheck = false;
            timeout = DateTime.UtcNow;
            while (!_pulseCheck)
            {
                if (DateTime.UtcNow.Subtract(timeout).TotalSeconds > 10)
                {
                    s_logger.Information("PulseFix: Failed to recieve a pulse within 10 seconds");
                    SafeCloseProcess();
                    break;
                }
                Thread.Sleep(100);
            }
            s_logger.Information("############## End Fix ##############");
        }
        #endregion

        private void LoadProfile(string profile)
        {
            s_logger.Information("Loading profile: {profile}", profile);

            if (ProfileManager.CurrentProfile == null || profile != ProfileManager.CurrentProfile.Path)
                ProfileManager.Load(profile.Trim());
        }
        #endregion

        private readonly BotStats _bs = new BotStats(Process.GetCurrentProcess().Id);
        private Thread _yarThread;
        private bool _pulseFix;

        public bool IsEnabled { get; set; }

        private void Reset()
        {
            _bs.LastPulse = DateTime.UtcNow.Ticks;
            _bs.LastRun = DateTime.UtcNow.Ticks;
            _bs.LastGame = DateTime.UtcNow.Ticks;
        }

        public bool Equals(IPlugin other)
        {
            return (other?.Name == Name) && (other?.Version == Version);
        }

        #region Out of game behavior
        private static Composite _yarHook;
        // This makes sure that our hook is in place during out of game ticks.
        private void InsertOrRemoveOutOfGameHook(bool forceInsert = false)
        {
            try
            {
                if (IsEnabled || forceInsert)
                {
                    if (_yarHook == null)
                        _yarHook = CreateYarHook();

                    s_logger.Information("Inserting YAR Hook");
                    TreeHooks.Instance.InsertHook("OutOfgame", 0, _yarHook);
                }
                else
                {
                    if (_yarHook == null)
                        return;

                    s_logger.Information("Removing YAR Hook");
                    TreeHooks.Instance.RemoveHook("OutOfgame", _yarHook);
                }
            }
            catch (Exception ex)
            {
                s_logger.Warning(ex, "Failed to manipulate tree hooks.");
            }
        }

        internal Composite CreateYarHook()
        {
            return new Action(ret => Pulse());
        }
        #endregion

        #region yarWorker
        private void StartYarWorker()
        {
            if (_yarThread == null || (_yarThread != null && !_yarThread.IsAlive))
            {
                s_logger.Information("Starting YAR Thread");
                _yarThread = new Thread(YarWorker) { Name = "YARWorker", IsBackground = true };
                _yarThread.Start();
            }
        }

        public void YarWorker()
        {
            s_logger.Information("YAR Worker Thread Started");
            while (true)
            {
                try
                {
                    _bs.IsRunning = BotMain.BotThread != null && BotMain.BotThread.IsAlive;

                    var isInGame = false;
                    try
                    {
                        isInGame = ZetaDia.IsInGame;
                    }
                    catch (Exception ex)
                    {
                        s_logger.Warning(ex, "Exception accessing IsInGame");
                    }
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
                            Send(ControlRequest.GameLeft);
                            Send(ControlRequest.NewDifficultyLevel);
                        }
                        _bs.IsInGame = false;
                    }

                    // Send stats
                    s_logger.ForContext("BotStats", _bs).Verbose("Statistics report");
                    
                    Thread.Sleep(750);
                }
                catch (ThreadAbortException ex)
                {
                    s_logger.Information(ex, "YAR Thread Aborted");
                    // End the thread...
                    return;
                }
                catch (Exception ex)
                {
                    s_logger.Warning(ex, "Exception during YAR Thread");
                    // End the thread...
                    return;
                }
            }
        }
        #endregion

        #region Handle Errors and strange situations
        private bool _handlederror;
        private void ErrorHandling()
        {
            if (ErrorDialog.IsVisible)
            {
                // Check if Demonbuddy found errordialog
                if (!_handlederror)
                {
                    Send(ControlRequest.CheckConnection);
                    _handlederror = true;
                }
                else
                {
                    _handlederror = false;
                    ErrorDialog.Click();
                    CheckForLoginScreen();
                }
            }

            if (UIElementTester.IsValid(UIElement.ErrorDialogOkButton))
            {
                // Demonbuddy failed to find error dialog use static hash to find the OK button
                Send(ControlRequest.CheckConnection);
                Zeta.Game.Internals.UIElement.FromHash(UIElement.ErrorDialogOkButton).Click();
                CheckForLoginScreen();
            }

            _handlederror = false;
            if (UIElementTester.IsValid(UIElement.LoginScreenUsername))
            {
                // We are at loginscreen
                Send(ControlRequest.CheckConnection);
            }
        }

        // Detect if we are booted to login screen or character selection screen
        private void CheckForLoginScreen()
        {
            DateTime timeout = DateTime.UtcNow;
            while (DateTime.UtcNow.Subtract(timeout).TotalSeconds <= 15)
            {
                BotMain.PauseFor(TimeSpan.FromMilliseconds(600));
                if (UIElementTester.IsValid(UIElement.StartResumeButton))
                    break;
                if (UIElementTester.IsValid(UIElement.LoginScreenUsername))
                {
                    // We are at loginscreen
                    Send(ControlRequest.CheckConnection);
                    break;
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region nested: BotStats
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

            public BotStats(int pid)
            {
                Pid = pid;
                LastPulse = DateTime.UtcNow.Ticks;
            }

            public override string ToString()
            {
                return
                    $"Pid={Pid} LastRun={LastRun} LastPulse={LastPulse} PluginPulse={PluginPulse} LastGame={LastGame} IsPaused={IsPaused} IsRunning={IsRunning} IsInGame={IsInGame} IsLoadingWorld={IsLoadingWorld} Coinage={Coinage} Experience={Experience}";
            }
        }
        #endregion
    }

    public class ProfileUtils
    {
        public static XElement GetProfileTag(string tagName, XElement element = null)
        {
            if (element == null)
            {
                Zeta.Bot.Profile.Profile profile = ProfileManager.CurrentProfile;
                if (profile?.Element != null)
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

        public static string GetProfileAttribute(string tagName, string attrName, XElement element = null)
        {
            XElement profileTag = GetProfileTag(tagName, element);
            XAttribute behaviorAttr = profileTag?.Attribute(attrName);

            if (behaviorAttr != null && !string.IsNullOrEmpty(behaviorAttr.Value))
            {
                return behaviorAttr.Value;
            }
            return string.Empty;
        }
    }

    #region ElementTester
    public static class UIElement
    {
        public static ulong LoginScreenUsername = 0xDE8625FCCFFDFC28,
            StartResumeButton = 0x51A3923949DC80B7,
            ErrorDialogOkButton = 0xB4433DA3F648A992;
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
        public static bool IsValid(ulong hash, bool isEnabled = true, bool isVisible = true, bool bisValid = true)
        {
            try
            {
                if (!Zeta.Game.Internals.UIElement.IsValidElement(hash))
                    return false;

                var element = Zeta.Game.Internals.UIElement.FromHash(hash);

                if ((isEnabled && !element.IsEnabled) || (!isEnabled && element.IsEnabled))
                    return false;
                if ((isVisible && !element.IsVisible) || (!isVisible && element.IsVisible))
                    return false;
                if ((bisValid && !element.IsValid) || (!bisValid && element.IsValid))
                    return false;
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
        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }
    #endregion
}

#region Trinity Support
namespace YARPlugin
{
    public static class TrinitySupport
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();

        private static bool _failed;
        private static Type _gilesTrinityType;
        public static bool Initialized { get; private set; }

        public static void Initialize()
        {
            Initialized = true;
            s_logger.Information("Initialize Trinity Support");
            Assembly asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.ToLower().StartsWith("trinity"));
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
                    s_logger.Warning(ex, "Failed to initialize Trinity Support.");
                }
            }
            else
            {
                s_logger.Warning("Trinity is not installed");
            }
            _failed = true;
        }

        public static bool IsEnabled
        {
            get
            {
                PluginContainer plugin = PluginManager.Plugins.FirstOrDefault(p => p.Plugin.Name.Equals("Trinity"));
                return (plugin != null && plugin.Enabled);
            }
        }

        private static bool DontMoveMeIAmDoingShit
        {
            get
            {
                try
                {
                    return (bool)(_gilesTrinityType?.GetField("bDontMoveMeIAmDoingShit", BindingFlags.Static)?.GetValue(null) ?? false);
                }
                catch (Exception ex)
                {
                    s_logger.Warning(ex, "Failed to get Trinity info");
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
                    return (bool)(_gilesTrinityType?.GetField("bMainBotPaused", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? false);
                }
                catch (Exception ex)
                {
                    s_logger.Warning(ex, "Failed to get Trinity info");
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
                return !_failed && DontMoveMeIAmDoingShit;
            }
        }
    }
}
#endregion
