﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Helpers
{
    public class ConnectionCheck
    {
        #region IP / Host check

        // Get external IP from: checkip.dyndns.org
        private static DateTime _lastcheck;
        private static bool _laststate;

        public static bool ValidConnection
        {
            get
            {
                if (!Settings.Default.ConnectionCheckIpHostCloseBots)
                    return true;

                // Check internet every 60 seconds
                if (General.DateSubtract(_lastcheck) > 30)
                {
                    _lastcheck = DateTime.UtcNow;
                    if (!CheckValidConnection(true))
                    {
                        _laststate = false;
                        Logger.Instance.Write("Invalid external IP or Hostname!");
                        Logger.Instance.Write("Waiting 30 seconds and check again!");
                        foreach (BotClass bot in BotSettings.Instance.Bots.Where(bot => bot != null && bot.IsRunning))
                        {
                            if (bot.Diablo.IsRunning || bot.Demonbuddy.IsRunning)
                            {
                                Logger.Instance.Write(bot, "Stopping bot (No Valid Internet Connection!)");
                                bot.Diablo.Stop();
                                bot.Demonbuddy.Stop();
                            }
                            bot.Status = "Waiting on internet connection";
                        }
                    }
                    else
                    {
                        _laststate = true;
                    }
                }
                return _laststate;
            }
        }

        public static bool CheckValidConnection(bool silent = false)
        {
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Stream data = wc.OpenRead("http://checkip.dyndns.org");

                string ip = string.Empty;
                string hostname = string.Empty;
                if (data != null)
                {
                    using (var reader = new StreamReader(data))
                    {
                        string s = reader.ReadToEnd();
                        Match m =
                            new Regex(@".*Current IP Address: ([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}).*").Match
                                (s);
                        if (m.Success)
                        {
                            ip = m.Groups[1].Value;
                            if (!silent)
                                DebugHelper.Write(string.Format("Host/IP Check: IP {0}{1}", ip,
                                    !string.IsNullOrEmpty(hostname) ? " HostName: " + hostname : ""));
                            if (!validIp(ip, silent))
                                return false;
                        }
                        else
                        {
                            throw new Exception("No IP found!");
                        }
                    }
                    // data.Close();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Write(string.Format("ValidConnection: {0}", ex.Message));
                DebugHelper.Exception(ex);
                return false;
            }
            return true;
        }

        private static bool validIp(string ip, bool silent)
        {
            string hostname = null;
            try
            {
                hostname = Dns.GetHostEntry(ip).HostName;
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }

            foreach (string line in Settings.Default.ConnectionCheckIpHostList.Split('\n'))
            {
                string test = line.Replace(" ", string.Empty).Trim();
                if (test.Length < 1)
                    continue;
                bool allowed = test.StartsWith("@");
                if (allowed)
                    test = test.Substring(1, test.Length - 1);
                if (Settings.Default.ConnectionCheckIpCheck)
                {
                    // Check Ip range
                    Match m =
                        new Regex(
                            @"([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})-([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})")
                            .Match(test);
                    if (m.Success)
                    {
                        IPAddress lowerip = IPAddress.Parse(m.Groups[1].Value);
                        IPAddress higherip = IPAddress.Parse(m.Groups[2].Value);
                        bool inrange = new IPAddressRange(lowerip, higherip).IsInRange(IPAddress.Parse(ip));
                        if (inrange)
                        {
                            if (allowed)
                            {
                                DebugHelper.Write(string.Format("Valid Connection: IP {0} in range -> {1}-{2}", ip,
                                    lowerip, higherip));
                                return true;
                            }
                            DebugHelper.Write(string.Format("Invalid Connection: IP {0} in range -> {1}-{2}", ip,
                                lowerip, higherip));
                            return false;
                        }
                        continue;
                    }
                    // Check single IP
                    m = new Regex(@"([0-9*]{1,3}\.[0-9*]{1,3}\.[0-9*]{1,3}\.[0-9*]{1,3})").Match(test);
                    if (m.Success)
                    {
                        test = m.Groups[1].Value;
                        if (General.WildcardMatch(test, ip))
                        {
                            if (allowed)
                            {
                                DebugHelper.Write(string.Format("Valid Connection: IP match {0} -> {1}", ip, test));
                                return true;
                            }
                            DebugHelper.Write(string.Format("Invalid Connection: IP match {0} -> {1}", ip, test));
                            return false;
                        }
                        continue;
                    }
                }

                if (hostname == null)
                    continue;
                if (General.WildcardMatch(test.ToLower(), hostname.ToLower()))
                {
                    if (allowed)
                    {
                        DebugHelper.Write(string.Format("Valid Connection: Host match {0} -> {1}", hostname, test));
                        return true;
                    }
                    DebugHelper.Write(string.Format("Invalid Connection: Host match {0} -> {1}", hostname, test));
                    return false;
                }
            }
            return true;
        }

        /*
        private static bool validHost(string hostname, bool silent)
        {
            // Always return true when Host Check is disabled
            if (!Settings.Default.ConnectionCheckHostCheck) return true;
            foreach (var line in Settings.Default.ConnectionCheckIpHostList.Split('\n'))
            {
                var test = line.Replace(" ", string.Empty);
                var m = new Regex(@"([0-9*]{1,3}\.[0-9*]{1,3}\.[0-9*]{1,3}\.[0-9*]{1,3})").Match(test);

                if (m.Success) continue;

                if (General.WildcardMatch(test.ToLower(), hostname.ToLower()))
                {
                    DebugHelper.Write(string.Format("ValidConnection: Host match {0} -> {1}", hostname, test));
                    return false;
                }
            }
            return true;
        }
         */

        #endregion

        #region PingCheck & IsConnected

        private static DateTime _lastpingcheck;
        private static bool _lastpingstate;

        public static bool IsConnected
        {
            get
            {
                if (!Settings.Default.ConnectionCheckCloseBots)
                    return true;
                // Check internet every 60 seconds
                if (General.DateSubtract(_lastpingcheck) > 30)
                {
                    _lastpingcheck = DateTime.UtcNow;
                    if (!PingCheck(true))
                    {
                        Logger.Instance.Write("Waiting 30 seconds and check again!");
                        _lastpingstate = false;
                        foreach (BotClass bot in BotSettings.Instance.Bots.Where(bot => bot != null && bot.IsRunning))
                        {
                            if (bot.Diablo.IsRunning || bot.Demonbuddy.IsRunning)
                            {
                                Logger.Instance.Write(bot, "Stopping bot (No Internet Connection!)");
                                bot.Demonbuddy.Stop();
                                bot.Diablo.Stop();
                            }
                            bot.Status = "Waiting on internet connection";
                        }
                    }
                    else
                    {
                        _lastpingstate = true;
                    }
                }
                return _lastpingstate;
            }
        }

        public static bool PingCheck(bool silent = false)
        {
            var ping = new Ping();
            try
            {
                // Ping host 1
                if (!silent)
                    Logger.Instance.WriteGlobal("PingCheck: Ping -> {0}", Settings.Default.ConnectionCheckPingHost1);
                PingReply reply = ping.Send(Settings.Default.ConnectionCheckPingHost1, 3000);
                if (reply == null)
                {
                    if (!silent)
                        DebugHelper.Write("PingCheck: reply = NULL");
                }
                else if (reply.Status != IPStatus.Success)
                {
                    if (!silent)
                        DebugHelper.Write(string.Format("PingCheck: {0} -> {1}", reply.Address, reply.Status));
                }
                else
                {
                    if (!silent)
                        DebugHelper.Write(string.Format("PingCheck: {0} -> {1}ms", reply.Address, reply.RoundtripTime));
                    return true;
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Write(string.Format("PingCheck: Failed with message: " + ex.Message));
                DebugHelper.Exception(ex);
            }

            try
            {
                // Ping host 2
                if (!silent)
                    DebugHelper.Write(string.Format("PingCheck: Ping -> {0}", Settings.Default.ConnectionCheckPingHost2));
                PingReply reply = ping.Send(Settings.Default.ConnectionCheckPingHost2, 3000);
                if (reply == null)
                {
                    if (!silent)
                        DebugHelper.Write(string.Format("PingCheck: reply = NULL"));
                }
                else if (reply.Status != IPStatus.Success)
                {
                    if (!silent)
                        DebugHelper.Write(string.Format("PingCheck: {0} -> {1}", reply.Address, reply.Status));
                }
                else
                {
                    if (!silent)
                        DebugHelper.Write(string.Format("PingCheck: {0} -> {1}ms", reply.Address, reply.RoundtripTime));
                    return true;
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Write(string.Format("PingCheck: Failed with message: " + ex.Message));
                DebugHelper.Exception(ex);
            }

            return false;
        }

        #endregion

        public static void Reset()
        {
            _laststate = true;
            _lastpingstate = true;
        }
    }

    #region IPAdressRange Check

    public class IPAddressRange
    {
        private readonly AddressFamily addressFamily;
        private readonly byte[] lowerBytes;
        private readonly byte[] upperBytes;

        public IPAddressRange(IPAddress lower, IPAddress upper)
        {
            // Assert that lower.AddressFamily == upper.AddressFamily

            addressFamily = lower.AddressFamily;
            lowerBytes = lower.GetAddressBytes();
            upperBytes = upper.GetAddressBytes();
        }

        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0;
                i < lowerBytes.Length &&
                (lowerBoundary || upperBoundary);
                i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }
    }

    #endregion
}