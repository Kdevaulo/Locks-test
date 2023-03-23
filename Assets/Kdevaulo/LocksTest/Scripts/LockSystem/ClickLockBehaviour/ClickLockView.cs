using System;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(ClickLockView))]
    public class ClickLockView : MonoBehaviour, ILockView
    {
        public event Action Clicked = delegate { };

        public PinKit[] PinKits => _pinKits;
        public float MoveInterval => _moveInterval;
        public int OpenLockTime => _openLockTime;
        public Color ClosedLedColor => _closedLedColor;
        public Color OpenedLedColor => _openedLedColor;
        public Timer OpenTimer => _openTimer;
        public Timer MoveTimer => _moveTimer;

        [SerializeField] private PinKit[] _pinKits;

        [SerializeField] private float _moveInterval;

        [SerializeField] private int _openLockTime;

        [SerializeField] private Color _closedLedColor;

        [SerializeField] private Color _openedLedColor;

        [SerializeField] private Timer _openTimer;

        [SerializeField] private Timer _moveTimer;

        [SerializeField] private Text _text;

        [SerializeField] private Button _button;

        [SerializeField] private Canvas _canvas;

        private void Awake()
        {
            _button.onClick.AddListener(HandleButtonClick);
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