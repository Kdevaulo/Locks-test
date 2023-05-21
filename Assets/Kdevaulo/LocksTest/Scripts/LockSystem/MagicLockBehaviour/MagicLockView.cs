using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    [AddComponentMenu(nameof(MagicLockBehaviour) + "/" + nameof(MagicLockView))]
    public class MagicLockView : AbstractLockView, ILockView
    {
        public DragHandler OuterLightDragHandler => _outerLightDragHandler;
        public DragHandler InnerLightDragHandler => _innerLightDragHandler;

        public float DragSmoothness => _dragSmoothness;
        public float MaxDegreeOffset => _maxDegreeOffset;
        public float HoldLightSeconds => _holdLightSeconds;
        public float RotationSpeed => _rotationSpeed;

        public Transform OuterRingContainer => _outerRingContainer;
        public Transform InnerRingContainer => _innerRingContainer;
        public Transform OuterWheel => _outerWheel;
        public Transform InnerWheel => _innerWheel;

        public MagicLockSoundPlayer SoundPlayer => _soundPlayer;

        [Header("Drag settings")]
        [SerializeField] private float _dragSmoothness = 10f;

        [Header("Rotation settings")]
        [SerializeField] private float _maxDegreeOffset = 5f;
        [SerializeField] private float _holdLightSeconds = 3f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("References")]
        [SerializeField] private Transform _outerRingContainer;
        [SerializeField] private Transform _innerRingContainer;
        [SerializeField] private Transform _outerWheel;
        [SerializeField] private Transform _innerWheel;

        [SerializeField] private DragHandler _outerLightDragHandler;
        [SerializeField] private DragHandler _innerLightDragHandler;

        [SerializeField] private MagicLockSoundPlayer _soundPlayer;

        private Camera _mainCamera;

        private void Awake()
        {
            InitializeToken();
        }

        void ICameraGetter.SetCamera(Camera targetCamera)
        {
            _mainCamera = targetCamera;

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

        public Camera GetCamera()
        {
            return _mainCamera;
        }

        public async UniTask DisappearAsync()
        {
            await AppearanceTweener.DisappearAsync(beforeDisappearDelay, lockContainer, cts.Token);
        }
    }
}