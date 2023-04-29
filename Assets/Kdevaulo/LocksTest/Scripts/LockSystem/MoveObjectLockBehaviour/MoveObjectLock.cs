using System;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    public class MoveObjectLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly MoveObjectLockView _lockView;

        private readonly float _userMovingSpeed;

        private readonly Timer _lockLoadTimer;

        private readonly ScaleFiller _scaleFiller;

        private readonly float _correctAreaDistance;

        private readonly float _maxAreaDistance;

        private readonly Vector3 _lockPosition;

        private readonly MoveObjectLockSoundPlayer _soundPlayer;

        private bool _canMoveObject;

        public MoveObjectLock(MoveObjectLockView lockView)
        {
            _lockView = lockView;
            _userMovingSpeed = _lockView.UserMovingSpeed;
            _lockLoadTimer = lockView.LockLoadTimer;

            _correctAreaDistance = lockView.CorrectAreaRadius;
            _maxAreaDistance = lockView.MaxAreaRadius;

            _lockPosition = lockView.LockContainerPosition;

            _soundPlayer = lockView.SoundPlayer;

            _scaleFiller = new ScaleFiller(_lockView.StartFillPoint, _lockView.EndFillPoint, _lockView.FillStep);
        }

        void ILock.Initialize()
        {
            _lockView.SetRandomObject();

            _lockView.ItemMoveCalled += OnItemMoveCalled;

            _scaleFiller.MaxReached += OnScaleFilled;
            _scaleFiller.MinReached += OnScaleUnFilled;

            _lockLoadTimer.Elapsed += HandleLoadTimerElapsed;
            _lockLoadTimer.StartTimer(_lockView.FillTickSeconds);

            _canMoveObject = true;
        }

        void ILock.Dispose()
        {
            UnsubscribeEvents();
        }

        private void OnItemMoveCalled(Vector2 offset)
        {
            if (_canMoveObject)
            {
                var targetPosition = CalculatePosition(offset);
                _lockView.SetObjectContainerLocalPosition(targetPosition);
            }
        }

        private Vector3 CalculatePosition(Vector2 offset)
        {
            var movingContainerPosition = _lockView.MovingContainerPosition;

            var targetPosition = movingContainerPosition + (Vector3) offset * _userMovingSpeed;

            return ClampPositionByCircle(movingContainerPosition, targetPosition);
        }

        private Vector3 ClampPositionByCircle(Vector3 lastPosition, Vector3 targetPosition)
        {
            return Vector3.Distance(_lockPosition, targetPosition) > _maxAreaDistance ? lastPosition : targetPosition;
        }

        private void OnScaleFilled()
        {
            DisableObjectMoving();

            StopTimer();

            _soundPlayer.PlayOpenSound();

            _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
        }

        private void OnScaleUnFilled()
        {
            DisableObjectMoving();

            StopTimer();

            _soundPlayer.PlayBreakSound();
        }

        private void DisableObjectMoving()
        {
            _canMoveObject = false;
        }

        private void StopTimer()
        {
            _lockLoadTimer.StopTimer();
        }

        private void HandleLoadTimerElapsed()
        {
            if (IsObjectInArea())
            {
                IncreaseFiller();
            }
            else
            {
                DecreaseFiller();
            }
        }

        private bool IsObjectInArea()
        {
            var distance = Vector3.Distance(_lockView.MovingContainerPosition, _lockPosition);

            return distance <= _correctAreaDistance;
        }

        private void IncreaseFiller()
        {
            _lockView.SetCorrectFillerColor();

            var targetPoint = _scaleFiller.GetIncreasePoint();

            SetFillerPosition(targetPoint);
        }

        private void DecreaseFiller()
        {
            _lockView.SetWrongFillerColor();

            var targetPoint = _scaleFiller.GetDecreasePoint();

            SetFillerPosition(targetPoint);
        }

        private void SetFillerPosition(Vector2 targetPoint)
        {
            _lockView.SetFillerPosition(targetPoint);
        }

        private void UnsubscribeEvents()
        {
            _scaleFiller.MaxReached -= OnScaleFilled;
            _scaleFiller.MinReached -= OnScaleUnFilled;

            _lockLoadTimer.Elapsed -= HandleLoadTimerElapsed;
        }
    }
}