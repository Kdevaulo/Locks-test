using System.Threading;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    public class TransformMover
    {
        private readonly float _smoothness;

        private readonly Transform _targetTransform;

        private readonly PointTransmitter _transmitter;

        private Vector2 _targetPosition;

        private CancellationTokenSource _cts;

        private bool _isDragging;

        public TransformMover(float smoothness, Transform targetTransform, PointTransmitter transmitter)
        {
            _smoothness = smoothness;
            _targetTransform = targetTransform;
            _transmitter = transmitter;
        }

        public void StartMovement(Vector2 vector)
        {
            ChangeTargetPosition(vector);

            _cts = new CancellationTokenSource();

            _isDragging = true;

            MoveAsync(_cts.Token).Forget();
        }

        public void HandleLightMovement(Vector2 vector)
        {
            ChangeTargetPosition(vector);
        }

        public void StopMovement(Vector2 vector)
        {
            _isDragging = false;

            ChangeTargetPosition(vector);

            CancelToken();
        }

        private async UniTask MoveAsync(CancellationToken token)
        {
            while (_isDragging)
            {
                _targetTransform.position = Vector3.Lerp(_targetTransform.position, _targetPosition,
                    _smoothness * Time.deltaTime);

                await UniTask.Yield(token);
            }
        }

        private void ChangeTargetPosition(Vector2 vector)
        {
            _targetPosition = _transmitter.TransmitPoint(vector);
        }

        private void CancelToken()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}