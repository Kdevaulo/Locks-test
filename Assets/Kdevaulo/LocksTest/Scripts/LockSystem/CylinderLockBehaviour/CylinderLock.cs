using System;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour
{
    public class CylinderLock : ILock
    {
        public event Action LockOpened = delegate { };

        private const int MaxAngle = 360;

        private readonly CylinderLockView _lockView;

        private readonly float _startAngle;

        private readonly float _minMaxRotationOffset;

        private readonly float _maxAngleOffset;

        private readonly Vector2 _lockOpenRange;

        private Vector2 _minMaxOpenAngle;

        private int _currentOpenPercentage;

        private bool _currentLockKeyPressed;

        private bool _lockOpened;

        public CylinderLock(CylinderLockView lockView)
        {
            _lockView = lockView;
            _minMaxRotationOffset = lockView.MinMaxRotationOffset;
            _lockOpenRange = lockView.LockOpenRange;
            _maxAngleOffset = lockView.MaxAngleOffset;

            _startAngle = ClampAngle(lockView.StartAngle);
        }

        void ILock.Initialize()
        {
            _lockView.LockOpened += HandleLockOpened;
            _lockView.Moved += HandleLockPickMoved;
            _lockView.OpenLockPressed += HandleOpenLockKey;

            _lockView.SetToolZRotation(_startAngle);

            ChooseRandomAngle();
        }

        void ILock.Dispose()
        {
            _lockView.LockOpened -= HandleLockOpened;
            _lockView.Moved -= HandleLockPickMoved;
            _lockView.OpenLockPressed -= HandleOpenLockKey;
        }

        private void ChooseRandomAngle()
        {
            var minAngle = Random.Range(_lockOpenRange.x, _lockOpenRange.y);

            _minMaxOpenAngle = new Vector2(minAngle, minAngle + _lockView.OpenAngleOffset);
        }

        private void HandleLockOpened()
        {
            _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
        }

        private void HandleOpenLockKey(bool value)
        {
            if (_lockOpened)
            {
                return;
            }

            _currentLockKeyPressed = value;

            if (value)
            {
                _lockView.RotateKeyholeContainerToTarget(_currentOpenPercentage);
            }
            else
            {
                _lockView.RotateKeyholeContainerToDefault(_currentOpenPercentage);
            }
        }

        private void HandleLockPickMoved(float value)
        {
            if (_currentLockKeyPressed || _lockOpened)
            {
                return;
            }

            var currentAngle = _lockView.GetAngle();

            value = ValidateLockPickValue(value, currentAngle);

            _lockView.RotateToolContainer(value);

            _currentOpenPercentage = CalculateCloseToTargetPercentage(currentAngle);
        }

        private float ValidateLockPickValue(float value, float currentAngle)
        {
            if (_minMaxRotationOffset <= 0)
            {
                return value;
            }

            var maxAngle = _startAngle + _minMaxRotationOffset;
            var minAngle = _startAngle - _minMaxRotationOffset;

            var targetAngle = ClampAngle(currentAngle + value);

            if (targetAngle < minAngle)
            {
                targetAngle += MaxAngle;
            }

            if (targetAngle > maxAngle)
            {
                targetAngle -= MaxAngle;
            }

            return targetAngle >= minAngle && targetAngle <= maxAngle ? value : 0;
        }

        private int CalculateCloseToTargetPercentage(float currentAngle)
        {
            var minOpenAngle = _minMaxOpenAngle.x;
            var maxOpenAngle = _minMaxOpenAngle.y;

            float distance = 0;

            if (currentAngle >= minOpenAngle && currentAngle <= maxOpenAngle)
            {
                return 100;
            }

            distance = currentAngle < minOpenAngle
                ? Mathf.Min(minOpenAngle - currentAngle, currentAngle + MaxAngle - maxOpenAngle)
                : Mathf.Min(currentAngle - maxOpenAngle, minOpenAngle + MaxAngle - currentAngle);

            if (distance > _maxAngleOffset)
            {
                return 0;
            }

            var percent = (int) ((_maxAngleOffset - distance) * 100 / _maxAngleOffset);

            return percent;
        }

        private float ClampAngle(float angle)
        {
            if (angle < 0)
            {
                return MaxAngle + angle;
            }

            if (angle <= MaxAngle)
            {
                return angle;
            }

            return angle % MaxAngle;
        }
    }
}