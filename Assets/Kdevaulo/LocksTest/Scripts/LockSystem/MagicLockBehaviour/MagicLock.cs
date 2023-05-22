using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    public class MagicLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly MagicLockView _lockView;

        private readonly DragHandler _outerDragHandler;
        private readonly DragHandler _innerDragHandler;

        private readonly Transform _outerRingContainer;
        private readonly Transform _innerRingContainer;
        private readonly Transform _outerWheel;
        private readonly Transform _innerWheel;

        private readonly MagicLockSoundPlayer _soundPlayer;

        private readonly float _dragSmoothness;
        private readonly float _maxDegreeOffset;
        private readonly float _holdLightSeconds;
        private readonly float _rotationSpeed;

        private const float MaxAngle = 360f;

        private Camera _mainCamera;

        private TransformMover _outerMover;
        private TransformMover _innerMover;

        private CancellationTokenSource _cts;

        private Vector2 _outerRotationRange;
        private Vector2 _innerRotationRange;

        private float _outerTimeCounter;
        private float _innerTimeCounter;

        private bool _isRotating;

        public MagicLock(MagicLockView lockView)
        {
            _lockView = lockView;

            _outerDragHandler = lockView.OuterLightDragHandler;
            _innerDragHandler = lockView.InnerLightDragHandler;

            _soundPlayer = lockView.SoundPlayer;

            _dragSmoothness = lockView.DragSmoothness;
            _maxDegreeOffset = lockView.MaxDegreeOffset;
            _holdLightSeconds = lockView.HoldLightSeconds;
            _rotationSpeed = lockView.RotationSpeed;

            _outerRingContainer = lockView.OuterRingContainer;
            _innerRingContainer = lockView.InnerRingContainer;
            _outerWheel = lockView.OuterWheel;
            _innerWheel = lockView.InnerWheel;
        }

        void ILock.Initialize()
        {
            _mainCamera = _lockView.GetCamera();

            var transmitter = new PointTransmitter(_mainCamera);

            _outerMover = new TransformMover(_dragSmoothness, _outerDragHandler.Rigidbody, transmitter);
            _innerMover = new TransformMover(_dragSmoothness, _innerDragHandler.Rigidbody, transmitter);

            SubscribeEvents();

            ChooseCorrectRotations();

            _isRotating = true;
            _cts = new CancellationTokenSource();
            CheckBehaviourAsync(_cts.Token).Forget();
        }

        void ILock.Dispose()
        {
            if (_outerMover != null && _innerMover != null)
            {
                _outerMover.StopMovement(Vector2.zero);
                _innerMover.StopMovement(Vector2.zero);

                UnsubscribeEvents();
            }

            TryCancelToken();
        }

        private void SubscribeEvents()
        {
            _outerDragHandler.OnDragBegin += _outerMover.StartMovement;
            _innerDragHandler.OnDragBegin += _innerMover.StartMovement;

            _outerDragHandler.OnDrag += _outerMover.HandleLightMovement;
            _innerDragHandler.OnDrag += _innerMover.HandleLightMovement;

            _outerDragHandler.OnDragEnd += _outerMover.StopMovement;
            _innerDragHandler.OnDragEnd += _innerMover.StopMovement;
        }

        private void ChooseCorrectRotations()
        {
            _outerRotationRange = GetRotationRange(GetRandomDegree());
            _innerRotationRange = GetRotationRange(GetRandomDegree());
        }

        private int GetRandomDegree()
        {
            return MagicLockConstants.CorrectZRotations[Random.Range(0, MagicLockConstants.CorrectZRotations.Length)];
        }

        private Vector2 GetRotationRange(int targetDegree)
        {
            return new Vector2(targetDegree - _maxDegreeOffset, targetDegree + _maxDegreeOffset);
        }

        private async UniTask CheckBehaviourAsync(CancellationToken token)
        {
            while (_isRotating)
            {
                CheckRotations();

                await UniTask.WaitForFixedUpdate(token);
            }
        }

        private void CheckRotations()
        {
            _outerTimeCounter =
                CalculateHoldTime(_outerRingContainer.rotation.eulerAngles.z, _outerRotationRange, _outerTimeCounter);
            _innerTimeCounter =
                CalculateHoldTime(_innerRingContainer.rotation.eulerAngles.z, _innerRotationRange, _innerTimeCounter);

            HandleCounterValues();

            if (CheckTimer(_outerTimeCounter) &
                CheckTimer(_innerTimeCounter))
            {
                FinishGame();
            }
        }

        private float CalculateHoldTime(float rotation, Vector2 rotationRange, float timeCounter)
        {
            if (IsInRange(rotation, rotationRange))
            {
                timeCounter += Time.deltaTime;
            }
            else
            {
                timeCounter = 0;
            }

            return timeCounter;
        }

        private bool IsInRange(float rotation, Vector2 targetRange)
        {
            if (targetRange.y - _maxDegreeOffset * 2 < 0) // note: targetRange through 0 (360)
            {
                return rotation >= 0 && rotation <= targetRange.y ||
                       rotation >= CastToTargetRange(targetRange.x) && rotation <= MaxAngle;
            }

            return rotation >= targetRange.x && rotation <= targetRange.y;
        }

        private float CastToTargetRange(float value)
        {
            if (value < 0)
            {
                return value % MaxAngle + MaxAngle;
            }

            return value % MaxAngle;
        }

        private void HandleCounterValues()
        {
            if (_outerTimeCounter > 0)
            {
                _soundPlayer.TryStartPlayingBigWheelSound();

                RotateTransform(_outerWheel);
            }
            else
            {
                _soundPlayer.TryStopPlayingBigWheelSound();
            }

            if (_innerTimeCounter > 0)
            {
                _soundPlayer.TryStartPlayingSmallWheelSound();

                RotateTransform(_innerWheel);
            }
            else
            {
                _soundPlayer.TryStopPlayingSmallWheelSound();
            }
        }

        private void RotateTransform(Transform targetTransform)
        {
            targetTransform.DORotate(Vector3.forward * _rotationSpeed, Time.deltaTime, RotateMode.WorldAxisAdd);
        }

        private bool CheckTimer(float seconds)
        {
            return seconds >= _holdLightSeconds;
        }

        private void FinishGame()
        {
            _isRotating = false;

            _soundPlayer.TryStopPlayingBigWheelSound();
            _soundPlayer.TryStopPlayingSmallWheelSound();
            _soundPlayer.PlayOpenSound();

            AnimateGameEndAsync().Forget();
        }

        private async UniTask AnimateGameEndAsync()
        {
            await _lockView.DisappearAsync();

            LockOpened.Invoke();
        }

        private void UnsubscribeEvents()
        {
            _outerDragHandler.OnDragBegin -= _outerMover.StartMovement;
            _innerDragHandler.OnDragBegin -= _innerMover.StartMovement;

            _outerDragHandler.OnDrag -= _outerMover.HandleLightMovement;
            _innerDragHandler.OnDrag -= _innerMover.HandleLightMovement;

            _outerDragHandler.OnDragEnd -= _innerMover.StopMovement;
            _innerDragHandler.OnDragEnd -= _outerMover.StopMovement;
        }

        private void TryCancelToken()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}