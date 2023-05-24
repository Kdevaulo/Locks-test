using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour
{
    [AddComponentMenu(nameof(CylinderLockBehaviour) +
                      "/" +
                      nameof(UserInputHandler) +
                      " in " +
                      nameof(CylinderLockBehaviour))]
    [RequireComponent(typeof(Button))]
    public class UserInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CylinderLockView _lockView;

        private bool _isButtonHold;

        public void OnPointerDown(PointerEventData data)
        {
            _isButtonHold = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            _isButtonHold = false;
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Mouse X");

            _lockView.MoveLockPick(-horizontal);

            _lockView.HandleOpenLockKey(Input.GetKey(KeyCode.Space) || _isButtonHold);
        }
    }
}