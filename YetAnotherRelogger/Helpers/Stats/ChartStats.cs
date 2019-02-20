using System;
using System.Diagnostics;
using YetAnotherRelogger.Helpers.Bot;

namespace YetAnotherRelogger.Helpers.Stats
{
    public class ChartStats
    {
        public Gold GoldStats;

        #region Gold Class

        public class Gold
        {
            public DateTime StartTime;

            public Gold()
            {
                Reset();
            }

            public DateTime LastGainTime { get; private set; }

            public double Hours => DateTime.UtcNow.Subtract(StartTime).TotalSeconds/3600;

            public double GoldPerHour
            {
                get
                {
                    var gph = (LastCoinage - StartCoinage)/Hours;
                    return double.IsNaN(gph) ? 0 : gph;
                }
            }

            public long LastGain { get; private set; }
            public long TotalGain { get; private set; }
            public long StartCoinage { get; private set; }
            public long LastCoinage { get; private set; }

            public void Update(Bot.Bot bot)
            {
                var coinage = bot.AntiIdle.Stats.Coinage;
                if (coinage > 0)
                {
                    LastGain = coinage - LastCoinage;
                    LastCoinage = coinage;
                    if (coinage < StartCoinage)
                    {
                        DebugHelper.Write(bot, "Reset! (Current coinage is below start coinage)", "GoldPerHour");
                        StartCoinage = 0;
                        TotalGain = 0;
                    }
                    if (StartCoinage <= 0)
                    {
                        DebugHelper.Write(bot, "New start coinage: {0:N0}", "GoldPerHour", coinage);
                        StartCoinage = coinage;
                        StartTime = DateTime.UtcNow;
                    }
                    else
                    {
                        if (LastGain > 0)
                            LastGainTime = DateTime.UtcNow;
                        else if (DateTime.UtcNow.Subtract(LastGainTime).TotalMinutes > 15)
                        {
                            DebugHelper.Write(bot, "Reset! No gold collected for 15 mins", "GoldPerHour");
                            Reset();
                        }
                        TotalGain += LastGain;
                        Debug.WriteLine("<{0}> LastGain: {1:N0}, TotalGain: {2:N0}, GPH: {3:N0}", bot.Name, LastGain,
                            TotalGain, GoldPerHour);
                    }
                }
            }

            public void Reset()
            {
                StartTime = DateTime.UtcNow;
                LastGainTime = DateTime.UtcNow;
                LastCoinage = 0;
                LastGain = 0;
                TotalGain = 0;
                StartCoinage = 0;
                LastCoinage = 0;
            }
        }

        #endregion

        public ChartStats()
        {
            GoldStats = new Gold();
        }
    }
}