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

        private static readonly Object BufferLock = 0;

        private static readonly Logger _instance = new Logger();

        static Logger()
        {
        }

        private Logger()
        {
            lock (BufferLock)
            {
                _buffer = new List<LogMessage>();
                Initialize();
            }
        }

        public static Logger Instance
        {
            get { return _instance; }
        }

        #endregion

        private readonly List<LogMessage> _buffer;
        private bool _canLog;
        private string _logfile;

        public string Logfile
        {
            get { return _logfile; }
            private set { _logfile = value; }
        }

        public string LogDirectory
        {
            get { return Path.GetDirectoryName(Logfile); }
        }

        private void Initialize()
        {
            string filename = string.Format("{0:yyyy-MM-dd HH.mm}", DateTime.Now);
            _logfile = string.Format(@"{0}\Logs\{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), filename);
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
                message.Message = string.Format("<{0}> {1}", Relogger.Instance.CurrentBot.Name,
                    string.Format(format, args));
            else
                message.Message = string.Format("[{0}] {1}", DateTime.Now, string.Format(format, args));
            _instance.AddBuffer(message);
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
            var message = new LogMessage { Message = string.Format("<{0}> {1}", bot.Name, string.Format(format, args)) };
            _instance.AddBuffer(message);
            AddToRtb(message);
        }

        /// <summary>
        ///     Write global log message
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteGlobal(string format, params object[] args)
        {
            var message = new LogMessage { Message = string.Format("{0}", string.Format(format, args)) };
            _instance.AddBuffer(message);
            AddToRtb(message);
        }

        /// <summary>
        ///     Add custom log message to log buffer
        /// </summary>
        /// <param name="message">Logmessage</param>
        public void AddLogmessage(LogMessage message)
        {
            _instance.AddBuffer(message);
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
                           RichTextBox rtb = Program.Mainform.richTextBox1;
                           //var font = new Font("Tahoma", 8, FontStyle.Regular);
                           //rtb.SelectionFont = font;
                           //rtb.SelectionColor = message.Color;
                           string text = string.Format("{0} [{1}] {2}", LoglevelChar(message.Loglevel), message.TimeStamp,
                               message.Message);
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
            lock (BufferLock)
            {
                _buffer.Add(logmessage);
                if (_buffer.Count > 3)
                    ClearBuffer();
            }
        }

        public void ClearBuffer()
        {
            lock (BufferLock)
            {
                if (!_canLog)
                    return;
                _canLog = false;

                // Write buffer to file
                using (var writer = new StreamWriter(_logfile, true))
                {
                    foreach (LogMessage message in _buffer)
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