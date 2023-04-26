using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(Startup))]
    public class Startup : MonoBehaviour
    {
        [SerializeField] private GameObject[] _lockPrefabs;

        [SerializeField] private Camera _mainCamera;

        [SerializeField] private Button _skipButton;

        private readonly LocksContainer _locksContainer = new LocksContainer();

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private LocksController _locksController;

        private void Awake()
        {
            _locksController = new LocksController(_locksContainer, _mainCamera, _skipButton);
            _disposables.Add(_locksController);
        }

        private void Start()
        {
            _locksController.Initialize(_lockPrefabs);
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }
    }
}