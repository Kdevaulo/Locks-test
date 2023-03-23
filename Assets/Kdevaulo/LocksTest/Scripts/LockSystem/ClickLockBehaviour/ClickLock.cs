using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    public class ClickLock : ILock
    {
        private readonly ClickLockView _lockView;

        private Timer _moveLedTimer;

        private Timer _openLockTimer;

        private PinKit[] _pinKits;

        private Color _closedLedColor;

        private Color _openedLedColor;

        private PinKit _currentKit;

        private PinKit _lastKit;

        private float _moveLedInterval;

        private int _openLockTime;

        private int _openLockSecondsCounter;

        private int _kitIndex;

        private int _maxIndex;

        private bool _isLeftDirection;

        public ClickLock(ClickLockView lockView)
        {
            _lockView = lockView;
            _pinKits = lockView.PinKits;
            _moveLedInterval = lockView.MoveInterval;
            _openLockTime = lockView.OpenLockTime;
            _closedLedColor = lockView.ClosedLedColor;
            _openedLedColor = lockView.OpenedLedColor;
            _openLockTimer = lockView.OpenTimer;
            _moveLedTimer = lockView.MoveTimer;
        }

        void ILock.Initialize()
        {
            _maxIndex = _pinKits.Length - 1;

            _kitIndex = Random.Range(0, _pinKits.Length);

            SetCurrentKit(_kitIndex);

            foreach (var kit in _pinKits)
            {
                kit.SetLedColor(_closedLedColor);
                kit.SetStandartPinPosition();
            }

            _lockView.SetText(_openLockTime.ToString());

            _moveLedTimer.StartTimer(_moveLedInterval);
            _openLockTimer.StartTimer(1);

            _moveLedTimer.Elapsed += HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed += HandleOpenLockTimerElapsed;
            _lockView.Clicked += HandleButtonClick;
        }

        // todo: call after move to next lock
        public void Dispose()
        {
            _moveLedTimer.StopTimer();
            _openLockTimer.StopTimer();

            _moveLedTimer.Elapsed -= HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed -= HandleOpenLockTimerElapsed;
            _lockView.Clicked -= HandleButtonClick;
        }

        private void HandleMoveLedTimerElapsed()
        {
            ChangeKitIndex(_isLeftDirection);

            SetCurrentKit(_kitIndex);

            TryChangeLastKitColor();

            _lastKit = _currentKit;
        }

        private void HandleOpenLockTimerElapsed()
        {
            if (++_openLockSecondsCounter >= _openLockTime)
            {
                _lockView.SetText("Time is up");

                _openLockTimer.StopTimer();
                _moveLedTimer.StopTimer();

                _moveLedTimer.Elapsed -= HandleMoveLedTimerElapsed;
                _openLockTimer.Elapsed -= HandleOpenLockTimerElapsed;
            }
            else
            {
                _lockView.SetText((_openLockTime - _openLockSecondsCounter).ToString());
            }
        }

        private void HandleButtonClick()
        {
            if (_currentKit.Activated)
            {
                _currentKit.SetActive(false);
                _currentKit.SetLedColor(_closedLedColor);
                _currentKit.SetStandartPinPosition();
            }
            else
            {
                _currentKit.SetActive(true);
                _currentKit.SetLedColor(_openedLedColor);
                _currentKit.SetCorrectPinPosition();
            }

            // todo: add speed up logic
            // todo: add finish logic

            _isLeftDirection = !_isLeftDirection;
        }

        private void ChangeKitIndex(bool decrease)
        {
            _kitIndex += decrease ? -1 : 1;

            if (_kitIndex < 0)
            {
                _kitIndex = _maxIndex;
            }
            else if (_kitIndex > _maxIndex)
            {
                _kitIndex = 0;
            }
        }

        private void SetCurrentKit(int index)
        {
            _currentKit = _pinKits[index];

            _currentKit.SetLedColor(_openedLedColor);
        }

        private void TryChangeLastKitColor()
        {
            if (_lastKit == null) return;

            var targetColor = _lastKit.Activated ? _openedLedColor : _closedLedColor;
            _lastKit.SetLedColor(targetColor);
        }
    }
}