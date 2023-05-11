using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    [AddComponentMenu(nameof(RotateImageLockBehaviour) +
                      "/" +
                      nameof(UserInputHandler) +
                      " in " +
                      nameof(RotateImageLockBehaviour))]
    public class UserInputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public event Action OnPointerDown = delegate { };
        public event Action OnDrag = delegate { };

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown.Invoke();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag.Invoke();
        }
    }
}