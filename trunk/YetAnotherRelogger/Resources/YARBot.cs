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
        private static readonly log4net.ILog Logger = Zeta.Common.Logger.GetLoggerInstanceForType();

        private const string YarKickstart = @"

            <Profile>
              <Name>YAR Kickstart</Name>
              <KillMonsters>True</KillMonsters>
              <PickupLoot>True</PickupLoot> 
              <Order></Order>
            </Profile>";

        public static bool IsKickstarted;

        public YARKickstart()
        {
            if (IsKickstarted)
                return;

            using (ZetaDia.Memory.AcquireFrame())
            {
                // No need to do anything if DB is already logged in. 
                if (ZetaDia.Service.IsValid && ZetaDia.Service.Hero.IsValid)
                    return;
            }

            IsKickstarted = true;
            Logger.Info("YARBot Initialized");

            var currentProfile = ProfileManager.CurrentProfile;
            if (currentProfile == null)
            {
                // Make DB not throw its toys when started with cmd line args without -profile
                var xmlFile = XDocument.Parse(YarKickstart);
                ProfileManager.CurrentProfile = Profile.Load(xmlFile.Root);

                // Make OrderBot not throw its toys when it tries to load a profile that needs plugins that haven't compiled yet.
                var path = Path.Combine(GlobalSettings.Instance.BotsPath, "YARBot", "kickstart.xml");
                xmlFile.Save(path);
                _lastProfile = GlobalSettings.Instance.LastProfile;
                GlobalSettings.Instance.LastProfile = path;
            }

            // If DB is started with command line arguments then after it logs into DB it will 
            // Init and Enable plugins from the wrong thread ('Bot Main' instead of application thread id 1)

            Task.Factory.StartNew(LoginCoordinator, TaskCreationOptions.LongRunning);
        }

        private static string _pluginPath;
        private static bool _isWaiting = true;
        private static List<string> _enabledPlugins;
        private static string _lastProfile;

        private static void LoginCoordinator()
        {
            if (!ZetaDia.Memory.IsProcessOpen || !ZetaDia.Memory.Executor.IsInitialized)
                return;

            PluginManager.OnPluginsReloaded += OnPluginsLoaded;

            while (true)
            {
                try
                {
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        if (ZetaDia.Service.IsValid && ZetaDia.Service.Hero.IsValid && ZetaDia.Service.Hero.HeroId > 0 && BotMain.IsRunning)
                        { 
                            Logger.Info("Arrived at hero selection screen");

                            // There is no way to prevent DB from 'reloading' plugins from /plugins/ directory when started with CMDLine/login.
                            // but we can stop it from initializing/enabling plugins and then do it ourselves from the proper thread.

                            _enabledPlugins = CharacterSettings.Instance.EnabledPlugins;
                            CharacterSettings.Instance.EnabledPlugins = new List<string>();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.InfoFormat("{0}", ex);
                    break;
                }
                Thread.Sleep(500);
            }
        }

        private static void OnPluginsLoaded(object sender, EventArgs eventArgs)
        {
            PluginManager.OnPluginsReloaded -= OnPluginsLoaded;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_enabledPlugins != null)
                {                    
                    PluginManager.SetEnabledPlugins(_enabledPlugins.ToArray());

                    var comms = PluginManager.Plugins.FirstOrDefault(p => p.Plugin.Name.ToLower().Contains("YAR Comms"));
                    if(comms != null && !comms.Enabled)
                        comms.Enabled = true;
                }
                else
                {                    
                    PluginManager.SetEnabledPlugins(PluginManager.Plugins.Select(p => p.Plugin.Name).ToArray());
                }

                //GlobalSettings.Instance.LastProfile = _lastProfile;
            });
        }

        public void Dispose()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Pulse()
        {

        }

        public void Initialize()
        {
        }

        public bool TryGetBotProfile(string path, out Profile profile)
        {
            profile = null;
            return false;
        }

        public string Name { get { return "YARKickstart"; } }

        public Composite Logic { get { return new Action(ret => RunStatus.Failure); } }
    }

}