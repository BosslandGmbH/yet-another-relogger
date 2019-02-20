using System;
using System.Drawing;
using System.Linq;
using Serilog;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Enums;

namespace YetAnotherRelogger.Helpers
{
    public class DebugHelper
    {
        private static readonly ILogger s_logger = Logger.Instance.GetLogger<DebugHelper>();
        public static void Exception(Exception exception, Loglevel level = Loglevel.Debug)
        {
            s_logger.Error(exception, "Exception logged.");
        }

        public static void Write(string message, string caller, params object[] args)
        {
            s_logger.Information($"[{{caller}}] {message}", new object[] { caller }.Union(args).ToArray());
        }

        public static void Write(Bot.Bot bot, string message, string caller, params object[] args)
        {
            s_logger.ForContext("Bot", bot.Name).Information($"[{{caller}}] {message}", new object[] { caller }.Union(args).ToArray());
        }

        public static void Write(Bot.Bot bot, string message, params object[] args)
        {
            s_logger.ForContext("Bot", bot.Name).Information(message, args);
        }

        public static void Write(string message)
        {
            s_logger.Information(message);
        }
    }
}
