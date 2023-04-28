using System;
using System.Collections.Generic;

using Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour;

using UnityEngine.Assertions;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class LocksContainer
    {
        private Dictionary<Type, Type> _locksWithViews;

        public LocksContainer()
        {
            _locksWithViews = new Dictionary<Type, Type>
            {
                // note: add new lock types here
                {typeof(ClickLockView), typeof(ClickLock)},
                {typeof(CylinderLockView), typeof(CylinderLock)},
                {typeof(MoveObjectLockView), typeof(MoveObjectLock)}
            };

            foreach (var pair in _locksWithViews)
            {
                Assert.IsNotNull(pair.Key.GetInterface(nameof(ILockView)));
                Assert.IsNotNull(pair.Value.GetInterface(nameof(ILock)));
            }
        }

        public Type GetLockTypeByView(Type viewType)
        {
            return _locksWithViews[viewType];
        }
    }
}