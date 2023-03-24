using System;

namespace Kdevaulo.LocksTest.Scripts
{
    public interface ILock
    {
        event Action LockOpened;
        void Initialize();
        void Dispose();
    }
}