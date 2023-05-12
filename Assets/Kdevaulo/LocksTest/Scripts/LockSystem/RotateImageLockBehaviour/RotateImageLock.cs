using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    public class RotateImageLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly Timer _checkTimer;
        private readonly RotateImageLockView _lockView;
        private readonly RotateImageLockSoundPlayer _soundPlayer;

        private readonly Sprite[] _sprites;

        private readonly List<SubscriptionContainer> _subscriptionContainers = new List<SubscriptionContainer>(3);
        private readonly List<TransformRotator> _transformRotators = new List<TransformRotator>(3);

        private Sprite _currentSprite;
        private Camera _targetCamera;

        private bool _isLockOpened;

        public RotateImageLock(RotateImageLockView lockView)
        {
            _lockView = lockView;
            _sprites = lockView.LockSprites;

            _checkTimer = lockView.CheckTimer;
            _soundPlayer = lockView.SoundPlayer;
        }

        void ILock.Initialize()
        {
            _targetCamera = _lockView.GetCamera(); // note: must be before SubscribeImageRings

            ChooseRandomSprite();
            SetSpriteToLock();

            SubscribeImageRings();

            _checkTimer.Elapsed += CheckIfLockOpened;

            _lockView.RotateRingsAtRandomAngles().ContinueWith(StartGame);

            _soundPlayer.PlayStartRotationSound();
        }

        void ILock.Dispose()
        {
            foreach (var container in _subscriptionContainers)
            {
                var handler = container.InputHandler;

                handler.OnDrag -= container.PointerDragAction;
                handler.OnPointerDown -= container.PointerDownAction;
            }

            _checkTimer.Elapsed -= CheckIfLockOpened;
        }

        private void ChooseRandomSprite()
        {
            _currentSprite = _sprites[Random.Range(0, _sprites.Length)];
        }

        private void SetSpriteToLock()
        {
            _lockView.SetSpriteToRings(_currentSprite);
        }

        private void SubscribeImageRings()
        {
            foreach (var imageRing in _lockView.ImageRingCollection)
            {
                var inputHandler = imageRing.InputHandler;

                var transformRotator = new TransformRotator(imageRing.Transform, _targetCamera);

                _transformRotators.Add(transformRotator);

                Action downAction = transformRotator.HandlePointerDown;
                Action dragAction = () =>
                {
                    transformRotator.HandlePointerDrag();
                    TryPlayRotationSound();
                };

                inputHandler.OnDrag += dragAction;
                inputHandler.OnPointerDown += downAction;

                _subscriptionContainers.Add(new SubscriptionContainer(dragAction, downAction, inputHandler));
            }
        }

        private void TryPlayRotationSound()
        {
            if (_isLockOpened)
                return;

            _soundPlayer.PlayRotateSoundAsync().Forget();
        }

        private void CheckIfLockOpened()
        {
            if (!_isLockOpened && _lockView.CheckIfRingsAtStartPosition())
            {
                HandleLockOpened();
            }
        }

        private void HandleLockOpened()
        {
            _isLockOpened = true;

            _lockView.EnableCorrectImage();

            foreach (var rotator in _transformRotators)
            {
                rotator.DisableRotation();
            }

            _lockView.DisappearAsync().ContinueWith(LockOpened.Invoke);

            _soundPlayer.PlayOpenSound();
        }

        private void StartGame()
        {
            _lockView.EnableColliders();

            _checkTimer.StartTimer(RotateImageLockConstants.DelayBeforeCheck);
        }
    }
}