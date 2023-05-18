using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    [Serializable]
    public class MagicRing
    {
        public Collider2D[] RotatePointColliders => _rotatePointColliders;

        public Collider2D BorderCollider => _borderCollider;

        [SerializeField] private Collider2D[] _rotatePointColliders;

        [SerializeField] private Collider2D _borderCollider;
    }
}