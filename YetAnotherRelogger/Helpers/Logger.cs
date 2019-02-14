using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Bot;

namespace YetAnotherRelogger.Helpers
{
    public sealed class Logger
    {
        #region singleton
        public static Logger Instance { get; } = new Logger();
        private Logger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(new WinFormsRtfAppender(Program.Mainform?.LogUpdateTimer, Program.Mainform?.richTextBox1, "{Timestamp:HH:mm:ss.fff} [{PID}] [{Level:u3}] {Message:l}{NewLine}{Exception}"), LogEventLevel.Verbose)
                .WriteTo.File($@"Logs\YAR-.txt",
                    rollingInterval: RollingInterval.Hour,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{PID}] [{Level:u3}] {Properties:j} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            _logger = Log.ForContext("PID", Process.GetCurrentProcess().Id).ForContext<Logger>();
        }
        #endregion

        private readonly ILogger _logger;
        public ILogger GetLogger<T>()
        {
            return _logger.ForContext<T>();
        }

        public string LogDirectory => Path.GetFullPath("Logs");

        /// <summary>
        ///     Write log message for active bot
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write(string format, params object[] args)
        {
            if (Relogger.Instance.CurrentBot != null)
                _logger.ForContext("BotName", Relogger.Instance.CurrentBot.Name).Information(format, args);
            else
                _logger.Information(format, args);
        }

        /// <summary>
        ///     Write Log message for specific bot
        /// </summary>
        /// <param name="bot">BotClass</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write(Bot.Bot bot, string format, params object[] args)
        {
            if (bot == null)
            {
                _logger.Information(format, args);
                return;
            }
            _logger.ForContext("BotName", bot.Name).Information(format, args);
        }

        /// <summary>
        ///     Write global log message
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteGlobal(string format, params object[] args)
        {
            _logger.Information(format, args);
        }

        #region Nested type: WpfRtfAppender

        public class WinFormsRtfAppender : ILogEventSink
        {
            private readonly Color _info;
            private readonly Color _debug;
            private readonly Color _error;
            private readonly Color _warn;
            private readonly Color _verbose;
            private readonly Color _fatal;

            private readonly RichTextBox _rtb;
            private int _logCount;
            private readonly ConcurrentQueue<LogEvent> _logMessages = new ConcurrentQueue<LogEvent>();

            private readonly ITextFormatter _formatter;

            public WinFormsRtfAppender(Timer logUpdateTimer, RichTextBox rtb, string outputTemplate = "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}", IFormatProvider formatProvider = null)
            {
                _rtb = rtb;

                _verbose = Color.SlateGray;
                _debug = Color.DarkGray;
                _info = Color.White;
                _warn = Color.Orange;
                _error = Color.Red;
                _fatal = Color.DarkRed;

                _formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

                logUpdateTimer.Tick += HandleTimerCallback;
            }

            private void HandleTimerCallback(object sender, EventArgs e)
            {
                Color curColor = _info;
                var w = new StringWriter();
                StringBuilder curChunk = w.GetStringBuilder();

                void EmitChunk(StringBuilder sb, Color col)
                {
                    _rtb.SelectionColor = col;
                    _rtb.AppendText(sb.ToString());
                }

                if (_logCount >= 2000)
                {
                    _rtb.Clear();
                    _logCount = 0;
                }

                while (_logMessages.TryDequeue(out LogEvent loggingEvent))
                {
                    Color color = _info;
                    switch (loggingEvent.Level)
                    {
                        case LogEventLevel.Verbose:
                            color = _verbose;
                            break;
                        case LogEventLevel.Debug:
                            color = _debug;
                            break;
                        case LogEventLevel.Fatal:
                            color = _fatal;
                            break;
                        case LogEventLevel.Error:
                            color = _error;
                            break;
                        case LogEventLevel.Warning:
                            color = _warn;
                            break;
                    }

                    if (!Equals(color, curColor))
                    {
                        EmitChunk(curChunk, curColor);
                        curChunk.Clear();
                        curColor = color;
                    }

                    _formatter.Format(loggingEvent, w);
                    _logCount++;
                }

                if (curChunk.Length != 0)
                {
                    EmitChunk(curChunk, curColor);
                    _logCount++;
                }
            }

            /// <summary> Event queue for all listeners interested in onLogging events.</summary>
            public void Emit(LogEvent logEvent)
            {
                _logMessages.Enqueue(logEvent);
            }
        }

        #endregion
    }
}
