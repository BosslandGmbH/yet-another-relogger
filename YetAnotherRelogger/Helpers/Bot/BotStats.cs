using System;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class BotStats
    {
        public int Pid { get; set; }
        public long LastRun { get; set; }
        public long LastPulse { get; set; }
        public long PluginPulse { get; set; }
        public long LastGame { get; set; }
        public bool IsPaused { get; set; }
        public bool IsRunning { get; set; }
        public bool IsInGame { get; set; }
        public bool IsLoadingWorld { get; set; }
        public long Coinage { get; set; }
        public long Experience { get; set; }

        public void Reset()
        {
            LastGame = PluginPulse = LastPulse = LastRun = DateTime.UtcNow.Ticks;
            IsPaused = IsRunning = IsInGame = false;
            Coinage = Experience = 0;
        }
    }
}
