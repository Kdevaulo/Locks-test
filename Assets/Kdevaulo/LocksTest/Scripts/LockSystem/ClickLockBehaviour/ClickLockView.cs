using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Color _closedLedColor;

        [SerializeField] private Color _openedLedColor;

        [SerializeField] private Timer _openTimer;

        [SerializeField] private Timer _moveTimer;

        [SerializeField] private Text _text;

        [SerializeField] private Button _button;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private ClickSoundPlayer _soundPlayer;

        [SerializeField] private Transform _lockContainer;

        private CancellationToken _cancellationToken;

        private void Awake()
        {
            _button.onClick.AddListener(HandleButtonClick);
            _cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleButtonClick);
        }

        void ILockView.SetCamera(Camera targetCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = targetCamera;
        }

        GameObject ILockView.GetGameObject()
        {
            return gameObject;
        }

        public async UniTask DisappearAsync()
        {
            _text.enabled = false;
            _button.enabled = false;

            await UniTask.WhenAll(
                _lockContainer.DORotate(new Vector3(0, 0, -180), 1f).WithCancellation(_cancellationToken),
                _lockContainer.DOScale(Vector3.zero, 1f).WithCancellation(_cancellationToken));
        }

        public void SetText(string value)
        {
            _text.text = value;
        }

        private void HandleButtonClick()
        {
            Clicked.Invoke();
        }
    }
}