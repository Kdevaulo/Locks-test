using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public static class AppearanceTweener
    {
        public static async UniTask DisappearAsync(float beforeDisappearDelay, Transform targetTransform,
            CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(beforeDisappearDelay), cancellationToken: token);

            await UniTask.WhenAll(targetTransform.DORotate(new Vector3(0, 0, -180), 1f).SetEase(Ease.Linear)
                    .WithCancellation(token),
                targetTransform.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear).WithCancellation(token));
        }
    }
}