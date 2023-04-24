using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts
{
    public interface ILockView
    {
        void SetCamera(Camera targetCamera);
        void Dispose();
        void DestroyGameObject();
    }
}