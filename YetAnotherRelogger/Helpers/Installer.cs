using System;
using System.IO;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Helpers
{
    public static class Installer
    {
        public static bool Check(string path)
        {
            return false;

            //try
            //{
            //    Logger.Instance.Write("Checking plugin: {0}", path);
            //    if (File.Exists(path))
            //    {
            //        string check = Resources.Plugin.Split('\n')[0].TrimEnd(); // read the first line of Plugin.cs
            //        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            //        {
            //            var reader = new StreamReader(fs);
            //            for (int i = 0; i < 3; i++) // check the first three lines of installed Plugin.cs
            //            {
            //                string line = reader.ReadLine();
            //                if (line != null && line.Equals(check)) // line matches resource\Plugin first line
            //                {
            //                    Logger.Instance.Write("Plugin is installed and latest version: {0}", check);
            //                    return true;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Logger.Instance.Write("Plugin does not exist");
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DebugHelper.Exception(ex);
            //}
            //Logger.Instance.Write("Plugin is outdated!");
            //return false;
        }

        public static void InstallPlugin(string path)
        {
            try
            {
                Logger.Instance.Write("Installing latest plugin: {0}", path);
                if (File.Exists(path))
                    File.Delete(path);
                else if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                File.WriteAllText(path, Resources.Plugin);
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        public static void InstallBot(string path)
        {
            try
            {
                Logger.Instance.Write("Installing latest YARBot: {0}", path);
                if (File.Exists(path))
                    File.Delete(path);
                else if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                File.WriteAllText(path, Resources.Bot);
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }



    }
}