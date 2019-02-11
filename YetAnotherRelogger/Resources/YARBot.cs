using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace YARKickstart
{
    public class YARKickstart : IBot
    {
        public string Name => "YARKickstart";

        public static bool IsKickstarted;

        private static readonly ILogger s_logger = Zeta.Common.Logger.GetLoggerInstanceForType();

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

            s_logger.Information("YARBot Initialized");

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

            Task.Run(KillAfterLogin);
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
            s_logger.Information("YARBot Ending Demonbuddy Process");
            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(ExitDemonBuddy);
                    return;
                }
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                s_logger.Warning(ex, "Exception during Exit");
            }
        }
        
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
        public Composite Logic => new Action(ret => RunStatus.Failure);
    }
}
