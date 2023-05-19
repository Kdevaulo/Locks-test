using System;
using System.Collections.Generic;
using System.Linq;

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

        public Timer CheckTimer => _checkTimer;
        public RotateImageLockSoundPlayer SoundPlayer => _soundPlayer;

        [Header("Correct behaviour settings")]
        [SerializeField] private float _maxDegreeDifference = 4f;

        [Header("References")]
        [SerializeField] private Sprite[] _lockSprites;

        [SerializeField] private SpriteRenderer _staticRenderer;
        [SerializeField] private SpriteRenderer _correctImageRenderer;

        [SerializeField] private GameObject _correctImageGameObject;

        [SerializeField] private RotateImageLockSoundPlayer _soundPlayer;
        [SerializeField] private ImageRingContainer _imageRingContainer;
        [SerializeField] private Timer _checkTimer;

        private Camera _targetCamera;

        private void Awake()
        {
            InitializeToken();
        }

        void ICameraGetter.SetCamera(Camera targetCamera)
        {
            _targetCamera = targetCamera;

            SetCameraToCanvas(targetCamera, hintCanvas);
        }

        void ILockView.Dispose()
        {
            TryCancelToken();
        }

        void ILockView.DestroyGameObject()
        {
            DestroyGameObject();
        }

        public async UniTask RotateRingsAtRandomAnglesAsync()
        {
            var tasks = new List<UniTask>(3);

            ForEachInImageRingCollection(x => tasks.Add(RandomTransformZRotator.DoRandomRotateAsync(x.Transform)));

            await UniTask.WhenAll(tasks);
        }

        public async UniTask DisappearAsync()
        {
            await AppearanceTweener.DisappearAsync(beforeDisappearDelay, lockContainer, cts.Token);
        }

        public Camera GetCamera()
        {
            return _targetCamera;
        }

        public bool CheckIfRingsAtStartPosition()
        {
            var result = _imageRingContainer.ImageRingCollection.All(x =>
            {
                var zRotation = x.Transform.localRotation.eulerAngles.z;

                return zRotation <= _maxDegreeDifference && zRotation >= 0 ||
                       zRotation >= RotateImageLockConstants.MaxAngle - _maxDegreeDifference &&
                       zRotation <= RotateImageLockConstants.MaxAngle;
            });

            return result;
        }

        public void EnableCorrectImage()
        {
            _correctImageGameObject.SetActive(true);
        }

        public void SetSpriteToRings(Sprite sprite)
        {
            ForEachInImageRingCollection(x => x.Renderer.sprite = sprite);

            _staticRenderer.sprite = sprite;
            _correctImageRenderer.sprite = sprite;
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