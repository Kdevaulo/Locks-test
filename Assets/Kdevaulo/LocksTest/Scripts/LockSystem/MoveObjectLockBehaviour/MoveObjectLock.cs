using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    public class MoveObjectLock : ILock
    {
        public event Action LockOpened;

        private readonly MoveObjectLockView _lockView;

        private readonly float _userMovingSpeed;

        public MoveObjectLock(MoveObjectLockView lockView)
        {
            _lockView = lockView;
            _userMovingSpeed = _lockView.UserMovingSpeed;
        }

        void ILock.Initialize()
        {
            _lockView.ItemMoved += HandleItemMoved;

            _lockView.SetRandomObject();
        }

        void ILock.Dispose()
        {
        }

        private void HandleItemMoved(Vector2 offset)
        {
            var targetPosition = CalculatePosition(offset);

            _lockView.SetObjectContainerLocalPosition(targetPosition);
        }

        private Vector3 CalculatePosition(Vector2 offset)
        {
            var targetPosition = _lockView.MovingContainerLocalPosition + (Vector3) offset * _userMovingSpeed;

            return ClampPositionByCircle(targetPosition);
        }

        private Vector3 ClampPositionByCircle(Vector3 position)
        {
            return position;
        }
    }
}