using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

using Object = System.Object;
using Timer = Kdevaulo.LocksTest.Scripts.Utils.Timer;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(ClickLockView))]
    public class ClickLockView : MonoBehaviour, ILockView
    {
        public event Action Clicked = delegate { };

        public PinKit[] PinKits => _pinKits;
        public int MaxDefaultDisabledCount => _maxDefaultDisabledCount;
        public float MoveLedDelayMultiplier => _moveLedDelayMultiplier;
        public float MinMoveDelay => _minMoveDelay;
        public int OpenLockTime => _openLockTime;
        public Color ClosedLedColor => _closedLedColor;
        public Color OpenedLedColor => _openedLedColor;
        public Timer OpenTimer => _openTimer;
        public Timer MoveTimer => _moveTimer;

        public ClickSoundPlayer SoundPlayer => _soundPlayer;

        [SerializeField] private PinKit[] _pinKits;

        [SerializeField] private int _maxDefaultDisabledCount = 4;

        [SerializeField] private float _moveLedDelayMultiplier = 0.05f;

        [SerializeField] private float _minMoveDelay = 0.05f;

        [SerializeField] private int _openLockTime = 25;

        [SerializeField] private float _beforeDisappearDelay = 0.5f;

        [SerializeField] private Color _closedLedColor;

        [SerializeField] private Color _openedLedColor;

        [SerializeField] private Timer _openTimer;

        [SerializeField] private Timer _moveTimer;

        [SerializeField] private Text _text;

        [SerializeField] private Button _interactionButton;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private ClickSoundPlayer _soundPlayer;

        [SerializeField] private Transform _lockContainer;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _interactionButton.onClick.AddListener(HandleButtonClick);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        void ILockView.SetCamera(Camera targetCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = targetCamera;
        }

        void ILockView.Dispose()
        {
            _interactionButton.onClick.RemoveListener(HandleButtonClick);

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
            _text.enabled = false;
            _interactionButton.enabled = false;

            await UniTask.Delay(TimeSpan.FromSeconds(_beforeDisappearDelay), cancellationToken: _cts.Token);

            await UniTask.WhenAll(_lockContainer.DORotate(new Vector3(0, 0, -180), 1f).SetEase(Ease.Linear)
                    .WithCancellation(_cts.Token),
                _lockContainer.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear).WithCancellation(_cts.Token));
        }

        public void SetText(string value)
        {
            _text.text = value;
        }

        public void DisableInteractionButton()
        {
            _interactionButton.gameObject.SetActive(false);
        }

        private void HandleButtonClick()
        {
            Clicked.Invoke();
        }
    }
}