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

            RemoveRandomPinsFromKits();

            var enabledKitsCount = _pinKits.Count;

            _maxIndex = _pinKits.Count - 1;

            _kitIndex = Random.Range(0, enabledKitsCount);

            SetCurrentKit(_kitIndex);

            InitializePinKits();

            _openLockSecondsCounter = _openLockTime;
            _lockView.SetText(_openLockTime.ToString());

            ChangeMoveLedSpeed(_allKitsCount);
            _openLockTimer.StartTimer(1);

            SubscribeEvents();
        }

        void ILock.Dispose()
        {
            UnsubscribeEvents();
        }

        private void RemoveRandomPinsFromKits()
        {
            var defaultDisabledCount = Random.Range(0, _lockView.MaxDefaultDisabledCount + 1);

            for (var i = 0; i < defaultDisabledCount; i++)
            {
                var index = Random.Range(0, _pinKits.Count);

                var kit = _pinKits[index];
                kit.SetCorrectPinPosition();

                _pinKits.RemoveAt(index);
            }
        }

        private void InitializePinKits()
        {
            foreach (var kit in _pinKits)
            {
                kit.SetLedColor(_closedLedColor);
                kit.SetStandartPinPosition();
            }
        }

        private void SubscribeEvents()
        {
            _moveLedTimer.Elapsed += HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed += HandleOpenLockTimerElapsed;
            _lockView.Clicked += HandleButtonClick;
        }

        private void HandleMoveLedTimerElapsed()
        {
            ChangeKitIndex(_isLeftDirection);

            SetCurrentKit(_kitIndex);

            TryChangeLastKitColor();

            _soundPlayer.PlayMoveLedSound();

            _lastKit = _currentKit;
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

        private void HandleOpenLockTimerElapsed()
        {
            if (--_openLockSecondsCounter <= 0)
            {
                HandleLockTimeIsUp();
            }
            else
            {
                _lockView.SetText(_openLockSecondsCounter.ToString());
            }
        }

        private void HandleLockTimeIsUp()
        {
            _lockView.SetText(ClickLockConstants.TimeOverText);
            _lockView.DisableInteractionButton();

            StopTimers();

            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            _moveLedTimer.Elapsed -= HandleMoveLedTimerElapsed;
            _openLockTimer.Elapsed -= HandleOpenLockTimerElapsed;
            _lockView.Clicked -= HandleButtonClick;
        }

        private void HandleButtonClick()
        {
            _isLeftDirection = !_isLeftDirection;

            if (_currentKit.Activated)
            {
                DisableCurrentKit();
            }
            else
            {
                EnableCurrentKit();
            }

            var disabledKitsCount = GetDisabledKitsCount();
            _soundPlayer.PlayClickSound(_pinKits.Count - disabledKitsCount);

            ChangeMoveLedSpeed(disabledKitsCount + _allKitsCount - _pinKits.Count);

            if (disabledKitsCount == 0)
            {
                HandleLockOpened();
            }
        }

        private void DisableCurrentKit()
        {
            _currentKit.SetActive(false);
            _currentKit.SetLedColor(_closedLedColor);
            _currentKit.SetStandartPinPosition();
        }

        private void EnableCurrentKit()
        {
            _currentKit.SetActive(true);
            _currentKit.SetLedColor(_openedLedColor);
            _currentKit.SetCorrectPinPosition();
        }

        private int GetDisabledKitsCount()
        {
            return _pinKits.Count(kit => !kit.Activated);
        }

        private void ChangeMoveLedSpeed(int disabledKitsCount)
        {
            _moveLedTimer.StartTimer(_minMoveDelay + _moveLedDelayMultiplier * disabledKitsCount);
        }

        private void HandleLockOpened()
        {
            StopTimers();

            _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
        }

        private void StopTimers()
        {
            _moveLedTimer.StopTimer();
            _openLockTimer.StopTimer();
        }
    }
}