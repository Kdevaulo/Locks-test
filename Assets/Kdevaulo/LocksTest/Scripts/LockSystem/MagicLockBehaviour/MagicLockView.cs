using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    [AddComponentMenu(nameof(MagicLockBehaviour) + "/" + nameof(MagicLockView))]
    public class MagicLockView : AbstractLockView, ILockView
    {
        public DragHandler OuterLightDragHandler => _outerLightDragHandler;
        public DragHandler InnerLightDragHandler => _innerLightDragHandler;

        public float DragSmoothness => _dragSmoothness;

        [Header("Drag settings")]
        [SerializeField] private float _dragSmoothness = 0.5f;

        [Header("References")]
        [SerializeField] private MagicRing _outerRing;
        [SerializeField] private MagicRing _innerRing;

        [SerializeField] private DragHandler _outerLightDragHandler;
        [SerializeField] private DragHandler _innerLightDragHandler;

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
    }
}