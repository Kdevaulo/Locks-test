using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    [AddComponentMenu(nameof(MagicLockBehaviour) +
                      "/" +
                      nameof(DragHandler) +
                      " in " +
                      nameof(MagicLockBehaviour))]
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public event Action<Vector2> OnDrag = delegate { };
        public event Action<Vector2> OnDragBegin = delegate { };
        public event Action<Vector2> OnDragEnd = delegate { };

        public Rigidbody2D Rigidbody => _rigidbody;

        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody;

        private PointerEventData _currentEventData;

        private Vector2 _targetPosition;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _currentEventData = eventData;

            OnDragBegin.Invoke(eventData.position);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (_currentEventData != eventData)
                return;

            OnDrag.Invoke(eventData.position);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _currentEventData = null;

            OnDragEnd.Invoke(eventData.position);
        }
    }
}