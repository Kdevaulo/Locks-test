using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class AbstractLockView : MonoBehaviour
    {
        protected CancellationTokenSource cts;

        protected void InitializeToken()
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        }

        protected void SetCameraToCanvas(Camera targetCamera, Canvas canvas)
        {
            canvas.worldCamera = targetCamera;
        }

        protected void TryCancelToken()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        protected void DestroyGameObject()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}