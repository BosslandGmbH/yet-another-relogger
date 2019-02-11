using System;
using System.Diagnostics;
using System.Xml.Serialization;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class WeekSchedule
    {
        [XmlIgnore] public bool ForceStart;
        public DaySchedule Friday;
        public DaySchedule Monday;
        public DaySchedule Saturday;
        public DaySchedule Sunday;
        public DaySchedule Thursday;
        public DaySchedule Tuesday;
        public DaySchedule Wednesday;

        private int _currentRandom;

        public WeekSchedule()
        {
            Monday = new DaySchedule();
            Tuesday = new DaySchedule();
            Wednesday = new DaySchedule();
            Thursday = new DaySchedule();
            Friday = new DaySchedule();
            Saturday = new DaySchedule();
            Sunday = new DaySchedule();
            Shuffle = false;
        }

        public int MinRandom { get; set; }
        public int MaxRandom { get; set; }
        public bool Shuffle { get; set; }

        public bool ShouldRun(bool isRunning)
        {
            var day = (int) DateTime.Now.DayOfWeek; // Get number for current day of the week 
            day = (day == 0 ? 7 : day); // day fix sunday is 7
            var currentDay = GetDaySchedule(day);
            //DaySchedule nextDay = GetDaySchedule((day == 7 ? 1 : day + 1));
            var currentHour = ClockFix(Convert.ToInt32(DateTime.Now.ToString("HH")));
            var prevHour = ClockFix((currentHour - 1 != -1 ? currentHour - 1 : 0));

            var thisHour = currentDay.Hours[currentHour];

            Debug.WriteLine("isRunning:{0} thishour:{1} day:{2}", isRunning, thisHour, day);
            if (isRunning)
            {
                // Check if we should stop

                if (!thisHour && DateTime.Now.Minute >= _currentRandom)
                {
                    GenerateNewRandom();
                    return ForceStart;
                }

                if (ForceStart)
                {
                    // Disable ForceStart
                    ForceStart = false;
                    Logger.Instance.Write("Continue normal schedule");
                }

                return true;
            }
            // Check if we need to start
            if (thisHour && (DateTime.Now.Minute >= _currentRandom || currentDay.Hours[prevHour]))
            {
                GenerateNewRandom();

                if (ForceStart)
                {
                    // Disable ForceStart
                    ForceStart = false;
                    Logger.Instance.Write("Continue normal schedule");
                }

                return true;
            }
            return ForceStart;
        }

        public void NextSchedule(bool start)
        {
            var day = (int) DateTime.Now.DayOfWeek; // Get number for current day of the week 
            day = (day == 0 ? 7 : day); // day fix sunday is 7
            var currentHour = ClockFix(Convert.ToInt32(DateTime.Now.ToString("HH")));

            var date = DateTime.Now;

            var x = 1;
            var first = true;
            for (var i = day; i <= 8; i++)
            {
                Debug.WriteLine("Day: " + i);
                var currentDay = GetDaySchedule(i);
                for (var h = (first ? currentHour : 0); h < 24; h++)
                {
                    if (currentDay.Hours[h] && start)
                    {
                        Logger.Instance.Write("Next scheduled start: {0:d/M} {1}:{2}", date, h, _currentRandom);
                        return;
                    }
                    if (!currentDay.Hours[h] && !start)
                    {
                        Logger.Instance.Write("Next scheduled stop: {0:d/M} {1}:{2}", date, h, _currentRandom);
                        return;
                    }
                }
                date = date.AddDays(1);
                first = false;
                // Check if we had all days of the week
                x++;
                Debug.WriteLine("Count: " + x);
                if (x > 7)
                    break;
                if (i >= 7)
                    i = 1;
            }
        }

        private void GenerateNewRandom()
        {
            var rnd = new MersenneTwister();
            _currentRandom = rnd.Next(MinRandom, MaxRandom);
        }

        public void GenerateNewSchedule()
        {
            var n = 0; // Box number
            for (var d = 1; d <= 7; d++)
            {
                var md = GetDaySchedule(d);
                for (var h = 0; h < 24; h++)
                {
                    md.Hours[h] = Forms.Wizard.WeekSchedule.GetSchedule[n].IsEnabled;
                    n++; // increase box number
                }
            }
        }

        private static int ClockFix(int hour)
        {
// Small work around for 24 hour to 00 hour for array fix
            return (hour == 24 ? 0 : hour);
        }

        private DaySchedule GetDaySchedule(int day)
        {
            var md = new DaySchedule();
            switch (day)
            {
                case 1:
                    md = Monday;
                    break;
                case 2:
                    md = Tuesday;
                    break;
                case 3:
                    md = Wednesday;
                    break;
                case 4:
                    md = Thursday;
                    break;
                case 5:
                    md = Friday;
                    break;
                case 6:
                    md = Saturday;
                    break;
                case 7:
                    md = Sunday;
                    break;
            }
            return md;
        }
    }

    public class DaySchedule
    {
        public bool[] Hours;

        public DaySchedule()
        {
            Hours = new bool[24];
            for (var i = 0; i < 24; i++)
            {
                Hours[i] = new bool();
            }
        }
    }
}
