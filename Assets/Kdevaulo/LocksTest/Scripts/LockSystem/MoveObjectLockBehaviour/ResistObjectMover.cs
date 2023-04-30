using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    public class ResistObjectMover
    {
        private readonly Vector2 _speedRange;

        private readonly Vector2 _delayRange;

        private readonly Vector2[] _directions;

        private readonly Timer _timer;

        private Vector2 _currentDirection;

        private float _currentSpeed;

        /// <param name="speedRange">Speed range for random movement speed [inclusive, inclusive]</param>
        /// <param name="delayRange">Delay range for random delay between direction change [inclusive, inclusive]</param>
        public ResistObjectMover(Timer timer, Vector2 speedRange, Vector2 delayRange, Vector2[] directions)
        {
            _speedRange = speedRange;
            _delayRange = delayRange;
            _directions = directions;

            _timer = timer;
            _timer.Elapsed += HandleTimerElapsed;

            SetRandomMoveParameters();

            RestartTimer();
        }

        public Vector2 GetNextPosition()
        {
            return _currentDirection * _currentSpeed;
        }

        public void Dispose()
        {
            _timer.StopTimer();
            _timer.Elapsed -= HandleTimerElapsed;
        }

        private void HandleTimerElapsed()
        {
            SetRandomMoveParameters();
            RestartTimer();
        }

        private void SetRandomMoveParameters()
        {
            _currentDirection = _directions[Random.Range(0, _directions.Length)];

            _currentSpeed = Random.Range(_speedRange.x, _speedRange.y);
        }

        private void RestartTimer()
        {
            _timer.StopTimer();

            _timer.StartTimer(GetRandomDelay());
        }

        private float GetRandomDelay()
        {
            return Random.Range(_delayRange.x, _delayRange.y);
        }
    }
}