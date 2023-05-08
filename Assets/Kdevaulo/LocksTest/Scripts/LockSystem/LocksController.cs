using System;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class LocksController : IDisposable
    {
        private readonly Camera _mainCamera;
        private readonly Button _skipButton;

        private readonly LocksContainer _container;

        private ILock _currentLock;
        private ILockView _currentLockView;

        private Randomizer _randomizer;

        private GameObject[] _lockViewPrefabs;

        public LocksController(LocksContainer container, Camera mainCamera, Button skipButton)
        {
            _container = container;
            _mainCamera = mainCamera;
            _skipButton = skipButton;
        }

        void IDisposable.Dispose()
        {
            _skipButton.onClick.RemoveListener(SkipLock);
        }

        public void Initialize(GameObject[] lockViewPrefabs)
        {
            _lockViewPrefabs = lockViewPrefabs;
            _randomizer = new Randomizer(0, _lockViewPrefabs.Length);

            _skipButton.onClick.AddListener(SkipLock);

            InitializeNewLock();
        }

        private void SkipLock()
        {
            InitializeNewLock();
        }

        private void InitializeNewLock()
        {
            if (_currentLock != null)
            {
                DestroyCurrentLock();
            }

            CreateLockView();

            CreateLock();

            _currentLockView.SetCamera(_mainCamera);

            _currentLock.LockOpened += InitializeNewLock;
            _currentLock.Initialize();
        }

        private void DestroyCurrentLock()
        {
            _currentLock.LockOpened -= InitializeNewLock;
            _currentLock.Dispose();

            _currentLockView.Dispose();

            _currentLockView.DestroyGameObject();
        }

        private void CreateLockView()
        {
            var lockViewPrefab = _lockViewPrefabs[_randomizer.GetValue()];

            var viewGameObject = Object.Instantiate(lockViewPrefab);

            _currentLockView = viewGameObject.GetComponent<ILockView>();

            Assert.IsNotNull(_currentLockView);
        }

        private void CreateLock()
        {
            var currentLockType = _container.GetLockTypeByView(_currentLockView.GetType());

            _currentLock = (ILock) Activator.CreateInstance(currentLockType, args: _currentLockView);
        }
    }
}