using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    [Serializable]
    public class ImageRingContainer
    {
        public ImageRing[] ImageRingCollection => _imageRingCollection;

        [SerializeField] private ImageRing[] _imageRingCollection;
    }
}