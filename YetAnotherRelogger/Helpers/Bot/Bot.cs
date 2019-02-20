using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Serilog;
using YetAnotherRelogger.Helpers.Attributes;
using YetAnotherRelogger.Helpers.Stats;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class Bot : INotifyPropertyChanged, ICloneable
    {
        private static readonly ILogger s_logger = Logger.Instance.GetLogger<Bot>();

        [XmlIgnore]
        private AntiIdleClass _antiIdle;
        [XmlIgnore]
        private Demonbuddy _demonbuddy;
        [XmlIgnore]
        [NoCopy]
        private string _demonbuddyPid;
        [XmlIgnore]
        private Diablo _diablo;
        [XmlIgnore]
        [NoCopy]
        private bool _isStandby;
        [XmlIgnore]
        [NoCopy]
        private string _runningtime;
        [XmlIgnore]
        [NoCopy]
        private DateTime _standbyTime;

        [XmlIgnore]
        [NoCopy]
        private string _status;

        public Bot()
        {
            Name = string.Empty;
            Description = string.Empty;
            AntiIdle = new AntiIdleClass();
            ChartStats = new ChartStats();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public Demonbuddy Demonbuddy
        {
            get => _demonbuddy;
            set
            {
                var db = value;
                db.Parent = this;
                _demonbuddy = db;
            }
        }

        public Diablo Diablo
        {
            get => _diablo;
            set
            {
                var d = value;
                d.Parent = this;
                _diablo = d;
            }
        }

        [XmlIgnore]
        public AntiIdleClass AntiIdle
        {
            get => _antiIdle;
            set
            {
                var ai = value;
                ai.Parent = this;
                _antiIdle = ai;
            }
        }

        public WeekSchedule Week { get; set; }
        public ProfileScheduleClass ProfileSchedule { get; set; }

        [XmlIgnore]
        [NoCopy]
        public bool IsStarted { get; set; }

        [XmlIgnore]
        [NoCopy]
        public bool IsRunning { get; set; }

        // Standby to try again at a later moment

        [XmlIgnore]
        [NoCopy]
        public bool IsStandby
        {
            get
            {
                // Increase retry count by 15 mins with a max of 1 hour
                if (_isStandby &&
                    General.DateSubtract(_standbyTime) > 900 * (AntiIdle.InitAttempts > 4 ? 4 : AntiIdle.InitAttempts))
                {
                    _isStandby = false;
                    _diablo.Start();
                    _demonbuddy.Start();
                }
                return _isStandby;
            }
            private set
            {
                _standbyTime = DateTime.UtcNow;
                _isStandby = value;
            }
        }

        [XmlIgnore]
        [NoCopy]
        public string Status
        {
            get => _status;
            set => SetField(ref _status, value, "Status");
        }

        [XmlIgnore]
        [NoCopy]
        public DateTime StartTime { get; set; }

        [XmlIgnore]
        [NoCopy]
        public string RunningTime
        {
            get => _runningtime;
            set => SetField(ref _runningtime, value, "RunningTime");
        }

        [XmlIgnore]
        [NoCopy]
        public ChartStats ChartStats { get; set; }

        [XmlIgnore]
        [NoCopy]
        public string DemonbuddyPid
        {
            get => _demonbuddyPid;
            set => SetField(ref _demonbuddyPid, value, "DemonbuddyPid");
        }

        #region Advanced Options Variables

        // Windows User
        public bool UseWindowsUser { get; set; }
        public bool CreateWindowsUser { get; set; }
        public string WindowsUserName { get; set; }
        public string WindowsUserPassword { get; set; }

        // Diablo Clone
        public bool UseDiabloClone { get; set; }
        public string DiabloCloneLocation { get; set; }

        // D3Prefs
        public string D3PrefsLocation { get; set; }

        #endregion

        public object Clone()
        {
            var clone = new Bot
            {
                AntiIdle = AntiIdle.Copy(),
                ChartStats = ChartStats.Copy(),
                CreateWindowsUser = CreateWindowsUser.Copy(),
                D3PrefsLocation = D3PrefsLocation.Copy(),
                Demonbuddy = Demonbuddy.Copy(),
                Description = Description.Copy(),
                Diablo = Diablo.Copy(),
                DiabloCloneLocation = DiabloCloneLocation.Copy(),
                Name = Name.Copy(),
                ProfileSchedule = new ProfileScheduleClass(),
                UseDiabloClone = UseDiabloClone.Copy(),
                UseWindowsUser = UseWindowsUser.Copy(),
                Week = Week.Copy(),
                WindowsUserName = WindowsUserName.Copy(),
                WindowsUserPassword = WindowsUserPassword.Copy()
            };

            foreach (var profile in ProfileSchedule.Profiles)
            {
                clone.ProfileSchedule.Profiles.Add(profile);
            }
            return clone;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                if (Program.Mainform == null)
                    return;
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        var handler = PropertyChanged;
                        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }
                    catch (Exception ex)
                    {
                        s_logger.Warning(ex, "Exception during OnPropertyChanged action");
                    }
                }));
            }
            catch (Exception ex)
            {
                s_logger.Warning(ex, "Exception during OnPropertyChanged");
            }
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void Start(bool force = false)
        {
            AntiIdle.Reset(freshstart: true);
            IsStarted = true;
            IsStandby = false;
            Week.ForceStart = force;
            Status = (force ? "Forced start" : "Started");
            if (force)
                s_logger.Warning("Forced to start! ");
        }

        public void Stop()
        {
            s_logger.Information("Stopping");
            Status = "Stopped";
            IsStarted = false;
            IsRunning = false;
            IsStandby = false;
            _demonbuddy.Stop();
            _diablo.Stop();
        }

        public void Standby()
        {
            s_logger.Information("Standby");
            Status = "Standby";
            IsStandby = true;
            _diablo.Stop();
            _demonbuddy.Stop();
        }

        public void Restart()
        {
            s_logger.Information("Restarting");
            Status = "Restarting";
            AntiIdle.FixAttempts = 0;
            _demonbuddy.Stop();
            _diablo.Stop();
        }

        public void KillDemonbuddy()
        {
            s_logger.Information("Killing Demonbuddy");
            _demonbuddy.Stop(true);
        }

        public void KillDiablo()
        {
            s_logger.Information("Killing Diablo");
            _diablo.Stop();
        }
    }
}
