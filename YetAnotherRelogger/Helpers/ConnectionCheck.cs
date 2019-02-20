﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
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
                        foreach (var bot in BotSettings.Instance.Bots.Where(bot => bot != null && bot.IsRunning))
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
                var data = wc.OpenRead("http://checkip.dyndns.org");

                var hostname = string.Empty;
                if (data != null)
                {
                    using (var reader = new StreamReader(data))
                    {
                        var s = reader.ReadToEnd();
                        var m =
                            new Regex(@".*Current IP Address: ([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}).*").Match
                                (s);
                        if (m.Success)
                        {
                            var ip = m.Groups[1].Value;
                            if (!silent)
                                DebugHelper.Write(
                                    $"Host/IP Check: IP {ip}{(!string.IsNullOrEmpty(hostname) ? " HostName: " + hostname : "")}");
                            if (!ValidIp(ip))
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
                DebugHelper.Write($"ValidConnection: {ex.Message}");
                DebugHelper.Exception(ex);
                return false;
            }
            return true;
        }

        private static bool ValidIp(string ip)
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

            foreach (var line in Settings.Default.ConnectionCheckIpHostList.Split('\n'))
            {
                var test = line.Replace(" ", string.Empty).Trim();
                if (test.Length < 1)
                    continue;
                var allowed = test.StartsWith("@");
                if (allowed)
                    test = test.Substring(1, test.Length - 1);
                if (Settings.Default.ConnectionCheckIpCheck)
                {
                    // Check Ip range
                    var m =
                        new Regex(
                            @"([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})-([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})")
                            .Match(test);
                    if (m.Success)
                    {
                        var lowerip = IPAddress.Parse(m.Groups[1].Value);
                        var higherip = IPAddress.Parse(m.Groups[2].Value);
                        var inrange = new IpAddressRange(lowerip, higherip).IsInRange(IPAddress.Parse(ip));
                        if (inrange)
                        {
                            if (allowed)
                            {
                                DebugHelper.Write($"Valid Connection: IP {ip} in range -> {lowerip}-{higherip}");
                                return true;
                            }
                            DebugHelper.Write($"Invalid Connection: IP {ip} in range -> {lowerip}-{higherip}");
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
                                DebugHelper.Write($"Valid Connection: IP match {ip} -> {test}");
                                return true;
                            }
                            DebugHelper.Write($"Invalid Connection: IP match {ip} -> {test}");
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
                        DebugHelper.Write($"Valid Connection: Host match {hostname} -> {test}");
                        return true;
                    }
                    DebugHelper.Write($"Invalid Connection: Host match {hostname} -> {test}");
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
                        foreach (var bot in BotSettings.Instance.Bots.Where(bot => bot != null && bot.IsRunning))
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
                var reply = ping.Send(Settings.Default.ConnectionCheckPingHost1, 3000);
                if (reply == null)
                {
                    if (!silent)
                        DebugHelper.Write("PingCheck: reply = NULL");
                }
                else if (reply.Status != IPStatus.Success)
                {
                    if (!silent)
                        DebugHelper.Write($"PingCheck: {reply.Address} -> {reply.Status}");
                }
                else
                {
                    if (!silent)
                        DebugHelper.Write($"PingCheck: {reply.Address} -> {reply.RoundtripTime}ms");
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
                    DebugHelper.Write($"PingCheck: Ping -> {Settings.Default.ConnectionCheckPingHost2}");
                var reply = ping.Send(Settings.Default.ConnectionCheckPingHost2, 3000);
                if (reply == null)
                {
                    if (!silent)
                        DebugHelper.Write("PingCheck: reply = NULL");
                }
                else if (reply.Status != IPStatus.Success)
                {
                    if (!silent)
                        DebugHelper.Write($"PingCheck: {reply.Address} -> {reply.Status}");
                }
                else
                {
                    if (!silent)
                        DebugHelper.Write($"PingCheck: {reply.Address} -> {reply.RoundtripTime}ms");
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
    public class IpAddressRange
    {
        private readonly AddressFamily _addressFamily;
        private readonly byte[] _lowerBytes;
        private readonly byte[] _upperBytes;

        public IpAddressRange(IPAddress lower, IPAddress upper)
        {
            // Assert that lower.AddressFamily == upper.AddressFamily

            _addressFamily = lower.AddressFamily;
            _lowerBytes = lower.GetAddressBytes();
            _upperBytes = upper.GetAddressBytes();
        }

        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != _addressFamily)
            {
                return false;
            }

            var addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (var i = 0;
                i < _lowerBytes.Length &&
                (lowerBoundary || upperBoundary);
                i++)
            {
                if ((lowerBoundary && addressBytes[i] < _lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > _upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == _lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == _upperBytes[i]);
            }

            return true;
        }
    }

    #endregion
}
