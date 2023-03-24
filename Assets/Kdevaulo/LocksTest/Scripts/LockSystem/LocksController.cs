using System;
using System.Collections.Generic;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

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

        private List<ILockView> _lockViews;

        public LocksController(LocksContainer container, Camera mainCamera)
        {
            _container = container;
            _mainCamera = mainCamera;
        }

        public void Initialize(List<ILockView> lockViews)
        {
            _lockViews = lockViews;

            _randomizer = new Randomizer(0, _lockViews.Count);

            InitializeNewLock();
        }

        private void InitializeNewLock()
        {
            if (_currentLock != null)
            {
                _currentLock.LockOpened -= InitializeNewLock;
                _currentLock.Dispose();

                var gameObject = _currentLockView.GetGameObject();
                gameObject.SetActive(false);
                Object.Destroy(gameObject);
            }

            _currentLockView = _lockViews[_randomizer.GetValue()];

            _currentLockView.SetCamera(_mainCamera);

            var currentLockType = _container.GetLockTypeByView(_currentLockView.GetType());

            _currentLock = (ILock) Activator.CreateInstance(currentLockType, args: _currentLockView);

            _currentLock.LockOpened += InitializeNewLock;

            _currentLock.Initialize();
        }
    }
}