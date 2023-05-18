using System;
using System.Collections.Generic;

using Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour;
using Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour;

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
                {typeof(MoveObjectLockView), typeof(MoveObjectLock)},
                {typeof(ResistanceClickLockView), typeof(ResistanceClickLock)},
                {typeof(RotateImageLockView), typeof(RotateImageLock)},
                {typeof(MagicLockView), typeof(MagicLock)}
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