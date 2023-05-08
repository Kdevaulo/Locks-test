using System;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour
{
    public class ResistanceClickLock : ILock
    {
        public event Action LockOpened = delegate { };

        private const int EnemyMultiplier = 1;
        private const int PlayerMultiplier = -1;

        private readonly float _enemyAttackDelay;
        private readonly float _enemyAttackDamage;
        private readonly float _userAttackDamage;

        private readonly Vector2 _scaleRotationRange;

        private readonly Timer _attackTimer;

        private ResistanceClickLockView _lockView;

        private float _currentRotation;

        public ResistanceClickLock(ResistanceClickLockView lockView)
        {
            _lockView = lockView;

            _attackTimer = lockView.AttackTimer;

            _enemyAttackDamage = lockView.EnemyAttackDamage;
            _enemyAttackDelay = lockView.EnemyAttackDelay;

            _userAttackDamage = lockView.UserAttackDamage;

            _scaleRotationRange = lockView.ScaleRotationRange;
        }

        void ILock.Initialize()
        {
            _attackTimer.Elapsed += HandleEnemyAttack;
            _attackTimer.StartTimer(_enemyAttackDelay);
        }

        void ILock.Dispose()
        {
            _attackTimer.Elapsed -= HandleEnemyAttack;
            _attackTimer.StopTimer();
        }

        private void HandleEnemyAttack()
        {
            _currentRotation += _enemyAttackDamage * EnemyMultiplier;

            RotateScale();
                // TODO: поворот ускоряется
            Debug.Log(_currentRotation);
        }

        private void HandlePlayerAttack()
        {
            _currentRotation += _userAttackDamage * PlayerMultiplier;

            RotateScale();
        }

        private void RotateScale()
        {
            _lockView.SetRotation(_currentRotation);
            CheckScaleValue();
        }

        private void CheckScaleValue()
        {
            if (_currentRotation <= _scaleRotationRange.x)
            {
                // loose
            }

            if (_currentRotation >= _scaleRotationRange.y)
            {
                // win
            }
        }
    }
}