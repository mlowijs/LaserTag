using System.Threading;

namespace LaserTag
{
    public class Timer
    {
        private System.Threading.Timer _timer;
        private int _dueTime, _period;
        private bool _started;

        public bool IsStarted
        {
            get { return _started; }
            set
            {
                if (value && !_started)
                {
                    _started = true;
                    _timer.Change(_dueTime, _period);
                }
                else if (!value && _started)
                {
                    _started = false;
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        public Timer(TimerCallback callback, int dueTime, int period)
        {
            _dueTime = dueTime;
            _period = period;

            _timer = new System.Threading.Timer(callback, null, Timeout.Infinite, Timeout.Infinite);
        }
    }
}
