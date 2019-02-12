﻿using Serilog;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger
{
    public class UdpLogListener
    {
        private static readonly ILogger s_logger = Log.ForContext<UdpLogListener>();
        #region Instance
        private static UdpLogListener _instance;
        public static UdpLogListener Instance => _instance ?? (_instance = new UdpLogListener());
        private UdpLogListener()
        {
            _listener = new UdpClient(Settings.Default.LogListenerPort);
            _running = false;
        }
        #endregion

        private readonly UdpClient _listener;
        private volatile bool _running;
        
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
            var json = JObject.Parse(msg);
            s_logger.Verbose("Message received {json}", json);
        }
    }
}