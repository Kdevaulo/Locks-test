using System;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(ClickLockView))]
    public class ClickLockView : AbstractLockView, ILockView
    {
        public event Action Clicked = delegate { };

        public ClickLockSoundPlayer SoundPlayer => _soundPlayer;
        public PinKit[] PinKits => _pinKits;

        public float MoveLedDelayMultiplier => _moveLedDelayMultiplier;
        public float MinMoveDelay => _minMoveDelay;

        public int MaxDefaultDisabledCount => _maxDefaultDisabledCount;
        public int OpenLockTime => _openLockTime;

        public Color ClosedLedColor => _closedLedColor;
        public Color OpenedLedColor => _openedLedColor;

        public Timer OpenTimer => _openTimer;
        public Timer MoveTimer => _moveTimer;

        [SerializeField] private PinKit[] _pinKits;

        [SerializeField] private int _maxDefaultDisabledCount = 4;
        [SerializeField] private int _openLockTime = 25;

        [SerializeField] private float _moveLedDelayMultiplier = 0.05f;
        [SerializeField] private float _minMoveDelay = 0.05f;

        [Header("References")]
        [SerializeField] private Color _closedLedColor;
        [SerializeField] private Color _openedLedColor;

        [SerializeField] private Timer _openTimer;
        [SerializeField] private Timer _moveTimer;

        [SerializeField] private Canvas _interactCanvas;

        [SerializeField] private Text _text;
        [SerializeField] private Button _interactionButton;

        [SerializeField] private ClickLockSoundPlayer _soundPlayer;

        private void Awake()
        {
            _interactionButton.onClick.AddListener(HandleButtonClick);

            InitializeToken();
        }

        void ICameraGetter.SetCamera(Camera targetCamera)
        {
            SetCameraToCanvas(targetCamera, _interactCanvas);
            SetCameraToCanvas(targetCamera, hintCanvas);
        }

        void ILockView.Dispose()
        {
            _interactionButton.onClick.RemoveListener(HandleButtonClick);

            TryCancelToken();
        }

        void ILockView.DestroyGameObject()
        {
            DestroyGameObject();
        }

        public async UniTask DisappearAsync()
        {
            _text.enabled = false;
            _interactionButton.enabled = false;

            await AppearanceTweener.DisappearAsync(beforeDisappearDelay, lockContainer, cts.Token);
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