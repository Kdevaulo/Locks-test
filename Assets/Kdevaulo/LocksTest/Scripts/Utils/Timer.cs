using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    [AddComponentMenu(nameof(Utils) + "/" + nameof(Timer))]
    public class Timer : MonoBehaviour
    {
        public event Action Elapsed = delegate { };

        private float _interval;

        private float _secondsLeft;

        private bool _isTimerActive;

        private void Update()
        {
            if (_isTimerActive)
            {
                _secondsLeft -= Time.deltaTime;

                if (_secondsLeft <= 0)
                {
                    Elapsed.Invoke();

                    _secondsLeft = _interval;
                }
            }
        }

        /// <param name="interval">Interval between timer elapse in seconds</param>
        public void StartTimer(float interval)
        {
            _interval = interval;
            _secondsLeft = interval;

            _isTimerActive = true;
        }

        public void StopTimer()
        {
            _isTimerActive = false;
        }
    }
}