using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour
{
    [AddComponentMenu(nameof(CylinderLockBehaviour) + "/" + nameof(CylinderLockView))]
    public class CylinderLockView : MonoBehaviour, ILockView
    {
        public event Action<float> Moved = delegate { };
        public event Action<bool> OpenLockPressed = delegate { };
        public event Action OpenLockStarted = delegate { };
        public event Action OpenLockEnded = delegate { };
        public event Action LockOpened = delegate { };

        public CylinderLockSoundPlayer SoundPlayer => _soundPlayer;
        public float StartAngle => _startAngle;
        public float MinMaxRotationOffset => _minMaxRotationOffset;
        public float OpenAngleOffset => _openAngleOffset;
        public float MaxAngleOffset => _maxAngleOffset;
        public Vector2 LockOpenRange => _lockOpenRange;

        [SerializeField] private CylinderLockSoundPlayer _soundPlayer;

        [SerializeField] private Transform _keyholeRotationContainer;

        [SerializeField] private Transform _toolRotationContainer;

        [SerializeField] private Transform _lockContainer;

        [SerializeField] private Canvas _canvas;

        [Header("Lockpick settings")]
        [SerializeField] private float _startAngle = 90f;

        [Min(0), Tooltip("Lockpick range from startAngle")]
        [SerializeField] private float _minMaxRotationOffset = 160f;

        [Tooltip("Offset between minOpenAngle and maxOpenAngle")]
        [SerializeField] private float _openAngleOffset = 20f;

        [Tooltip("Range of the lockpick from lockOpenRange after which the animation does not turn on")]
        [SerializeField] private float _maxAngleOffset = 70f;

        [Tooltip("Range for minOpenAngle")]
        [SerializeField] private Vector2 _lockOpenRange = new Vector2(-20, 200);

        [Header("LockCylinderSettings")]
        [SerializeField] private float _lockRotationAngle = -90;

        [SerializeField] private float _lockRotationSpeed = 10f;

        [Header("Other settings")]
        [SerializeField] private float _beforeDisappearDelay = 0.5f;

        private float _currentRotationProgress = 0f;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        void ILockView.SetCamera(Camera targetCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = targetCamera;
        }

        void ILockView.Dispose()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        void ILockView.DestroyGameObject()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public async UniTask DisappearAsync()
        {
            await AppearanceTweener.DisappearAsync(_beforeDisappearDelay, _lockContainer, _cts.Token);
        }

        public void MoveLockPick(float value)
        {
            Moved.Invoke(value);
        }

        public void HandleOpenLockKey(bool value)
        {
            OpenLockPressed.Invoke(value);
        }

        public float GetAngle()
        {
            return _toolRotationContainer.localEulerAngles.z;
        }

        public void RotateToolContainer(float value)
        {
            var currentRotation = _toolRotationContainer.rotation;
            _toolRotationContainer.Rotate(currentRotation.x, currentRotation.y, value);
        }

        public void SetToolZRotation(float value)
        {
            var eulerAngles = _toolRotationContainer.rotation.eulerAngles;
            _toolRotationContainer.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, value);
        }

        public void RotateKeyholeContainerToTarget(float percentage)
        {
            RotateKeyholeContainer(1, percentage);
        }

        public void RotateKeyholeContainerToDefault(float percentage)
        {
            RotateKeyholeContainer(-1, percentage);
        }

        private void RotateKeyholeContainer(float directionMultiplier, float rotationPercentage)
        {
            if (_currentRotationProgress == 0)
            {
                if (directionMultiplier == 1)
                {
                    OpenLockStarted.Invoke();
                }
                else
                {
                    OpenLockEnded.Invoke();
                }
            }

            var percentageSpeedMultiplier = 100 / rotationPercentage;

            _currentRotationProgress =
                Mathf.Clamp01(_currentRotationProgress +
                              Time.deltaTime * directionMultiplier * _lockRotationSpeed * percentageSpeedMultiplier);

            var rotation = Quaternion.Lerp(Quaternion.identity,
                Quaternion.Euler(0, 0, _lockRotationAngle * rotationPercentage / 100),
                _currentRotationProgress);
            _keyholeRotationContainer.localRotation = rotation;

            if (_currentRotationProgress == 1 && rotationPercentage == 100)
            {
                LockOpened.Invoke();
            }
        }
    }
}