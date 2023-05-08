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
        private readonly CylinderLockSoundPlayer _soundPlayer;

        private readonly float _startAngle;
        private readonly float _minMaxRotationOffset;
        private readonly float _maxAngleOffset;

        private readonly Vector2 _lockOpenRange;

        private Vector2 _minMaxOpenAngle;

        private int _currentOpenPercentage;

        private bool _isLockOpening;
        private bool _lockOpened;

        public CylinderLock(CylinderLockView lockView)
        {
            _lockView = lockView;

            _minMaxRotationOffset = lockView.MinMaxRotationOffset;
            _lockOpenRange = lockView.LockOpenRange;
            _maxAngleOffset = lockView.MaxAngleOffset;

            _soundPlayer = lockView.SoundPlayer;

            _startAngle = ClampAngle(lockView.StartAngle);
        }

        void ILock.Initialize()
        {
            _lockView.LockOpened += HandleLockOpened;
            _lockView.OpenLockPressed += HandleOpenLockKeyPressed;
            _lockView.OpenLockStarted += HandleOpenLockStarted;
            _lockView.OpenLockEnded += HandleOpenLockEnded;
            _lockView.Moved += HandleLockPickMoved;

            _lockView.SetToolZRotation(_startAngle);

            ChooseRandomAngle();
        }

        void ILock.Dispose()
        {
            _lockView.LockOpened -= HandleLockOpened;
            _lockView.OpenLockPressed -= HandleOpenLockKeyPressed;
            _lockView.OpenLockStarted -= HandleOpenLockStarted;
            _lockView.OpenLockEnded -= HandleOpenLockEnded;
            _lockView.Moved -= HandleLockPickMoved;
        }

        private void ChooseRandomAngle()
        {
            var minAngle = Random.Range(_lockOpenRange.x, _lockOpenRange.y);

            _minMaxOpenAngle = new Vector2(minAngle, minAngle + _lockView.OpenAngleOffset);
        }

        private void HandleLockOpened()
        {
            _lockOpened = true;

            _soundPlayer.PlayOpenSound();
            _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
        }

        private void HandleOpenLockKeyPressed(bool value)
        {
            if (_lockOpened)
            {
                return;
            }

            if (value)
            {
                _lockView.RotateKeyholeContainerToTarget(_currentOpenPercentage);
            }
            else
            {
                _lockView.RotateKeyholeContainerToDefault(_currentOpenPercentage);
            }
        }

        private void HandleOpenLockStarted()
        {
            _isLockOpening = true;

            _soundPlayer.PlayRotateSound();
        }

        private void HandleOpenLockEnded()
        {
            _isLockOpening = false;
        }

        private void HandleLockPickMoved(float value)
        {
            if (_isLockOpening || _lockOpened)
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

        private int CalculateCloseToTargetPercentage(float currentAngle)
        {
            var minOpenAngle = _minMaxOpenAngle.x;
            var maxOpenAngle = _minMaxOpenAngle.y;

            var openDistance = maxOpenAngle - minOpenAngle;

            if (currentAngle >= minOpenAngle && currentAngle <= maxOpenAngle ||
                minOpenAngle < 0 && minOpenAngle > -openDistance && currentAngle > MaxAngle - openDistance)
            {
                return 100;
            }

            float distance = currentAngle < minOpenAngle
                ? Mathf.Min(minOpenAngle - currentAngle, currentAngle + MaxAngle - maxOpenAngle)
                : Mathf.Min(currentAngle - maxOpenAngle, minOpenAngle + MaxAngle - currentAngle);

            if (distance > _maxAngleOffset)
            {
                return 0;
            }

            var percent = (int) ((_maxAngleOffset - distance) * 100 / _maxAngleOffset);

            return percent;
        }
    }
}