using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    [AddComponentMenu(nameof(RotateImageLockBehaviour) + "/" + nameof(RotateImageLockView))]
    public class RotateImageLockView : AbstractLockView, ILockView
    {
        public Sprite[] LockSprites => _lockSprites;

        public ImageRing[] ImageRingCollection => _imageRingContainer.ImageRingCollection;

        [Header("References")]
        [SerializeField] private Sprite[] _lockSprites;

        [SerializeField] private Canvas _hintCanvas;

        [SerializeField] private ImageRingContainer _imageRingContainer;

        [SerializeField] private SpriteRenderer _staticRenderer;

        private Camera _targetCamera;

        private void Awake()
        {
            InitializeToken();
        }

        void ICameraGetter.SetCamera(Camera targetCamera)
        {
            _targetCamera = targetCamera;

            SetCameraToCanvas(targetCamera, _hintCanvas);
        }

        void ILockView.Dispose()
        {
            TryCancelToken();
        }

        void ILockView.DestroyGameObject()
        {
            DestroyGameObject();
        }

        public async UniTask RotateRingsAtRandomAngles()
        {
            var tasks = new List<UniTask>(3);

            ForEachInImageRingCollection(x => tasks.Add(RandomTransformZRotator.DoRandomRotate(x.Transform)));

            await UniTask.WhenAll(tasks);
        }

        public Camera GetCamera()
        {
            return _targetCamera;
        }

        public void SetSpriteToRings(Sprite sprite)
        {
            ForEachInImageRingCollection(x => x.Renderer.sprite = sprite);

            _staticRenderer.sprite = sprite;
        }

        public void EnableColliders()
        {
            ForEachInImageRingCollection(x => x.Collider.enabled = true);
        }

        private void ForEachInImageRingCollection(Action<ImageRing> action)
        {
            foreach (var imageRing in _imageRingContainer.ImageRingCollection)
            {
                action.Invoke(imageRing);
            }
        }
    }
}