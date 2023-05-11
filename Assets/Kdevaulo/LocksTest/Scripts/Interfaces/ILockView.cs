namespace Kdevaulo.LocksTest.Scripts
{
    public interface ILockView : ICameraGetter
    {
        void Dispose();
        void DestroyGameObject();
    }
}