using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(Startup))]
    public class Startup : MonoBehaviour
    {
        [SerializeField] private GameObject[] _lockPrefabs;

        [SerializeField] private Camera _mainCamera;

        private List<ILockView> _lockViews = new List<ILockView>();

        private LocksContainer _locksContainer = new LocksContainer();

        private LocksController _locksController;

        private void Awake()
        {
            _locksController = new LocksController(_locksContainer, _mainCamera);
        }

        private void Start()
        {
            foreach (var lockPrefab in _lockPrefabs)
            {
                var lockInstance = Instantiate(lockPrefab, transform);

                lockInstance.TryGetComponent(out ILockView lockBehaviour);

                Assert.IsNotNull(lockBehaviour);

                _lockViews.Add(lockBehaviour);
            }

            _locksController.Initialize(_lockViews);
        }
    }
}