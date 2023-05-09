using System;

using Cysharp.Threading.Tasks;

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
        private readonly ResistanceClickLockView _lockView;
        private readonly ResistanceClickLockSoundPlayer _soundPlayer;

        private float _currentRotation;

        public ResistanceClickLock(ResistanceClickLockView lockView)
        {
            _lockView = lockView;
            _soundPlayer = lockView.SoundPlayer;

            _attackTimer = lockView.AttackTimer;

            _enemyAttackDamage = lockView.EnemyAttackDamage;
            _enemyAttackDelay = lockView.EnemyAttackDelay;

            _userAttackDamage = lockView.UserAttackDamage;

            _scaleRotationRange = lockView.ScaleRotationRange;
        }

        void ILock.Initialize()
        {
            _lockView.Clicked += HandlePlayerAttack;

            _attackTimer.Elapsed += HandleEnemyAttack;
            _attackTimer.StartTimer(_enemyAttackDelay);

            _soundPlayer.StartPlayBackgroundBattleSound();
        }

        void ILock.Dispose()
        {
            _lockView.Clicked -= HandlePlayerAttack;

            _attackTimer.Elapsed -= HandleEnemyAttack;
            _attackTimer.StopTimer();
        }

        private void HandlePlayerAttack()
        {
            _currentRotation += _userAttackDamage * PlayerMultiplier;

            RotateScale();
        }

        private void HandleEnemyAttack()
        {
            _currentRotation += _enemyAttackDamage * EnemyMultiplier;

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
                StopBattle();
                _lockView.DisableTextCanvas();

                _soundPlayer.PlayWinSound();

                _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);
            }

            if (_currentRotation >= _scaleRotationRange.y)
            {
                StopBattle();

                _soundPlayer.PlayLoseSound();
            }
        }

        private void StopBattle()
        {
            DisableInteractButton();
            StopTimer();

            _soundPlayer.StopBattleSound();
        }

        private void DisableInteractButton()
        {
            _lockView.DisableInteractButton();
        }

        private void StopTimer()
        {
            _attackTimer.StopTimer();
        }
    }
}