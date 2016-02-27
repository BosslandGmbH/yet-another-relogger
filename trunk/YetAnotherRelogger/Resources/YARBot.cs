using System;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
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
              <Order />
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

            var currentProfile = ProfileManager.CurrentProfile;
            if (currentProfile == null)
            {
                // Make DB not throw its toys when started with cmd line args without -profile
                var xmlFile = XDocument.Parse(YarKickstart);
                ProfileManager.CurrentProfile = Profile.Load(xmlFile.Root);

                // Make OrderBot not throw its toys when it tries to load a profile that needs plugins that haven't compiled yet.
                var path = Path.Combine(GlobalSettings.Instance.BotsPath, "kickstart.xml");
                xmlFile.Save(path);
                GlobalSettings.Instance.LastProfile = path;
            }

            // If DB is started with command line arguments then after it logs into DB it will 
            // Init and Enable plugins from the wrong thread ('Bot Main' instead of application thread id 1)

            // Bots are constructed from the correct thread, allowing this workaround where 
            // YARbot calls the plugin methods a seconds time, from the correct thread.

            Task.Factory.StartNew(LoginCoordinator, TaskCreationOptions.LongRunning);
        }

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
                        if (ZetaDia.Service.IsValid && ZetaDia.Service.Hero.IsValid && BotMain.IsRunning)
                        {
                            // On Hero Selection Screen                            
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.InfoFormat("{0}", ex);
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        private static void OnPluginsLoaded(object sender, EventArgs eventArgs)
        {
            PluginManager.OnPluginsReloaded -= OnPluginsLoaded;

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var plugin in PluginManager.Plugins)
                {
                    plugin.Plugin.OnInitialize();
                    plugin.Plugin.OnEnabled();
                }
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