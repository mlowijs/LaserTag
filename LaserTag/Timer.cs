using System.Threading;

namespace LaserTag
{
    class Timer
    {
        private System.Threading.Timer _timer;
        private int _dueTime, _period;
        private bool _enabled;

        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                if (value && !_enabled)
                {
                    _enabled = true;
                    Start();
                }
                else if (!value && _enabled)
                {
                    _enabled = false;
                    Stop();
                }
            }
        }

        public Timer(TimerCallback calback, int dueTime, int period)
        {
            _dueTime = dueTime;
            _period = period;

            _timer = new System.Threading.Timer(calback, null, Timeout.Infinite, Timeout.Infinite);
        }


        private void Start()
        {
            _timer.Change(_dueTime, _period);
        }

        private void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
