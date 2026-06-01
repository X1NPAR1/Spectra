using System;
using System.Windows.Forms;

namespace Spectra.common
{
    // Time-based automatic vibrance scheduler ("night mode").
    // When enabled, applies a day level during day hours and a night level
    // during night hours, switching automatically at the configured times.
    public class ScheduleManager : IDisposable
    {
        private readonly Timer _timer;
        private bool _lastWasNight;
        private bool _initialized;

        public bool   Enabled   { get; set; }
        public int    DayLevel  { get; set; }
        public int    NightLevel{ get; set; }
        public TimeSpan DayStart   { get; set; } = new TimeSpan(8, 0, 0);   // 08:00
        public TimeSpan NightStart { get; set; } = new TimeSpan(20, 0, 0);  // 20:00

        // Raised with the level that should be applied when the period changes.
        public event Action<int> ApplyLevel;

        public ScheduleManager()
        {
            _timer = new Timer { Interval = 30000 }; // check every 30s
            _timer.Tick += (s, e) => Evaluate();
            _timer.Start();
        }

        public bool IsNightNow()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            // Night spans across midnight when NightStart > DayStart (the usual case).
            if (NightStart > DayStart)
                return now >= NightStart || now < DayStart;
            // Inverted config (day spans midnight).
            return now >= NightStart && now < DayStart;
        }

        // Forces an immediate evaluation (e.g. right after the user toggles the feature).
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
