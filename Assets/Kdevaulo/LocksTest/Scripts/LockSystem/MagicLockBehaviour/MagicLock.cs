using System;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    public class MagicLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly MagicLockView _lockView;

        private readonly DragHandler _innerDragHandler;
        private readonly DragHandler _outerDragHandler;

        private readonly float _dragSmoothness;

        private Camera _mainCamera;

        private TransformMover _outerMover;
        private TransformMover _innerMover;

        public MagicLock(MagicLockView lockView)
        {
            _lockView = lockView;

            _innerDragHandler = lockView.InnerLightDragHandler;
            _outerDragHandler = lockView.OuterLightDragHandler;

            _dragSmoothness = lockView.DragSmoothness;
        }

        void ILock.Initialize()
        {
            _mainCamera = _lockView.GetCamera();

            var transmitter = new PointTransmitter(_mainCamera);

            _innerMover =
                new TransformMover(_dragSmoothness, _innerDragHandler.ContainerTransform, transmitter);
            _outerMover =
                new TransformMover(_dragSmoothness, _outerDragHandler.ContainerTransform, transmitter);

            SubscribeEvents();
        }

        void ILock.Dispose()
        {
            _outerMover.StopMovement(Vector2.zero);
            _innerMover.StopMovement(Vector2.zero);

            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _innerDragHandler.OnDragBegin += _innerMover.StartMovement;
            _outerDragHandler.OnDragBegin += _outerMover.StartMovement;

            _innerDragHandler.OnDrag += _innerMover.HandleLightMovement;
            _outerDragHandler.OnDrag += _outerMover.HandleLightMovement;

            _outerDragHandler.OnDragEnd += _outerMover.StopMovement;
            _innerDragHandler.OnDragEnd += _innerMover.StopMovement;
        }

        private void UnsubscribeEvents()
        {
            _innerDragHandler.OnDragBegin -= _innerMover.StartMovement;
            _outerDragHandler.OnDragBegin -= _outerMover.StartMovement;

            _innerDragHandler.OnDrag -= _innerMover.HandleLightMovement;
            _outerDragHandler.OnDrag -= _outerMover.HandleLightMovement;

            _outerDragHandler.OnDragEnd -= _innerMover.StopMovement;
            _innerDragHandler.OnDragEnd -= _outerMover.StopMovement;
        }
    }
}