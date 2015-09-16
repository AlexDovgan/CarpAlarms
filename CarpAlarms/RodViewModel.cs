using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Scheduler;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Data;


namespace CarpAlarms
{
    public class IntToDouble : IValueConverter
    {

        #region IValueConverter Members

        // Define the Convert method to change a DateTime object to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter,    System.Globalization.CultureInfo culture)
        {
            
            return System.Convert.ToDouble(value);

        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value); ;
        }

        #endregion
    }

    class DummyDisabled : ICommand
    {
        Action _action;
        public DummyDisabled ()
        {
            
        }

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            
        }
    }
    class ActionCommand : ICommand
    {
        Action _action;
        public ActionCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
    class RodViewModel : INotifyPropertyChanged
    {
        protected ScheduledAction alarm;
        int _alarmTime;
        public RodViewModel(string rodName, Brush color)
        {
            RodName = rodName;
            Color = color;
            AlarmTime = 2;
            SetAlarmCommand = new ActionCommand(new Action(SetAlarm));
            alarm = ScheduledActionService.Find(rodName);
            if (alarm == null)
                ResetAlarmCommand = new DummyDisabled();
            else
                ResetAlarmCommand = new ActionCommand(ResetAlarm);
            DispatcherTimer newTimer = new DispatcherTimer();
            newTimer.Interval = TimeSpan.FromSeconds(1);
            newTimer.Tick += OnTimerTick;
            newTimer.Start();
        }

        void OnTimerTick(Object sender, EventArgs args)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ElapsedTime"));
            }
        }
        
        public Brush Color
        {
            get;
            protected set;

        }
        public ICommand ResetAlarmCommand
        {
            get;
            private set;
        }
        public ICommand SetAlarmCommand
        {
            get;
            private set;
        }
        public String ElapsedTime
        {
            get
            {
                if (alarm != null && alarm.BeginTime > DateTime.Now)
                    return (alarm.BeginTime - DateTime.Now).ToString(@"hh\:mm\:ss");
                else
                {

                    ResetAlarmCommand = new DummyDisabled();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ResetAlarmCommand"));
                    }
                    return new TimeSpan().ToString(@"hh\:mm\:ss");
                }
            }
            protected set
            {

            }
        }

        public int AlarmTime
        {
            get
            {

                return _alarmTime;
            }
            set
            {
                _alarmTime = value;
                if (PropertyChanged!=null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AlarmTime"));
            }
        }
        public string RodName
        {
            get;
            protected set;
        }
        public void OnRodExpired()
        {
            if (alarm != null)
            {
                ScheduledActionService.Remove(alarm.Name);
                alarm = null;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ElapsedTime"));
                }
            }
        }
        void ResetAlarm()
        {
            this.alarm = ScheduledActionService.Find(RodName);
            if (this.alarm != null)
            {
                ScheduledActionService.Remove(this.alarm.Name);
                this.alarm = null;
            }
        }
        void SetAlarm()
        {
            MessageBoxResult resul=MessageBox.Show(String.Format("Do you whant reset alarm on:{0} hours", AlarmTime), "Reset Alarm", MessageBoxButton.OKCancel);
            if (resul == MessageBoxResult.Cancel)
                return;
            this.alarm = ScheduledActionService.Find(RodName);
            if (this.alarm != null)
            {
                ScheduledActionService.Remove(this.alarm.Name);
                this.alarm = null;
            }

            

            
            Alarm alarm;
            alarm = new Alarm(RodName);
            // NOTE: setting the Title property is not supported for alarms
            //alarm.Title = this.txtTitle.Text;
            alarm.Content = RodName + " timer expired";
            double seconds = AlarmTime *3600;

            //NOTE: the value of BeginTime must be after the current time
            //set the BeginTime time property in order to specify when the alarm should be shown
            alarm.BeginTime = DateTime.Now.AddSeconds(seconds);

            // NOTE: ExpirationTime must be after BeginTime
            // the value of the ExpirationTime property specifies when the schedule of the alarm expires
            // very useful for recurring alarms, ex:
            // show alarm every day at 5PM but stop after 10 days from now
            alarm.ExpirationTime = alarm.BeginTime.AddSeconds(5.0);

            alarm.RecurrenceType = RecurrenceInterval.None;
            alarm.Sound = new Uri("/Ringtones/Alarm.mp3", UriKind.Relative);
            ScheduledActionService.Add(alarm);
            this.alarm = alarm;
            ResetAlarmCommand = new ActionCommand(new Action(ResetAlarm));
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ElapsedTime"));
                PropertyChanged(this, new PropertyChangedEventArgs("ResetAlarmCommand"));
            }
            

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}


