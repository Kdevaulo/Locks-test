using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    [Serializable]
    public class ImageRing
    {
        public Collider2D Collider => _collider;
        public SpriteRenderer Renderer => _renderer;
        public Transform Transform => _transform;

        public UserInputHandler InputHandler => _inputHandler;

        [SerializeField] private Collider2D _collider;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _transform;

        [SerializeField] private UserInputHandler _inputHandler;
    }
}