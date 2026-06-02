using System;
using System.Windows.Forms;

namespace Spectra.common
{
    public class ScheduleManager : IDisposable
    {
        private readonly Timer _timer;
        private bool _lastWasNight;
        private bool _initialized;

        public bool    Enabled    { get; set; }
        public int     DayLevel   { get; set; }
        public int     NightLevel { get; set; }
        public TimeSpan DayStart  { get; set; } = new TimeSpan(8,  0, 0);
        public TimeSpan NightStart{ get; set; } = new TimeSpan(20, 0, 0);

        public event Action<int> ApplyLevel;

        public ScheduleManager()
        {
            _timer = new Timer { Interval = 30000 };
            _timer.Tick += (s, e) => Evaluate();
            _timer.Start();
        }

        public bool IsNightNow()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (NightStart > DayStart)
                return now >= NightStart || now < DayStart;
            return now >= NightStart && now < DayStart;
        }

        public void EvaluateNow()
        {
            _initialized = false;
            Evaluate();
        }

        private void Evaluate()
        {
            if (!Enabled) return;
            bool night = IsNightNow();
            if (_initialized && night == _lastWasNight) return;

            _lastWasNight = night;
            _initialized  = true;
            ApplyLevel?.Invoke(night ? NightLevel : DayLevel);
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
