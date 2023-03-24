using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    public class ClickLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly ClickLockView _lockView;

        private readonly Timer _moveLedTimer;

        private readonly Timer _openLockTimer;

        private readonly ClickSoundPlayer _soundPlayer;

        private readonly List<PinKit> _pinKits;

        private readonly Color _closedLedColor;

        private readonly Color _openedLedColor;

        private readonly float _moveLedDelayMultiplier;

        private readonly float _minMoveDelay;

        private readonly int _openLockTime;

        private PinKit _currentKit;

        private PinKit _lastKit;

        private int _openLockSecondsCounter;

        private int _kitIndex;

        private int _maxIndex;

        private int _allKitsCount;

        private bool _isLeftDirection;

        public ClickLock(ClickLockView lockView)
        {
            _lockView = lockView;
            _pinKits = lockView.PinKits.ToList();

            _closedLedColor = lockView.ClosedLedColor;
            _openedLedColor = lockView.OpenedLedColor;

            _openLockTime = lockView.OpenLockTime;
            _openLockTimer = lockView.OpenTimer;

            _moveLedDelayMultiplier = lockView.MoveLedDelayMultiplier;
            _minMoveDelay = lockView.MinMoveDelay;
            _moveLedTimer = lockView.MoveTimer;

            _soundPlayer = lockView.SoundPlayer;
        }

        void ILock.Initialize()
        {
            _allKitsCount = _pinKits.Count;

            var defaultDisabledCount = Random.Range(0, _lockView.MaxDefaultDisabledCount + 1);

            for (var i = 0; i < defaultDisabledCount; i++)
            {
                var index = Random.Range(0, _pinKits.Count);

                var kit = _pinKits[index];
                kit.SetCorrectPinPosition();

                _pinKits.RemoveAt(index);
            }

            var enabledKitsCount = _pinKits.Count;

            _maxIndex = _pinKits.Count - 1;

            _kitIndex = Random.Range(0, enabledKitsCount);

            SetCurrentKit(_kitIndex);

            foreach (var kit in _pinKits)
            {
                kit.SetLedColor(_closedLedColor);
                kit.SetStandartPinPosition();
            }

            _lockView.SetText(_openLockTime.ToString());

            ChangeMoveLedSpeed(_allKitsCount);
            _openLockTimer.StartTimer(1);

            _moveLedTimer.Elapsed += HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed += HandleOpenLockTimerElapsed;
            _lockView.Clicked += HandleButtonClick;
        }

        void ILock.Dispose()
        {
            _moveLedTimer.Elapsed -= HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed -= HandleOpenLockTimerElapsed;
            _lockView.Clicked -= HandleButtonClick;
        }

        private void HandleMoveLedTimerElapsed()
        {
            ChangeKitIndex(_isLeftDirection);

            SetCurrentKit(_kitIndex);

            TryChangeLastKitColor();

            _soundPlayer.PlayMoveLedSound();

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
                _lockView.Clicked -= HandleButtonClick;
            }
            else
            {
                _lockView.SetText((_openLockTime - _openLockSecondsCounter).ToString());
            }
        }

        private void HandleButtonClick()
        {
            _soundPlayer.PlayClickSound(_pinKits.Count - GetDisabledKitsCount());

            _isLeftDirection = !_isLeftDirection;

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

            var disabledKitsCount = GetDisabledKitsCount();
            ChangeMoveLedSpeed(disabledKitsCount + _allKitsCount - _pinKits.Count);

            if (disabledKitsCount == 0)
            {
                _moveLedTimer.StopTimer();
                _openLockTimer.StopTimer();

                _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
            }
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

            _currentKit.SetLedColor(_currentKit.Activated ? _closedLedColor : _openedLedColor);
        }

        private void TryChangeLastKitColor()
        {
            if (_lastKit == null) return;

            var targetColor = _lastKit.Activated ? _openedLedColor : _closedLedColor;
            _lastKit.SetLedColor(targetColor);
        }

        private void ChangeMoveLedSpeed(int disabledKitsCount)
        {
            _moveLedTimer.StartTimer(_minMoveDelay + _moveLedDelayMultiplier * disabledKitsCount);
        }

        private int GetDisabledKitsCount()
        {
            return _pinKits.Count(kit => !kit.Activated);
        }
    }
}