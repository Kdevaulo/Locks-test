using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public static class RandomTransformZRotator
    {
        private const float MinDuration = 1f;
        private const float MaxDuration = 2f;
        private const float MaxAngle = 360f;
        // note: if max angle > 360 - DORotate needed in different RotateMode

        /// <summary>
        /// Rotates transform with random duration (1-2s) and to random angle (0-360)
        /// </summary>
        public static async UniTask DoRandomRotate(Transform transform)
        {
            var randomZAngle = Random.Range(0f, MaxAngle);
            var duration = Random.Range(MinDuration, MaxDuration);

            await transform.DORotate(new Vector3(0, 0, randomZAngle), duration).SetEase(Ease.InOutSine);
        }
    }
}