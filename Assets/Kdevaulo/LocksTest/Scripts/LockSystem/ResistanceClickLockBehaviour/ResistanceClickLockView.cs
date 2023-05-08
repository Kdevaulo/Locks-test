using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour
{
    [AddComponentMenu(nameof(ResistanceClickLockBehaviour) + "/" + nameof(ResistanceClickLockView))]
    public class ResistanceClickLockView : MonoBehaviour, ILockView
    {
        public Vector2 ScaleRotationRange => _scaleRotationRange;

        public float EnemyAttackDamage => _enemyAttackDamage;
        public float EnemyAttackDelay => _enemyAttackDelay;
        public float UserAttackDamage => _userAttackDamage;

        public Timer AttackTimer => _attackTimer;

        [Header("Enemy Attack Settings")]
        [SerializeField] private float _enemyAttackDelay = 0.05f;
        [SerializeField] private float _enemyAttackDamage = 0.05f;

        [Header("User Attack Settings")]
        [SerializeField] private float _userAttackDamage = 2;

        [Header("Other Settings")]
        [SerializeField] private Vector2 _scaleRotationRange;

        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Transform _scaleRotatingItem;

        [SerializeField] private Timer _attackTimer;

        void ILockView.SetCamera(Camera targetCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = targetCamera;
        }

        void ILockView.Dispose()
        {
        }

        void ILockView.DestroyGameObject()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public void SetRotation(float targetZRotation)
        {
            var currentRotation = _scaleRotatingItem.rotation;
            _scaleRotatingItem.Rotate(currentRotation.x, currentRotation.y, targetZRotation);
        }
    }
}