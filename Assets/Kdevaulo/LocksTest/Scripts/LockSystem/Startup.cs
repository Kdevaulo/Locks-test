using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(Startup))]
    public class Startup : MonoBehaviour
    {
        [SerializeField] private GameObject[] _lockPrefabs;

        [SerializeField] private Camera _mainCamera;

        private readonly LocksContainer _locksContainer = new LocksContainer();

        private LocksController _locksController;

        private void Awake()
        {
            _locksController = new LocksController(_locksContainer, _mainCamera);
        }

        private void Start()
        {
            _locksController.Initialize(_lockPrefabs);
        }
    }
}