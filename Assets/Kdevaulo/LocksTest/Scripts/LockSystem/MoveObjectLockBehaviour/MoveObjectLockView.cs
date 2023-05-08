using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

using Timer = Kdevaulo.LocksTest.Scripts.Utils.Timer;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [AddComponentMenu(nameof(MoveObjectLockBehaviour) + "/" + nameof(MoveObjectLockView))]
    public class MoveObjectLockView : MonoBehaviour, ILockView
    {
        public event Action<Vector2> ItemMoveCalled = delegate { };

        public Vector3 MovingContainerPosition => _movingObjectContainer.position;
        public Vector3 LockContainerPosition => _lockContainer.position;

        public Vector2 StartFillPoint => _startFillPoint;
        public Vector2 EndFillPoint => _endFillPoint;
        public Vector2 ResistMovingSpeedRange => _resistMovingSpeedRange;
        public Vector2 BetweenChangeDirectionSecondsRange => _betweenChangeDirectionSecondsRange;

        public Vector2[] Directions => _directions;

        public Timer LockLoadTimer => _lockLoadTimer;
        public Timer ObjectMoverTimer => _objectMoverTimer;

        public float CorrectAreaRadius => _correctAreaRadius;
        public float UserMovingSpeed => _userMovingSpeed;
        public float FillTickSeconds => _fillTickSeconds;
        public float MaxAreaRadius => _maxAreaRadius;
        public float FillStep => _fillStep;

        public MoveObjectLockSoundPlayer SoundPlayer => _soundPlayer;

        [Header("User settings")]
        [SerializeField] private float _userMovingSpeed = 0.1f;

        [Header("Filler settings")]
        [SerializeField] private float _fillStep = 0.1f;
        [SerializeField] private float _fillTickSeconds = 0.1f;

        [SerializeField] private Vector2 _startFillPoint;
        [SerializeField] private Vector2 _endFillPoint;

        [Header("Correct/wrong behaviour settings")]
        [SerializeField] private float _correctAreaRadius = 1.25f;
        [SerializeField] private float _maxAreaRadius = 1.5f;

        [SerializeField] private Color _correctColor = Color.green;
        [SerializeField] private Color _wrongColor = Color.red;

        [Header("Resist moving settings")]
        [SerializeField] private Vector2 _resistMovingSpeedRange;
        [SerializeField] private Vector2 _betweenChangeDirectionSecondsRange;

        [SerializeField] private Vector2[] _directions;

        [Header("Other settings")]
        [SerializeField] private float _beforeDisappearDelay;

        [Header("References")]
        [SerializeField] private SpriteRenderer _shadowRenderer;
        [SerializeField] private SpriteRenderer _movingObjectRenderer;
        [SerializeField] private SpriteRenderer _fillerRenderer;

        [SerializeField] private Transform _movingObjectContainer;
        [SerializeField] private Transform _movingFillerContainer;
        [SerializeField] private Transform _lockContainer;

        [SerializeField] private Timer _lockLoadTimer;
        [SerializeField] private Timer _objectMoverTimer;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private SpritesData _spritesData;
        [SerializeField] private MoveObjectLockSoundPlayer _soundPlayer;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_lockContainer.position, CorrectAreaRadius);
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

        public void MoveItem(Vector2 offset)
        {
            ItemMoveCalled.Invoke(offset);
        }

        public void SetObjectContainerLocalPosition(Vector3 targetPosition)
        {
            _movingObjectContainer.localPosition = targetPosition;
        }

        public void SetRandomObject()
        {
            var sprite = _spritesData.GetRandomItem();

            _movingObjectRenderer.sprite = sprite;
            _shadowRenderer.sprite = sprite;
        }

        public void SetFillerPosition(Vector2 targetPosition)
        {
            _movingFillerContainer.localPosition = targetPosition;
        }

        public void SetCorrectFillerColor()
        {
            _fillerRenderer.color = _correctColor;
        }

        public void SetWrongFillerColor()
        {
            _fillerRenderer.color = _wrongColor;
        }
    }
}