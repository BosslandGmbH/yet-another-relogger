using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger
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

    public class UdpLogListener
    {
        private static readonly ILogger s_logger = Logger.Instance.GetLogger<UdpLogListener>();
        #region Instance
        private static UdpLogListener _instance;
        public static UdpLogListener Instance => _instance ?? (_instance = new UdpLogListener());
        private UdpLogListener()
        {
            ListeningPort = NetworkTools.GetFreeUdpPort(55000, 56000);
            _listener = new UdpClient(ListeningPort);
            _running = false;
        }
        #endregion

        private readonly UdpClient _listener;
        private volatile bool _running;

        public int ListeningPort { get; }
        
        public void Start()
        {
            _running = true;
            Listen();
        }
        public void Stop()
        {
            _running = false;
        }

        private async void Listen()
        {
            var resp = await _listener.ReceiveAsync();
            PacketReceived(resp);
            if (_running)
                Listen();
        }

        private async void PacketReceived(UdpReceiveResult packet)
        {
            var msg = Encoding.UTF8.GetString(packet.Buffer);
            dynamic json = JObject.Parse(msg);
            var rmsg = json.RenderedMessage.Value;
            var pid = json.Properties.PID.Value;
            var context = json.Properties.SourceContext.Value;
            var level = json.Level.Value;
            if(!Enum.TryParse(level, out LogEventLevel l))
                l = LogEventLevel.Verbose;
            s_logger.ForContext("PID", pid).ForContext("SourceContext", context).Write(l, "{rmsg}", rmsg);
        }
    }
}
