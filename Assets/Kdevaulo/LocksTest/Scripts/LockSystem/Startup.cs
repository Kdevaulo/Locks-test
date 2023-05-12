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
        [SerializeField] private Button _startButton;

        [SerializeField] private Canvas _pressToContinueCanvas;

        private readonly LocksContainer _locksContainer = new LocksContainer();

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private LocksController _locksController;
        private PressToOpenScreenController _pressToOpenScreenController;

        private void Awake()
        {
            _locksController = new LocksController(_locksContainer, _mainCamera, _skipButton);

            _pressToOpenScreenController = new PressToOpenScreenController(_pressToContinueCanvas, _startButton);

            _disposables.Add(_locksController);

            _pressToOpenScreenController.StartPressed += HandleStartPressed;
            _locksController.LockInitialized += HandleLockInitialized;
        }

        private void Start()
        {
            _locksController.Initialize(_lockPrefabs);
        }

        private void OnDestroy()
        {
            _pressToOpenScreenController.StartPressed -= HandleStartPressed;
            _locksController.LockInitialized -= HandleLockInitialized;

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }

        private void HandleStartPressed()
        {
            _locksController.StartGame();
        }

        private void HandleLockInitialized()
        {
            _pressToOpenScreenController.EnableScreen();
        }
    }
}