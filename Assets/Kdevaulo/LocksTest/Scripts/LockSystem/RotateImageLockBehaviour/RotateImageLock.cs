using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    public class RotateImageLock : ILock
    {
        public event Action LockOpened = delegate { };

        private readonly RotateImageLockView _lockView;

        private readonly Sprite[] _sprites;

        private readonly List<SubscriptionContainer> _subscriptionContainers = new List<SubscriptionContainer>(3);

        private Sprite _currentSprite;

        private Camera _targetCamera;

        public RotateImageLock(RotateImageLockView lockView)
        {
            _lockView = lockView;
            _sprites = lockView.LockSprites;
        }

        void ILock.Initialize()
        {
            _targetCamera = _lockView.GetCamera();

            ChooseRandomSprite();
            SetSpriteToLock();

            SubscribeImageRings();

            _lockView.RotateRingsAtRandomAngles().ContinueWith(_lockView.EnableColliders);
        }

        void ILock.Dispose()
        {
            foreach (var container in _subscriptionContainers)
            {
                var handler = container.InputHandler;

                handler.OnDrag -= container.PointerDragAction;
                handler.OnPointerDown -= container.PointerDownAction;
            }
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

                Action dragAction = transformRotator.HandlePointerDrag;
                Action downAction = transformRotator.HandlePointerDown;

                inputHandler.OnDrag += dragAction;
                inputHandler.OnPointerDown += downAction;

                _subscriptionContainers.Add(new SubscriptionContainer(dragAction, downAction, inputHandler));
            }
        }
    }
}