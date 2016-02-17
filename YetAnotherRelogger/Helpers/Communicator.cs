using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Helpers
{
    public class Communicator
    {
        #region singleton

        private static readonly Communicator instance = new Communicator();

        static Communicator()
        {
        }

        private Communicator()
        {
        }

        public static Communicator Instance
        {
            get { return instance; }
        }

        #endregion

        private static int _connections;
        private Thread _threadWorker;

        public static int Connections
        {
            get { return _connections; }
            set
            {
                _connections = value < 0 ? 0 : value;
                StatConnections += _connections;
            }
        }

        public static int StatConnections { get; set; }
        public static int StatFailed { get; set; }

        public void Start()
        {
            _threadWorker = new Thread(Worker) { IsBackground = true, Name = "CommunicatorWorker" };
            _threadWorker.Start();
        }

        public void Worker()
        {
            while (true)
            {
                try
                {
                    var serverStream = new NamedPipeServerStream("YetAnotherRelogger", PipeDirection.InOut, 254);
                    serverStream.WaitForConnection();
                    var handleClient = new HandleClient(serverStream);
                    new Thread(handleClient.Start) { Name = "CommunicatorHandleClient" }.Start();
                }
                catch (Exception ex)
                {
                    StatFailed++;
                    DebugHelper.Exception(ex);
                }
            }
        }

        private class HandleClient : IDisposable
        {
            private StreamReader _reader;
            private NamedPipeServerStream _stream;
            private StreamWriter _writer;

            public HandleClient(NamedPipeServerStream stream)
            {
                _stream = stream;
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };
            }

            public void Dispose()
            {
                //Free managed resources
                if (_stream != null)
                {
                    try
                    {
                        _stream.Close();
                    }
                    catch (ObjectDisposedException) { }
                    catch { }
                    _stream = null;
                }
                if (_reader != null)
                {
                    try
                    {
                        _reader.Close();
                    }
                    catch (ObjectDisposedException) { }
                    catch { }
                    _reader = null;
                }
                //if (_writer != null)
                //{
                //    try
                //    {
                //        _writer.Close();
                //    }
                //    catch (ObjectDisposedException) { }
                //    catch { }
                //    _writer = null;
                //}
            }

            public void Start()
            {
                bool isXml = false;
                string xml = string.Empty;
                DateTime duration = DateTime.UtcNow;
                Connections++;
                try
                {
                    Debug.WriteLine("PipeConnection [{0}]: Connected:{1}", _stream.GetHashCode(), _stream.IsConnected);
                    while (_stream.IsConnected)
                    {
                        string dataLine = _reader.ReadLine();
                        if (dataLine == null)
                        {
                            Thread.Sleep(Program.Sleeptime);
                            continue;
                        }
                        if (dataLine.Equals("END"))
                        {
                            Debug.WriteLine("PipeConnection [{0}]: Duration:{1} XML:{2}", _stream.GetHashCode(),
                                General.DateSubtract(duration, false), xml);
                            HandleXml(xml);
                        }

                        if (dataLine.StartsWith("XML:"))
                        {
                            dataLine = dataLine.Substring(4);
                            isXml = true;
                        }

                        if (isXml)
                        {
                            xml += dataLine + "\n";
                        }
                        else
                        {
                            Debug.WriteLine("PipeConnection [{0}]: Duration:{1} Data:{2}", _stream.GetHashCode(),
                                General.DateSubtract(duration, false), dataLine);
                            HandleMsg(dataLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    StatFailed++;
                }
                Debug.WriteLine("PipeConnection [{0}]: Connected:{1} Duration:{2}ms", _stream.GetHashCode(),
                    _stream.IsConnected, General.DateSubtract(duration, false));
                Dispose();
                Connections--;
            }

            private void HandleXml(string data)
            {
                BotStats stats;
                var xml = new XmlSerializer(typeof(BotStats));
                using (var stringReader = new StringReader(data))
                {
                    stats = xml.Deserialize(stringReader) as BotStats;
                }

                if (stats != null)
                {
                    try
                    {
                        BotClass bot =
                            BotSettings.Instance.Bots.FirstOrDefault(
                                b =>
                                    (b != null && b.Demonbuddy != null && b.Demonbuddy.Proc != null) &&
                                    b.Demonbuddy.Proc.Id == stats.Pid);
                        if (bot != null)
                        {
                            if (bot.AntiIdle.Stats == null)
                                bot.AntiIdle.Stats = new BotStats();

                            bot.AntiIdle.UpdateCoinage(stats.Coinage);
                            bot.AntiIdle.Stats = stats;
                            bot.AntiIdle.LastStats = DateTime.UtcNow;
                            Send(bot.AntiIdle.Reply());
                            return;
                        }

                        Logger.Instance.WriteGlobal("Could not find a matching bot for Demonbuddy:{0}", stats.Pid);
                        return;
                    }
                    catch (Exception ex)
                    {
                        StatFailed++;
                        Send("Internal server error: " + ex.Message);
                        DebugHelper.Exception(ex);
                        return;
                    }
                }
                Send("Roger!");
            }

            private void HandleMsg(string msg)
            {
                // Message Example:
                // PID:CMD DATA
                // 1234:GameLeft 25-09-1985 18:27:00
                Debug.WriteLine("Recieved: " + msg);
                try
                {
                    string pid = msg.Split(':')[0];
                    string cmd = msg.Substring(pid.Length + 1).Split(' ')[0];
                    int x;
                    msg = msg.Substring(((x = pid.Length + cmd.Length + 2) >= msg.Length ? 0 : x));

                    BotClass b =
                        BotSettings.Instance.Bots.FirstOrDefault(
                            f =>
                                (f.Demonbuddy != null && f.Demonbuddy.Proc != null) &&
                                f.Demonbuddy.Proc.Id == Convert.ToInt32(pid));
                    if (b == null)
                    {
                        Send("Error: Unknown process");
                        StatFailed++;
                        return;
                    }

                    long nowTicks = DateTime.UtcNow.Ticks;

                    switch (cmd)
                    {
                        case "Initialized":
                            b.AntiIdle.Stats = new BotStats
                            {
                                LastGame = nowTicks,
                                LastPulse = nowTicks,
                                PluginPulse = nowTicks,
                                LastRun = nowTicks
                            };
                            
                            b.AntiIdle.LastStats = DateTime.UtcNow;
                            b.AntiIdle.State = IdleState.CheckIdle;
                            b.AntiIdle.IsInitialized = true;
                            b.AntiIdle.InitAttempts = 0;
                            Send("Roger!");
                            break;
                        case "GameLeft":
                            b.ProfileSchedule.Count++;
                            if (b.ProfileSchedule.Current.Runs > 0)
                                Logger.Instance.Write(b, "Runs completed ({0}/{1})", b.ProfileSchedule.Count,
                                    b.ProfileSchedule.MaxRuns);
                            else
                                Logger.Instance.Write(b, "Runs completed {0}", b.ProfileSchedule.Count);

                            if (b.ProfileSchedule.IsDone)
                            {
                                string newprofile = b.ProfileSchedule.GetProfile;
                                Logger.Instance.Write(b, "Next profile: {0}", newprofile);
                                Send("LoadProfile " + newprofile);
                            }
                            else
                                Send("Roger!");
                            break;
                        case "NewDifficultyLevel":
                            Logger.Instance.Write(b, "Sending DifficultyLevel: {0}",
                                b.ProfileSchedule.Current.DifficultyLevel);
                            Send("DifficultyLevel " + (int)b.ProfileSchedule.Current.DifficultyLevel);
                            break;
                        case "UserStop":
                            b.Status = string.Format("User Stop: {0:d-m H:M:s}", DateTime.UtcNow);
                            b.AntiIdle.State = IdleState.UserStop;
                            Logger.Instance.Write(b, "Demonbuddy stopped by user");
                            Send("Roger!");
                            break;
                        case "StartDelay":
                            var delay = new DateTime(long.Parse(msg));
                            b.AntiIdle.StartDelay = delay.AddSeconds(60);
                            b.AntiIdle.State = IdleState.StartDelay;
                            Send("Roger!");
                            break;
                        // Giles Compatibility
                        case "ThirdpartyStop":
                            b.Status = string.Format("Thirdparty Stop: {0:d-m H:M:s}", DateTime.UtcNow);
                            b.AntiIdle.State = IdleState.UserStop;
                            Logger.Instance.Write(b, "Demonbuddy stopped by Thirdparty");
                            Send("Roger!");
                            break;
                        case "TrinityPause":
                            b.AntiIdle.State = IdleState.UserPause;
                            Logger.Instance.Write(b, "Trinity Pause Detected");
                            Send("Roger!");
                            break;
                        case "AllCompiled":
                            {
                                Logger.Instance.Write(b, "Check Force Enable Plugins? {0}", b.Demonbuddy.ForceEnableAllPlugins);
                                Send(b.Demonbuddy.ForceEnableAllPlugins ? "ForceEnableAll" : "ForceEnableYar");
                                //Send(b.ProfileSchedule.GetProfile);
                                break;
                            }
                        case "RequestProfile":
                        {
                            var profile = b.ProfileSchedule.GetProfile;
                                Logger.Instance.Write(b, "Sending Current Profile to Load {0}", profile);
                                Send("LoadProfile " + profile);
                                break;
                            }
                        case "CrashTender":
                            if (Settings.Default.UseKickstart && File.Exists(msg))
                                b.Demonbuddy.CrashTender(msg);
                            else
                                b.Demonbuddy.CrashTender();
                            Send("Roger!");
                            break;
                        case "CheckConnection":
                            ConnectionCheck.CheckValidConnection(true);
                            Send("Roger!");
                            break;
                        case "D3Exit":
                            Send("Shutdown");
                            b.Diablo.Proc.CloseMainWindow();
                            break;
                        // Unknown command reply
                        default:
                            Send("Unknown command!");
                            Logger.Instance.WriteGlobal("Unknown command recieved: " + msg);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    StatFailed++;
                    Send("Internal server error: " + ex.Message);
                    DebugHelper.Exception(ex);
                }
            }

            private void Send(string msg)
            {
                try
                {
                    Debug.WriteLine("Replying: " + msg);
                    msg = msg.Trim();
                    if (!msg.EndsWith("\n"))
                        msg += "\n";
                    _writer.WriteLine(msg);
                    //_writer.Flush();
                }
                catch (Exception ex)
                {
                    StatFailed++;
                    Logger.Instance.WriteGlobal("msg={0} ex={1}", msg, ex);
                    DebugHelper.Exception(ex);
                }
            }

            public void SendShutdown()
            {
                Send("Shutdown");
            }
        }
    }
}