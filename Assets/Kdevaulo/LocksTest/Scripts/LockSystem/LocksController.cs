using System;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class LocksController
    {
        private readonly Camera _mainCamera;

        private readonly LocksContainer _container;

        private Randomizer _randomizer;

        private ILock _currentLock;

        private ILockView _currentLockView;

        private GameObject[] _lockViewPrefabs;

        public LocksController(LocksContainer container, Camera mainCamera)
        {
            _container = container;
            _mainCamera = mainCamera;
        }

        public void Initialize(GameObject[] lockViewPrefabs)
        {
            _lockViewPrefabs = lockViewPrefabs;

            _randomizer = new Randomizer(0, _lockViewPrefabs.Length);

            InitializeNewLock();
        }

        private void InitializeNewLock()
        {
            if (_currentLock != null)
            {
                _currentLock.LockOpened -= InitializeNewLock;
                _currentLock.Dispose();

                _currentLockView.Dispose();

                var gameObject = _currentLockView.GetGameObject();
                gameObject.SetActive(false);
                Object.Destroy(gameObject);
            }

            var lockViewPrefab = _lockViewPrefabs[_randomizer.GetValue()];

            var viewGameObject = Object.Instantiate(lockViewPrefab);

            _currentLockView = viewGameObject.GetComponent<ILockView>();

            Assert.IsNotNull(_currentLockView);

            _currentLockView.SetCamera(_mainCamera);

            var currentLockType = _container.GetLockTypeByView(_currentLockView.GetType());

            _currentLock = (ILock) Activator.CreateInstance(currentLockType, args: _currentLockView);

            _currentLock.LockOpened += InitializeNewLock;

            _currentLock.Initialize();
        }
    }
}