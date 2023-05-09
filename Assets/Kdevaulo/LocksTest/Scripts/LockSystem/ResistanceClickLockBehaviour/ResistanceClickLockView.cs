using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.UI;

using Timer = Kdevaulo.LocksTest.Scripts.Utils.Timer;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour
{
    [AddComponentMenu(nameof(ResistanceClickLockBehaviour) + "/" + nameof(ResistanceClickLockView))]
    public class ResistanceClickLockView : MonoBehaviour, ILockView
    {
        public event Action Clicked = delegate { };

        public Vector2 ScaleRotationRange => _scaleRotationRange;

        public float EnemyAttackDamage => _enemyAttackDamage;
        public float EnemyAttackDelay => _enemyAttackDelay;
        public float UserAttackDamage => _userAttackDamage;

        public Timer AttackTimer => _attackTimer;
        public ResistanceClickLockSoundPlayer SoundPlayer => _soundPlayer;

        [Header("Enemy Attack Settings")]
        [SerializeField] private float _enemyAttackDelay = 0.05f;
        [SerializeField] private float _enemyAttackDamage = 0.3f;

        [Header("User Attack Settings")]
        [SerializeField] private float _userAttackDamage = 6;

        [Header("Other Settings")]
        [SerializeField] private Vector2 _scaleRotationRange;

        [SerializeField] private float _beforeDisappearDelay = 0.5f;

        [Header("References")]
        [SerializeField] private Canvas _textCanvas;
        [SerializeField] private Canvas _interactionCanvas;

        [SerializeField] private Transform _scaleRotatingItem;
        [SerializeField] private Transform _lockContainer;

        [SerializeField] private Button _interactionButton;

        [SerializeField] private Timer _attackTimer;
        [SerializeField] private ResistanceClickLockSoundPlayer _soundPlayer;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _interactionButton.onClick.AddListener(HandleButtonClick);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        void ILockView.SetCamera(Camera targetCamera)
        {
            _interactionCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _interactionCanvas.worldCamera = targetCamera;

            _textCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _textCanvas.worldCamera = targetCamera;
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
            _interactionButton.enabled = false;

            await AppearanceTweener.DisappearAsync(_beforeDisappearDelay, _lockContainer, _cts.Token);
        }

        public void SetRotation(float targetZRotation)
        {
            var currentRotation = _scaleRotatingItem.rotation;
            _scaleRotatingItem.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, targetZRotation);
        }

        public void DisableInteractButton()
        {
            _interactionButton.gameObject.SetActive(false);
        }

        public void DisableTextCanvas()
        {
            _textCanvas.gameObject.SetActive(false);
        }

        private void HandleButtonClick()
        {
            Clicked.Invoke();
        }
    }
}