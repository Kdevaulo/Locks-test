using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour
{
    [AddComponentMenu(nameof(CylinderLockBehaviour) +
                      "/" +
                      nameof(UserInputHandler) +
                      " in " +
                      nameof(CylinderLockBehaviour))]
    public class UserInputHandler : MonoBehaviour
    {
        [SerializeField] private CylinderLockView _lockView;

        private void Update()
        {
            var horizontal = Input.GetAxis("Mouse X");

            _lockView.MoveLockPick(-horizontal);

            _lockView.HandleOpenLockKey(Input.GetKey(KeyCode.Space));
        }
    }
}