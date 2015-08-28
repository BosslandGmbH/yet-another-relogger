using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using YetAnotherRelogger.Helpers.Stats;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class BotClass : INotifyPropertyChanged, ICloneable
    {
        [XmlIgnore]
        private AntiIdleClass _antiIdle;
        [XmlIgnore]
        private DemonbuddyClass _demonbuddy;
        [XmlIgnore]
        [NoCopy]
        private string _demonbuddyPid;
        [XmlIgnore]
        private DiabloClass _diablo;
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

        public BotClass()
        {
            Name = string.Empty;
            Description = String.Empty;
            AntiIdle = new AntiIdleClass();
            ChartStats = new ChartStats();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public DemonbuddyClass Demonbuddy
        {
            get { return _demonbuddy; }
            set
            {
                DemonbuddyClass db = value;
                db.Parent = this;
                _demonbuddy = db;
            }
        }

        public DiabloClass Diablo
        {
            get { return _diablo; }
            set
            {
                DiabloClass d = value;
                d.Parent = this;
                _diablo = d;
            }
        }

        [XmlIgnore]
        public AntiIdleClass AntiIdle
        {
            get { return _antiIdle; }
            set
            {
                AntiIdleClass ai = value;
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
            get { return _status; }
            set { SetField(ref _status, value, "Status"); }
        }

        [XmlIgnore]
        [NoCopy]
        public DateTime StartTime { get; set; }

        [XmlIgnore]
        [NoCopy]
        public string RunningTime
        {
            get { return _runningtime; }
            set { SetField(ref _runningtime, value, "RunningTime"); }
        }

        [XmlIgnore]
        [NoCopy]
        public ChartStats ChartStats { get; set; }

        [XmlIgnore]
        [NoCopy]
        public string DemonbuddyPid
        {
            get { return _demonbuddyPid; }
            set { SetField(ref _demonbuddyPid, value, "DemonbuddyPid"); }
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
            var clone = new BotClass
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
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null)
                            handler(this, new PropertyChangedEventArgs(propertyName));
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
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
                Logger.Instance.Write(this, "Forced to start! ");
        }

        public void Stop()
        {
            Logger.Instance.Write(this, "Stopping");
            Status = "Stopped";
            IsStarted = false;
            IsRunning = false;
            IsStandby = false;
            _demonbuddy.Stop();
            _diablo.Stop();
        }

        public void Standby()
        {
            Logger.Instance.Write(this, "Standby!");
            Status = "Standby";
            IsStandby = true;
            _diablo.Stop();
            _demonbuddy.Stop();
        }

        public void Restart()
        {
            Logger.Instance.Write(this, "Restarting...");
            Status = "Restarting";
            AntiIdle.FixAttempts = 0;
            _demonbuddy.Stop();
            _diablo.Stop();
        }

        public void KillDB()
        {
            Logger.Instance.Write(this, "Killing Demonbuddy");
            _demonbuddy.Stop(true);
        }

        public void KillDiablo()
        {
            Logger.Instance.Write(this, "Killing Diablo");
            _diablo.Stop();

        }
    }
}