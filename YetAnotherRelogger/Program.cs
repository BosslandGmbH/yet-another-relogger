using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using YetAnotherRelogger.Forms;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger
{
    internal static class Program
    {
        //public const string VERSION = "0.2.3.0";
        public const int Sleeptime = 10;

        public static MainForm2 Mainform;
        public static bool IsRunAsAdmin;
        public static bool Pause;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                // Allow only one instance to be run
                if (!SingleInstance.Start())
                {
                    SingleInstance.ShowFirstInstance();
                    return;
                }

                // Run as admin check
                var identity = WindowsIdentity.GetCurrent();
                IsRunAsAdmin = (new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator));

                // Get Commandline args
                CommandLineArgs.Get();

                if (CommandLineArgs.SafeMode)
                {
                    var result = MessageBox.Show(@"Launching in safe mode!
This will reset some features",
                        @"YetAnotherRelogger Safe Mode", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel)
                        return;
                }

                // Load settings
                BotSettings.Instance.Load();
                Settings.Default.Reload();
                Settings.Default.Upgrade();

                if (Settings.Default.AutoPosScreens == null ||
                    (Settings.Default.AutoPosScreens != null && Settings.Default.AutoPosScreens.Count == 0))
                    AutoPosition.UpdateScreens();

                // Start background threads
                Relogger.Instance.Start();
                Communicator.Instance.Start();

                if (!CommandLineArgs.SafeMode)
                {
                    if (Settings.Default.StatsEnabled)
                        StatsUpdater.Instance.Start();
                    if (Settings.Default.FocusCheck)
                        ForegroundChecker.Instance.Start();
                }
                else
                {
                    Settings.Default.StatsEnabled = false;
                    Settings.Default.FocusCheck = false;
                    AutoPosition.UpdateScreens();
                }


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Mainform = new MainForm2();
                Application.Run(Mainform);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteGlobal(ex.ToString());
            }
            // Clean up
            SingleInstance.Stop();
            Settings.Default.Save();
            Logger.Instance.WriteGlobal("Closed!");
            Logger.Instance.ClearBuffer();
        }
    }

    #region SingleInstance

    // http://www.codeproject.com/Articles/32908/C-Single-Instance-App-With-the-Ability-To-Restore
    public static class SingleInstance
    {
        public static readonly int WmShowfirstinstance = WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}",
            ProgramInfo.AssemblyGuid);

        private static Mutex _mutex;

        public static bool Start()
        {
            var mutexName = $"Local\\{ProgramInfo.AssemblyGuid}";

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);
            _mutex = new Mutex(true, mutexName, out var onlyInstance);
            return onlyInstance;
        }

        public static void ShowFirstInstance()
        {
            WinApi.PostMessage(
                (IntPtr) WinApi.HwndBroadcast,
                WmShowfirstinstance,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static void Stop()
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }
    }

    #endregion

    #region ProgramInfo

    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof (GuidAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((GuidAttribute) attributes[0]).Value;
            }
        }
    }

    #endregion
}
