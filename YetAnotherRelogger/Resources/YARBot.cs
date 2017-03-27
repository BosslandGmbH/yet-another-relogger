using System;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace YARKickstart
{
    public class YARKickstart : IBot
    {
        public string Name => "YARKickstart";

        public static bool IsKickstarted;

        private static readonly log4net.ILog Logger = Zeta.Common.Logger.GetLoggerInstanceForType();

        private const string YarKickstartProfile = @"

            <Profile>
              <Name>YAR Kickstart</Name>
              <KillMonsters>True</KillMonsters>
              <PickupLoot>True</PickupLoot> 
              <Order></Order>
            </Profile>";

        public YARKickstart()
        {
            if (IsKickstarted)
                return;

            Logger.Info("YARBot Initialized");

            //PluginManager.OnPluginsReloaded += OnPluginsLoaded_WrapPlugins;

            // No need to do anything if DB is already logged in. 
            if (IsLoggedIn)
                return;
         
            IsKickstarted = true;

            var currentProfile = ProfileManager.CurrentProfile;
            if (currentProfile == null)
            {
                // Make DB not throw its toys when started with cmd line args without -profile
                var xmlFile = XDocument.Parse(YarKickstartProfile);
                ProfileManager.CurrentProfile = Profile.Load(xmlFile.Root);

                // Make OrderBot not throw its toys when it tries to load a profile that needs plugins that haven't compiled yet.
                var path = Path.Combine(GlobalSettings.Instance.BotsPath, "YARBot", "kickstart.xml");
                xmlFile.Save(path);
                GlobalSettings.Instance.LastProfile = path;
            }

            Task.Run(() => KillAfterLogin());
        }

        public async Task<bool> KillAfterLogin()
        {
            while (!IsLoggedIn)
            {
                await Task.Delay(250);                
            }
            ExitDemonBuddy();
            return true;
        }

        public bool IsLoggedIn
        {
            get
            {
                using (ZetaDia.Memory.AcquireFrame())
                {
                    return ZetaDia.Service.IsValid && ZetaDia.Service.Hero.IsValid;
                }
            }
        }

        internal static void ExitDemonBuddy()
        {
            Logger.Info("YARBot Ending DemonBuddy Process");
            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(ExitDemonBuddy);
                    return;
                }
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        //private static void OnPluginsLoaded_WrapPlugins(object sender, EventArgs eventArgs)
        //{
        //    // this event fires multiple times during startup
        //    // wrap plugins so that OnInitialized and OnEnabled fire from the correct thread (1: UI/Dispatcher).

        //    var containers = PluginManager.Plugins.ToArray();
        //    for (var i = 0; i < containers.Length; i++)
        //    {
        //        var container = containers[i];
        //        var yarContainer = container.Plugin as YARPluginWrapper;
        //        if (yarContainer == null)
        //        {
        //            container.Plugin = new YARPluginWrapper(container.Plugin);
        //            container.Plugin.OnInitialize();
        //        }
        //        else if (!yarContainer.IsValid)
        //        {
        //            // DB thinks the empty wrapper is a real plugin, remove it.
        //            PluginManager.Plugins.Remove(container);
        //        }
        //    }
        //}

        ///// <summary>
        ///// A wrapper for plugins that routes some methods through the application dispatcher.
        ///// </summary>
        //public class YARPluginWrapper : IPlugin
        //{
        //    private readonly IPlugin _plugin;
        //    public YARPluginWrapper() { }
        //    public YARPluginWrapper(IPlugin plugin) { _plugin = plugin; }
        //    public bool Equals(IPlugin other) => !IsValid || _plugin.Equals(other);
        //    public void OnPulse() => _plugin?.OnPulse();
        //    public void OnInitialize() => Application.Current.Dispatcher.Invoke(() => _plugin?.OnInitialize());
        //    public void OnShutdown() => Application.Current.Dispatcher.Invoke(() => _plugin?.OnShutdown());
        //    public void OnEnabled() => Application.Current.Dispatcher.Invoke(() => _plugin?.OnEnabled());
        //    public void OnDisabled() => Application.Current.Dispatcher.Invoke(() => _plugin?.OnDisabled());
        //    public string Author => _plugin?.Author;
        //    public Version Version => _plugin?.Version;
        //    public string Name => _plugin?.Name;
        //    public string Description => _plugin?.Description;
        //    public Window DisplayWindow => _plugin?.DisplayWindow;
        //    public bool IsValid => _plugin != null;
        //}

        public bool TryGetBotProfile(string path, out Profile profile)
        {
            profile = null;
            return false;
        }

        public void Dispose() { }
        public void Start() { }
        public void Stop() { }
        public void Pulse() { }
        public void Initialize() { }
        public Composite Logic { get { return new Action(ret => RunStatus.Failure); } }
    }

}