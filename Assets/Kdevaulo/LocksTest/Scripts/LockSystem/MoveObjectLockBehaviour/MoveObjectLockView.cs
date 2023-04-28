using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [AddComponentMenu(nameof(MoveObjectLockBehaviour) + "/" + nameof(MoveObjectLockView))]
    public class MoveObjectLockView : MonoBehaviour, ILockView
    {
        public event Action<Vector2> ItemMoved = delegate { };

        public Vector3 MovingContainerLocalPosition => _movingObjectContainer.localPosition;

        public float UserMovingSpeed => _userMovingSpeed;

        [SerializeField] private float _userMovingSpeed = 0.1f;

        [SerializeField] private SpriteRenderer _shadowRenderer;

        [SerializeField] private SpriteRenderer _movingObjectRenderer;

        [SerializeField] private Transform _movingObjectContainer;

        [SerializeField] private SpritesData _spritesData;

        [SerializeField] private Canvas _canvas;

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

        public void MoveItem(Vector2 offset)
        {
            ItemMoved.Invoke(offset);
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
    }
}