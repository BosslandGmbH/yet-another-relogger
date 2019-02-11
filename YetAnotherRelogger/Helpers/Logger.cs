using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Enums;

namespace YetAnotherRelogger.Helpers
{
    public sealed class Logger
    {
        #region singleton

        private static readonly object s_bufferLock = 0;

        private static readonly Logger s_instance = new Logger();

        static Logger()
        {
        }

        private Logger()
        {
            lock (s_bufferLock)
            {
                _buffer = new List<LogMessage>();
                Initialize();
            }
        }

        public static Logger Instance => s_instance;

        #endregion

        private readonly List<LogMessage> _buffer;
        private bool _canLog;
        private string _logfile;

        public string Logfile
        {
            get => _logfile;
            private set => _logfile = value;
        }

        public string LogDirectory => Path.GetDirectoryName(Logfile);

        private void Initialize()
        {
            var filename = $"{DateTime.Now:yyyy-MM-dd HH.mm}";
            _logfile = $@"{Path.GetDirectoryName(Application.ExecutablePath)}\Logs\{filename}.txt";
            Debug.WriteLine(_logfile);

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(_logfile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_logfile));
                _canLog = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Creating log file failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _canLog = false;
            }
        }

        /// <summary>
        ///     Write log message for active bot
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write(string format, params object[] args)
        {
            var message = new LogMessage();
            if (Relogger.Instance.CurrentBot != null)
                message.Message = $"<{Relogger.Instance.CurrentBot.Name}> {string.Format(format, args)}";
            else
                message.Message = $"[{DateTime.Now}] {string.Format(format, args)}";
            s_instance.AddBuffer(message);
            AddToRtb(message);
        }

        /// <summary>
        ///     Write Log message for specific bot
        /// </summary>
        /// <param name="bot">BotClass</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write(BotClass bot, string format, params object[] args)
        {
            if (bot == null)
            {
                WriteGlobal(format, args);
                return;
            }
            var message = new LogMessage { Message = $"<{bot.Name}> {string.Format(format, args)}"};
            s_instance.AddBuffer(message);
            AddToRtb(message);
        }

        /// <summary>
        ///     Write global log message
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteGlobal(string format, params object[] args)
        {
            var message = new LogMessage { Message = $"{string.Format(format, args)}"};
            s_instance.AddBuffer(message);
            AddToRtb(message);
        }

        /// <summary>
        ///     Add custom log message to log buffer
        /// </summary>
        /// <param name="message">Logmessage</param>
        public void AddLogmessage(LogMessage message)
        {
            s_instance.AddBuffer(message);
            AddToRtb(message);
        }

        private void AddToRtb(LogMessage message)
        {
            if (Program.Mainform == null || Program.Mainform.richTextBox1 == null)
                return;

            Debug.Write(message.Message + Environment.NewLine);

            try
            {
                if (Program.Mainform.InvokeRequired && !Program.Mainform.IsDisposed)
                {
                    Program.Mainform.Invoke(new Action(() =>
                       {
                           var rtb = Program.Mainform.richTextBox1;
                           //var font = new Font("Tahoma", 8, FontStyle.Regular);
                           //rtb.SelectionFont = font;
                           //rtb.SelectionColor = message.Color;
                           var text = $"{LoglevelChar(message.Loglevel)} [{message.TimeStamp}] {message.Message}";
                           rtb.AppendText(text + Environment.NewLine);
                       }));
                }
            }
            catch (Exception ex)
            {
                Instance.Write("Exception in addToRTB: {0}", ex);
                // Failed! do nothing
            }
        }

        private void AddBuffer(LogMessage logmessage)
        {
            lock (s_bufferLock)
            {
                _buffer.Add(logmessage);
                if (_buffer.Count > 3)
                    ClearBuffer();
            }
        }

        public void ClearBuffer()
        {
            lock (s_bufferLock)
            {
                if (!_canLog)
                    return;
                _canLog = false;

                // Write buffer to file
                using (var writer = new StreamWriter(_logfile, true))
                {
                    foreach (var message in _buffer)
                    {
                        writer.WriteLine("{0} [{1}] {2}", LoglevelChar(message.Loglevel), message.TimeStamp, message.Message);
                    }
                }
                _buffer.Clear();
                _canLog = true;
            }
        }

        /// <summary>
        ///     Get Loglevel char
        /// </summary>
        /// <param name="loglevel">Loglevel</param>
        /// <returns>char</returns>
        public char LoglevelChar(Loglevel loglevel)
        {
            switch (loglevel)
            {
                case Loglevel.Debug:
                    return 'D';
                case Loglevel.Verbose:
                    return 'V';
                default:
                    return 'N';
            }
        }
    }

    public class LogMessage : IDisposable
    {
        public Color Color;
        public Loglevel Loglevel;
        public string Message;

        public LogMessage(Color color, Loglevel loglevel, string message, params object[] args)
        {
            TimeStamp = DateTime.Now;
            Loglevel = Loglevel.Normal;
        }

        public LogMessage()
        {
            Color = Color.Black;
            TimeStamp = DateTime.Now;
            Loglevel = Loglevel.Normal;
        }

        public DateTime TimeStamp { get; private set; }

        public void Dispose()
        {
            Message = null;
        }
    }
}
