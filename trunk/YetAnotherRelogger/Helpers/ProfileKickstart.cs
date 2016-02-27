using System;
using System.Xml.Linq;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Properties;
using Zeta.Bot;

namespace YetAnotherRelogger.Helpers
{
    //public static class ProfileKickstart
    //{
    //    // GameParams Regex Pattern
    //    private const string GameParamsRegex = @"(<GameParams .+/>)";

    //    // Kickstart profile layout
    //    private const string YarKickstart = @"

    //        <!-- This is a automaticly generated profile by YetAnotherRelogger -->
    //        <Profile>
    //          <Name>YAR Kickstart</Name>
    //          <KillMonsters>True</KillMonsters>
    //          <PickupLoot>True</PickupLoot> 
    //          <Order />
    //        </Profile>";

    //    public static string GenerateKickstart(string DBLocation)
    //    {
    //        try
    //        {
    //            var directory = Path.GetDirectoryName(DBLocation);
    //            if (directory == null)
    //                return string.Empty;

    //            //ProfileManager.Load();

    //            var kickstartProfilePath = Path.Combine(directory, "Profiles", "Kickstart.xml"); 

    //            XDocument xmlFile = XDocument.Parse(YarKickstart);

    //            xmlFile.Save(kickstartProfilePath);

    //            return kickstartProfilePath;

    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.Instance.Write("Failed to generate Kickstart profile: {0}", ex.Message);
    //            DebugHelper.Exception(ex);
    //        }
    //        return string.Empty;
    //    }
    //}
}