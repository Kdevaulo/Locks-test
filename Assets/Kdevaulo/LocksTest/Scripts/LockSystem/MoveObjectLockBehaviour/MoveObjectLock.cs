using System;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    public class MoveObjectLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly MoveObjectLockSoundPlayer _soundPlayer;
        private readonly ResistObjectMover _objectMover;
        private readonly MoveObjectLockView _lockView;
        private readonly ScaleFiller _scaleFiller;
        private readonly Timer _lockLoadTimer;

        private readonly float _userMovingSpeed;
        private readonly float _correctAreaDistance;
        private readonly float _maxAreaDistance;

        private readonly Vector3 _lockPosition;

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
            _objectMover = new ResistObjectMover(_lockView.ObjectMoverTimer, _lockView.ResistMovingSpeedRange,
                _lockView.BetweenChangeDirectionSecondsRange, _lockView.Directions);
        }

        void ILock.Initialize()
        {
            _lockView.SetRandomObject();

            _lockView.ItemMoveCalled += OnItemMoveCalled;

            _scaleFiller.MaxReached += OnScaleFilled;
            _scaleFiller.MinReached += OnScaleUnFilled;

            _lockLoadTimer.Elapsed += HandleLoadTimerElapsed;
            _lockLoadTimer.StartTimer(_lockView.FillTickSeconds);

            _soundPlayer.StartPlayLockSound();

            _canMoveObject = true;
        }

        void ILock.Dispose()
        {
            UnsubscribeEvents();
            _objectMover.Dispose();
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

            var resistOffset = (Vector3) _objectMover.GetNextPosition();

            return ClampPositionByCircle(movingContainerPosition, targetPosition + resistOffset);
        }

        private Vector3 ClampPositionByCircle(Vector3 lastPosition, Vector3 targetPosition)
        {
            return Vector3.Distance(_lockPosition, targetPosition) > _maxAreaDistance ? lastPosition : targetPosition;
        }

        private void OnScaleFilled()
        {
            StopLockActivity();

            _soundPlayer.PlayOpenSound();

            AnimateGameEndAsync().Forget();
        }

        private async UniTask AnimateGameEndAsync()
        {
            await _lockView.DisappearAsync();

            LockOpened.Invoke();
        }

        private void OnScaleUnFilled()
        {
            StopLockActivity();

            _soundPlayer.PlayBreakSound();
        }

        private void StopLockActivity()
        {
            DisableObjectMoving();
            StopTimer();

            _soundPlayer.StopLockSound();
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