using System;

using Cysharp.Threading.Tasks;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour
{
    [AddComponentMenu(nameof(ResistanceClickLockBehaviour) + "/" + nameof(ResistanceClickLockView))]
    public class ResistanceClickLockView : AbstractLockView, ILockView
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

        [Header("References")]
        [SerializeField] private Canvas _textCanvas;
        [SerializeField] private Canvas _interactionCanvas;

        [SerializeField] private Transform _scaleRotatingItem;

        [SerializeField] private Button _interactionButton;

        [SerializeField] private Timer _attackTimer;
        [SerializeField] private ResistanceClickLockSoundPlayer _soundPlayer;

        private void Awake()
        {
            _interactionButton.onClick.AddListener(HandleButtonClick);

            InitializeToken();
        }

        void ICameraGetter.SetCamera(Camera targetCamera)
        {
            SetCameraToCanvas(targetCamera, _interactionCanvas);
            SetCameraToCanvas(targetCamera, _textCanvas);
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
            _interactionButton.enabled = false;

            await AppearanceTweener.DisappearAsync(beforeDisappearDelay, lockContainer, cts.Token);
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