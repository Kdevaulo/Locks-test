using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public static class AppearanceTweener
    {
        private const float Duration = 1f;

        private const Ease EaseType = Ease.Linear;

        private static readonly Vector3 TargetRotation = new Vector3(0, 0, -180);

        public static async UniTask DisappearAsync(float beforeDisappearDelay, Transform targetTransform,
            CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(beforeDisappearDelay), cancellationToken: token);

            await UniTask.WhenAll(targetTransform.DORotate(TargetRotation, Duration).SetEase(EaseType)
                    .WithCancellation(token),
                targetTransform.DOScale(Vector3.zero, Duration).SetEase(EaseType).WithCancellation(token));
        }
    }
}