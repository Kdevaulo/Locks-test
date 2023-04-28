using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [AddComponentMenu(nameof(MoveObjectLockBehaviour) +
                      "/" +
                      nameof(UserInputHandler) +
                      " in " +
                      nameof(MoveObjectLockBehaviour))]
    public class UserInputHandler : MonoBehaviour
    {
        [SerializeField] private MoveObjectLockView _lockView;

        private void Update()
        {
            var horizontal = Input.GetAxis("Mouse X");
            var vertical = Input.GetAxis("Mouse Y");

            _lockView.MoveItem(new Vector2(horizontal, vertical));
        }
    }
}