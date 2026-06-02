using System;
using System.Threading;

namespace Spectra.common
{
    sealed class LevelAnimator : IDisposable
    {
        private readonly Action<int> _apply;
        private Timer  _timer;
        private double _current;
        private double _target;
        private double _step;
        private readonly object _lock = new object();

        public bool Enabled  { get; set; } = true;
        public int  Duration { get; set; } = 300;

        public LevelAnimator(Action<int> apply) { _apply = apply; }

        public void AnimateTo(int from, int to)
        {
            if (!Enabled || Duration <= 0 || from == to)
            {
                _apply(to);
                return;
            }
            lock (_lock)
            {
                _current = from;
                _target  = to;
                int ticks = Math.Max(1, Duration / 16);
                _step = (to - from) / (double)ticks;
                _timer?.Dispose();
                _timer = new Timer(Tick, null, 0, 16);
            }
        }

        private void Tick(object state)
        {
            lock (_lock)
            {
                if (_timer == null) return;
                _current += _step;
                bool done = _step >= 0 ? _current >= _target : _current <= _target;
                if (done)
                {
                    _current = _target;
                    _timer.Dispose();
                    _timer = null;
                }
                _apply((int)Math.Round(_current));
            }
        }

        public void Dispose()
        {
            lock (_lock) { _timer?.Dispose(); _timer = null; }
        }
    }
}
