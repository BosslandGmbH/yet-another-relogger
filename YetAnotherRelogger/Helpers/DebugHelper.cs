using System;
using System.Drawing;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Enums;

namespace YetAnotherRelogger.Helpers
{
    public static class DebugHelper
    {
        public static void Exception(Exception exception, Loglevel level = Loglevel.Debug)
        {
            Logger.Instance.AddLogmessage(
                new LogMessage
                {
                    Color = Color.Red,
                    Loglevel = level,
                    Message = $"Exception: [{exception.Message}]{Environment.NewLine}{exception}"
                });
        }

        public static void Write(string message, string caller, params object[] args)
        {
            Logger.Instance.AddLogmessage(
                new LogMessage
                {
                    Color = Color.DarkGreen,
                    Loglevel = Loglevel.Verbose,
                    Message = $"[{caller}] {string.Format(message, args)}"
                });
        }

        public static void Write(BotClass bot, string message, string caller, params object[] args)
        {
            Logger.Instance.AddLogmessage(
                new LogMessage
                {
                    Color = Color.DarkGreen,
                    Loglevel = Loglevel.Verbose,
                    Message = $"<{bot.Name}> [{caller}] {string.Format(message, args)}"
                });
        }

        public static void Write(BotClass bot, string message, params object[] args)
        {
            Logger.Instance.AddLogmessage(
                new LogMessage
                {
                    Color = Color.DarkGreen,
                    Loglevel = Loglevel.Verbose,
                    Message = $"<{bot.Name}> {string.Format(message, args)}"
                });
        }

        public static void Write(string message)
        {
            Logger.Instance.AddLogmessage(
                new LogMessage
                {
                    Color = Color.DarkGreen,
                    Loglevel = Loglevel.Verbose,
                    Message = message
                });
        }
    }
}
