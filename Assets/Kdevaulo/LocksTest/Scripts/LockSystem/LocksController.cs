using System;
using System.Collections.Generic;

using Kdevaulo.LocksTest.Scripts.Utils;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class LocksController
    {
        private readonly Camera _mainCamera;

        private LocksContainer _container;

        private Randomizer _randomizer;

        public LocksController(LocksContainer container, Camera mainCamera)
        {
            _container = container;
            _mainCamera = mainCamera;
        }

        public void Initialize(List<ILockView> lockViews)
        {
            _randomizer = new Randomizer(0, lockViews.Count);

            var currentView = lockViews[_randomizer.GetValue()];

            currentView.SetCamera(_mainCamera);

            var currentLockType = _container.GetLockTypeByView(currentView.GetType());

            var currentLock = (ILock) Activator.CreateInstance(currentLockType, args: currentView);
            currentLock.Initialize();
        }
    }
}